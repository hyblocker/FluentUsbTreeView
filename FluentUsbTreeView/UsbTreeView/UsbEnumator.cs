using FluentUsbTreeView.PInvoke;
using FluentUsbTreeView.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using Wpf.Ui.Controls;
using static FluentUsbTreeView.PInvoke.CfgMgr32;
using static FluentUsbTreeView.PInvoke.UsbApi;
using static System.Net.Mime.MediaTypeNames;
using WpfTreeViewItem = System.Windows.Controls.TreeViewItem;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace FluentUsbTreeView.UsbTreeView {
    // Basically a C# port of enum.c from the Microsoft usbview with a few alterations since C# allows me to write cleaner code in some aspects
    // the native win32 bit of this will always be ugly though lol
    public static class UsbEnumator {
        private static bool gDoConfigDesc = true; // @TODO: Migrate to settings

        private const int NUM_STRING_DESC_TO_GET = 32;
        private const int MAX_DRIVER_KEY_NAME = 256;

        private static uint s_totalHostControllers;
        private static uint s_totalRootHubs;
        private static uint s_totalStandardHubs;
        private static uint s_totalPeripheralDevices;

        private static LinkedList<UsbHostControllerInfo> EnumeratedHCListHead = new LinkedList<UsbHostControllerInfo>();

        private static DeviceGuidList s_HubList;
        private static DeviceGuidList s_DeviceList;

        static UsbEnumator() {
            s_HubList = new DeviceGuidList();
            s_DeviceList = new DeviceGuidList();
        }

        private static readonly string[] ConnectionStatuses =
        {
            "",                   // 0  - NoDeviceConnected
            "",                   // 1  - DeviceConnected
            "FailedEnumeration",  // 2  - DeviceFailedEnumeration
            "GeneralFailure",     // 3  - DeviceGeneralFailure
            "Overcurrent",        // 4  - DeviceCausedOvercurrent
            "NotEnoughPower",     // 5  - DeviceNotEnoughPower
            "NotEnoughBandwidth", // 6  - DeviceNotEnoughBandwidth
            "HubNestedTooDeeply", // 7  - DeviceHubNestedTooDeeply
            "InLegacyHub",        // 8  - DeviceInLegacyHub
            "Enumerating",        // 9  - DeviceEnumerating
            "Reset"               // 10 - DeviceReset
        };

        public static void EnumerateHostControllers(WpfTreeViewItem hTreeParent, ref UsbTreeState TreeState) {

            IntPtr                              hHCDev = IntPtr.Zero;
            IntPtr                              deviceInfo = IntPtr.Zero;
            SP_DEVINFO_DATA                     deviceInfoData = new SP_DEVINFO_DATA();
            SP_DEVICE_INTERFACE_DATA            deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            IntPtr                              deviceDetailData = IntPtr.Zero;
            uint                                index = 0;
            uint                                requiredLength = 0;
            bool                                success;

            EnumeratedHCListHead.Clear();

            s_totalHostControllers = 0;
            s_totalRootHubs = 0;
            s_totalStandardHubs = 0;
            s_totalPeripheralDevices = 0;
            EnumerateAllDevices();

            // Iterate over host controllers using the new GUID based interface
            Guid usbHostControllerClass = WinApiGuids.GUID_CLASS_USB_HOST_CONTROLLER;
            deviceInfo = SetupApi.SetupDiGetClassDevs(ref usbHostControllerClass, null, IntPtr.Zero, ( DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE ));

            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

            for ( index = 0; SetupApi.SetupDiEnumDeviceInfo(deviceInfo, index, ref deviceInfoData); index++ ) {
                deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

                success = SetupApi.SetupDiEnumDeviceInterfaces(deviceInfo, IntPtr.Zero, ref usbHostControllerClass, index, ref deviceInterfaceData);

                if ( !success ) {
                    HandleNativeFailure();
                    break;
                }

                success = SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref deviceInterfaceData, IntPtr.Zero, 0, ref requiredLength, IntPtr.Zero);

                if ( !success && Marshal.GetLastWin32Error() != Kernel32.ERROR_INSUFFICIENT_BUFFER ) {
                    HandleNativeFailure();
                    break;
                }

                deviceDetailData = Marshal.AllocHGlobal(( int ) requiredLength);
                try {
                    Marshal.WriteInt32(deviceDetailData, 8);
                    success = SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfo, ref deviceInterfaceData, deviceDetailData, requiredLength, ref requiredLength, IntPtr.Zero);

                    if ( !success ) {
                        HandleNativeFailure();
                        break;
                    }

                    // Extract string device path from the pointer, since casting seems to fail
                    IntPtr pDetailDataView_DevicePath = new IntPtr(deviceDetailData.ToInt64() + 4);
                    string devicePath = Marshal.PtrToStringAuto(pDetailDataView_DevicePath);

                    // Open handle
                    hHCDev = Kernel32.CreateFile(devicePath, Kernel32.GENERIC_WRITE, Kernel32.FILE_SHARE_WRITE, 0, Kernel32.OPEN_EXISTING, 0, 0);

                    // If the handle is valid, then we've successfully opened a Host
                    // Controller.  Display some info about the Host Controller itself,
                    // then enumerate the Root Hub attached to the Host Controller.
                    if ( hHCDev != Kernel32.INVALID_HANDLE_VALUE ) {
                        EnumerateHostController(hTreeParent, hHCDev, ref devicePath, deviceInfo, ref deviceInfoData);
                        Kernel32.CloseHandle(hHCDev);
                    }
                } finally {
                    Marshal.FreeHGlobal(deviceDetailData);
                }
            }

            SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);

            TreeState.HostControllers       = s_totalHostControllers;
            TreeState.RootHubs              = s_totalRootHubs;
            TreeState.ExternalHubs          = s_totalStandardHubs;
            TreeState.PeripheralDevices     = s_totalPeripheralDevices;
        }

        public static void EnumerateHostController(WpfTreeViewItem hTreeParent, IntPtr hHCDev, ref string devicePath, IntPtr deviceInfo, ref SP_DEVINFO_DATA deviceInfoData) {
            string                  driverKeyName = null;
            WpfTreeViewItem         hHCItem = null;
            string                  rootHubName = null;
            LinkedListNode<UsbHostControllerInfo>             listEntry = null;
            UsbHostControllerInfo   hcInfo = new UsbHostControllerInfo();
            // USBHOSTCONTROLLERINFO   hcInfoInList = new USBHOSTCONTROLLERINFO();
            Int32                   dwSuccess;
            bool                    success = false;
            uint                    deviceAndFunction = 0;
            UsbDevicePnpStrings  DevProps = new UsbDevicePnpStrings();


            // Allocate a structure to hold information about this host controller.
            hcInfo.DeviceInfoType = UsbDeviceInfoType.HostController;

            // Obtain the driver key name for this host controller.
            driverKeyName = GetHCDDriverKeyName(hHCDev);

            if ( driverKeyName == null ) {
                // Failure obtaining driver key name.
                HandleNativeFailure();
                return;
            }

            // Don't enumerate this host controller again if it already
            // on the list of enumerated host controllers.
            foreach ( UsbHostControllerInfo hcInfoInList in EnumeratedHCListHead ) {
                if ( driverKeyName == hcInfoInList.UsbDeviceProperties.DriverKey ) {
                    // Already on the list, exit
                    return;
                }
            }

            // Prepare the node for this entry on the tree
            listEntry = new LinkedListNode<UsbHostControllerInfo>(hcInfo);
            hcInfo.ListEntry = listEntry;

            // Obtain host controller device properties
            if ( driverKeyName.Length < MAX_DRIVER_KEY_NAME ) {
                DevProps = DeviceNode.DriverNameToDeviceProperties(driverKeyName, driverKeyName.Length);
                if (DevProps == null) {
                    DeviceInfoNode devNode = UsbEnumator.FindMatchingDeviceNodeForDevicePath(devicePath, true);
                    DevProps = DeviceNode.PollDeviceProperties(devNode.DeviceInfo, devNode.DeviceInfoData);
                }
            }

            hcInfo.UsbDeviceProperties = DevProps;
            hcInfo.UsbDeviceProperties.DevicePath = devicePath;
            hcInfo.UsbDeviceProperties.DriverKey = driverKeyName;

            // Get the USB Host Controller power map
            dwSuccess = GetHostControllerPowerMap(hHCDev, ref hcInfo);

            if ( Kernel32.ERROR_SUCCESS != dwSuccess ) {
                HandleNativeFailure();
            }

            // Get bus, device, and function
            hcInfo.BusDeviceFunctionValid = false;

            // all this extra work when we could just pass by ref :D
            IntPtr tempUintPtr = Marshal.AllocHGlobal(sizeof(uint));
            success = SetupApi.SetupDiGetDeviceRegistryProperty(deviceInfo, ref deviceInfoData, DevRegProperty.SPDRP_BUSNUMBER, out _, tempUintPtr, sizeof(uint), out _);
            byte[] busNumberBytes = BitConverter.GetBytes(hcInfo.BusNumber);
            Marshal.Copy(busNumberBytes, 0, tempUintPtr, busNumberBytes.Length);

            if ( success ) {
                success = SetupApi.SetupDiGetDeviceRegistryProperty(deviceInfo, ref deviceInfoData, DevRegProperty.SPDRP_ADDRESS, out _, tempUintPtr, sizeof(uint), out _);
                // Copy into data
                byte[] deviceAndFunctionBytes = BitConverter.GetBytes(deviceAndFunction);
                Marshal.Copy(deviceAndFunctionBytes, 0, tempUintPtr, deviceAndFunctionBytes.Length);
            }

            Marshal.FreeHGlobal(tempUintPtr);

            if ( success ) {
                hcInfo.BusDevice = ( ushort ) ( deviceAndFunction >> 16 );
                hcInfo.BusFunction = ( ushort ) ( deviceAndFunction & 0xffff );
                hcInfo.BusDeviceFunctionValid = true;
            }

            // Get the USB Host Controller info
            dwSuccess = GetHostControllerInfo(hHCDev, ref hcInfo);

            if ( Kernel32.ERROR_SUCCESS != dwSuccess ) {
                HandleNativeFailure();
            }

            // Get the USB Host Controller bandwidth info
            dwSuccess = GetHostControllerBandwidth(hHCDev, ref hcInfo);
            if ( Kernel32.ERROR_SUCCESS != dwSuccess ) {
                HandleNativeFailure();
            }

            string leafName = DeviceNameUtil.GetFriendlyUsbHostControllerName(hcInfo);

            // Add this host controller to the USB device tree view.
            hHCItem = MainWindow.Instance.AddLeaf(hTreeParent, hcInfo, leafName, UsbTreeIcon.HostController, true);

            if ( hHCItem == null ) {
                // Failure adding host controller to USB device tree
                // view.
                HandleNativeFailure();
                return;
            }

            hcInfo.SymbolicLink = GetRootHubName(hHCDev);

            // Track this host controller
            s_totalHostControllers++;

            // Add this host controller to the list of enumerated
            // host controllers.
            EnumeratedHCListHead.AddLast(hcInfo.ListEntry);

            // Get the name of the root hub for this host
            // controller and then enumerate the root hub.
            rootHubName = hcInfo.SymbolicLink;

            if ( rootHubName != null ) {
                if ( rootHubName.Length < MAX_DRIVER_KEY_NAME ) {
                    EnumerateHub(hHCItem,
                                 rootHubName,
                                 null,       // ConnectionInfo
                                 null,       // ConnectionInfoV2
                                 null,       // PortConnectorProps
                                 null,       // ConfigDesc
                                 null,       // BosDesc
                                 null,       // StringDescs
                                 null);      // We do not pass DevProps for RootHub
                }
            } else {
                // Failure obtaining root hub name.
                HandleNativeFailure();
            }
        }

        private static void EnumerateHub(
            WpfTreeViewItem hTreeParent,
            string HubName,
            USB_NODE_CONNECTION_INFORMATION_EX? ConnectionInfo,
            USB_NODE_CONNECTION_INFORMATION_EX_V2? ConnectionInfoV2,
            USB_PORT_CONNECTOR_PROPERTIES? PortConnectorProps,
            USB_CONFIGURATION_DESCRIPTOR? ConfigDesc,
            USB_BOS_DESCRIPTOR? BosDesc,
            StringDescriptorsCollection StringDescs,
            UsbDevicePnpStrings DevProps
        ) {
            // Initialize locals to not allocated state so the error cleanup routine
            // only tries to cleanup things that were successfully allocated.
            //
            USB_NODE_INFORMATION    hubInfo = new USB_NODE_INFORMATION();
            USB_HUB_INFORMATION_EX  hubInfoEx = new USB_HUB_INFORMATION_EX();
            USB_HUB_CAPABILITIES_EX hubCapabilityEx = new USB_HUB_CAPABILITIES_EX();
            IntPtr                  hHubDevice = Kernel32.INVALID_HANDLE_VALUE;
            WpfTreeViewItem         hItem = null;
            UsbHubInfo              info = new UsbHubInfo();
            string                  devicePath = null;
            string                  driverKeyName = null;
            int                     nBytes = 0;
            bool                    success = false;
            string                  leafName = "";

            // Allocate some space for a USBDEVICEINFO structure to hold the
            // hub info, hub name, and connection info pointers.  GPTR zero
            // initializes the structure for us.

            // Allocate some space for a USB_NODE_INFORMATION structure for this Hub
            hubInfo = new USB_NODE_INFORMATION();
            hubInfoEx = new USB_HUB_INFORMATION_EX();
            hubCapabilityEx = new USB_HUB_CAPABILITIES_EX();

            bool isRootHub = ConnectionInfo == null;

            // Keep copies of the Hub Name, Connection Info, and Configuration
            // Descriptor pointers
            info.HubName = HubName;
            info.PortConnectorProps = PortConnectorProps;
            info.UsbDeviceProperties = DevProps;

            if ( !isRootHub ) {
                info.DeviceInfoType         = UsbDeviceInfoType.ExternalHub;
                info.ConnectionInfo         = ConnectionInfo;
                info.ConfigDesc             = ConfigDesc;
                info.StringDescs            = StringDescs;
                info.BosDesc                = BosDesc;
                info.ConnectionInfoV2       = ConnectionInfoV2;
                s_totalStandardHubs++;
            } else {
                info.DeviceInfoType         = UsbDeviceInfoType.RootHub;
                s_totalRootHubs++;
            }

            // Allocate a temp buffer for the full hub device name.
            devicePath = "\\\\?\\" + HubName;

            // Try to hub the open device
            hHubDevice = Kernel32.CreateFile(devicePath, Kernel32.GENERIC_WRITE, Kernel32.FILE_SHARE_WRITE, 0, Kernel32.OPEN_EXISTING, 0, 0);

            if ( hHubDevice == Kernel32.INVALID_HANDLE_VALUE ) {
                HandleNativeFailure();
                return;
            }

            // Now query USBHUB for the USB_NODE_INFORMATION structure for this hub.
            // This will tell us the number of downstream ports to enumerate, among
            // other things.
            success = Kernel32.DeviceIoControl(hHubDevice, Kernel32.IOCTL_USB_GET_NODE_INFORMATION, ref hubInfo, Marshal.SizeOf(typeof(USB_NODE_INFORMATION)), out hubInfo, Marshal.SizeOf(typeof(USB_NODE_INFORMATION)), out nBytes, IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                return;
            }
            info.HubInfo = hubInfo;

            success = Kernel32.DeviceIoControl(hHubDevice, Kernel32.IOCTL_USB_GET_HUB_INFORMATION_EX, ref hubInfoEx, Marshal.SizeOf(typeof(USB_HUB_INFORMATION_EX)), out hubInfoEx, Marshal.SizeOf(typeof(USB_HUB_INFORMATION_EX)), out nBytes, IntPtr.Zero);

            info.HubInfoEx = hubInfoEx;

            // Fail gracefully for downlevel OS's from Win8
            if ( !success || nBytes < Marshal.SizeOf(typeof(USB_HUB_INFORMATION_EX)) ) {
                info.HubInfoEx = null;
            }

            // Obtain Hub Capabilities
            success = Kernel32.DeviceIoControl(hHubDevice, Kernel32.IOCTL_USB_GET_HUB_CAPABILITIES_EX, ref hubCapabilityEx, Marshal.SizeOf(typeof(USB_HUB_CAPABILITIES_EX)), out hubCapabilityEx, Marshal.SizeOf(typeof(USB_HUB_CAPABILITIES_EX)), out nBytes, IntPtr.Zero);

            info.HubCapabilityEx = hubCapabilityEx;
            // Fail gracefully
            if ( !success || nBytes < Marshal.SizeOf(typeof(USB_HUB_CAPABILITIES_EX)) ) {
                info.HubCapabilityEx = null;
            }

            // Since this is a root hub the index is ALWAYS 1
            driverKeyName = GetDriverKeyName(hHubDevice, 1);

            // @TODO: a
            if ( driverKeyName == null ) {
                DeviceInfoNode deviceNode = FindMatchingDeviceNodeForDevicePath(devicePath, true);
                if ( deviceNode != null ) {
                    driverKeyName = deviceNode.DeviceDriverName;
                }
                // if (!DeviceNode.DevicePathToDrvierKeyName(devicePath, out driverKeyName)) {
                //     HandleNativeFailure();
                // }
            }
            if ( DevProps == null ) {
                DeviceInfoNode devNode = FindMatchingDeviceNodeForDevicePath(devicePath, true);
                if (devNode != null ) {
                    DevProps = DeviceNode.PollDeviceProperties(devNode.DeviceInfo, devNode.DeviceInfoData);
                }
            }
            info.UsbDeviceProperties = DevProps;
            info.UsbDeviceProperties.DevicePath = devicePath;
            info.UsbDeviceProperties.DriverKey = driverKeyName;

            // Build the leaf name from the port number and the device description
            if ( !isRootHub ) {
                leafName = $"[Port{ConnectionInfo.Value.ConnectionIndex}] " + ConnectionStatuses[(int)ConnectionInfo.Value.ConnectionStatus] + " :  ";
                // StringCchPrintf(leafName, dwSizeOfLeafName, "[Port%d] ", ConnectionInfo->ConnectionIndex);
                // StringCchCat(leafName,
                //     dwSizeOfLeafName,
                //     ConnectionStatuses[ConnectionInfo->ConnectionStatus]);
                // StringCchCatN(leafName,
                //     dwSizeOfLeafName,
                //     " :  ",
                //     sizeof(" :  "));
            }

            if ( DevProps != null ) {
                leafName = leafName + info.UsbDeviceProperties.DeviceDesc;
                // leafName = leafName + DeviceNameUtil.GetFriendlyUsbHubName(info);
            } else {
                if ( isRootHub ) {
                    // External hub
                    leafName = leafName + HubName;
                    // leafName = leafName + DeviceNameUtil.GetFriendlyUsbHubName(info);
                } else {
                    // Root hub
                    leafName = leafName + "RootHub";
                }
            }

            // Now add an item to the TreeView with the PUSBDEVICEINFO pointer info
            // as the LPARAM reference value containing everything we know about the
            // hub.
            hItem = MainWindow.Instance.AddLeaf(hTreeParent, info, leafName, UsbTreeIcon.UsbHub, isRootHub);

            if ( hItem == null ) {
                HandleNativeFailure();
                return;
            }

            // Now recursively enumerate the ports of this hub.
            EnumerateHubPorts( hItem, hHubDevice, devicePath, hubInfo.u.HubInformation.HubDescriptor.bNumberOfPorts );


            Kernel32.CloseHandle(hHubDevice);
            return;
        }

        private static unsafe void EnumerateHubPorts( WpfTreeViewItem hTreeParent, IntPtr hHubDevice, string devicePath, uint NumPorts) {
            uint                index = 0;
            bool                success = false;
            // HRESULT             hr = S_OK;
            string              driverKeyName = null;
            UsbDevicePnpStrings DevProps;
            int                 dwSizeOfLeafName = 0;
            string              leafName;
            UsbTreeIcon         icon = 0;

            USB_NODE_CONNECTION_INFORMATION_EX      connectionInfoEx;
            USB_PORT_CONNECTOR_PROPERTIES           pPortConnectorProps;
            USB_PORT_CONNECTOR_PROPERTIES           portConnectorProps;
            USB_CONFIGURATION_DESCRIPTOR?           configDesc;
            USB_BOS_DESCRIPTOR?                     bosDesc;
            StringDescriptorsCollection             stringDescs;
            USBDEVICEINFO                           info;
            USB_NODE_CONNECTION_INFORMATION_EX_V2   connectionInfoExV2;
            bool                                    connectionInfoExV2HasValue = true;
            DeviceInfoNode                          pNode;

            // Loop over all ports of the hub.
            //
            // Port indices are 1 based, not 0 based.
            //
            for (index = 1; index <= NumPorts; index++)
            {
                int nBytesEx;
                int nBytes = 0;

                connectionInfoEx = new USB_NODE_CONNECTION_INFORMATION_EX();
                portConnectorProps = new USB_PORT_CONNECTOR_PROPERTIES();
                pPortConnectorProps = new USB_PORT_CONNECTOR_PROPERTIES();
                // ZeroMemory(&portConnectorProps, sizeof(portConnectorProps));
                configDesc = new USB_CONFIGURATION_DESCRIPTOR();
                bosDesc = new USB_BOS_DESCRIPTOR();
                stringDescs = null;
                info = new USBDEVICEINFO();
                connectionInfoExV2 = new USB_NODE_CONNECTION_INFORMATION_EX_V2();
                pNode = new DeviceInfoNode();
                DevProps = null;
                // ZeroMemory(leafName, sizeof(leafName));

                //
                // Allocate space to hold the connection info for this port.
                // For now, allocate it big enough to hold info for 30 pipes.
                //
                // Endpoint numbers are 0-15.  Endpoint number 0 is the standard
                // control endpoint which is not explicitly listed in the Configuration
                // Descriptor.  There can be an IN endpoint and an OUT endpoint at
                // endpoint numbers 1-15 so there can be a maximum of 30 endpoints
                // per device configuration.
                //
                // Should probably size this dynamically at some point.
                //

                nBytesEx = SIZE_USB_NODE_CONNECTION_INFORMATION_EX +
                         ( (Marshal.SizeOf(typeof(USB_PIPE_INFO)) ) * 30);

                connectionInfoEx = new USB_NODE_CONNECTION_INFORMATION_EX();
                connectionInfoExV2 = new USB_NODE_CONNECTION_INFORMATION_EX_V2();

                int fuck = Marshal.SizeOf(portConnectorProps);

                // Now query USBHUB for the structures
                // for this port.  This will tell us if a device is attached to this
                // port, among other things.
                // The fault tolerate code is executed first.

                portConnectorProps.ConnectionIndex = index;

                success = Kernel32.DeviceIoControl(hHubDevice,
                                          Kernel32.IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES,
                                          &portConnectorProps,
                                          Marshal.SizeOf(typeof(USB_PORT_CONNECTOR_PROPERTIES)),
                                          &portConnectorProps,
                                          Marshal.SizeOf(portConnectorProps),
                                          &nBytes,
                                          IntPtr.Zero);

                int winErr = Marshal.GetLastWin32Error();

                if (success && nBytes == SIZE_USB_PORT_CONNECTOR_PROPERTIES) 
                {
                    byte[] portConnector2 = new byte[portConnectorProps.ActualLength];

                    // if (pPortConnectorProps != null)
                    fixed (byte* portConnectorPtr = portConnector2)
                    {
                        ( (USB_PORT_CONNECTOR_PROPERTIES*) portConnectorPtr )->ConnectionIndex = index;
                
                        success = Kernel32.DeviceIoControl(hHubDevice,
                                                  Kernel32.IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES,
                                                  portConnectorPtr,
                                                  portConnectorProps.ActualLength,
                                                  portConnectorPtr,
                                                  portConnectorProps.ActualLength,
                                                  &nBytes,
                                                  IntPtr.Zero);

                        if (!success || nBytes < portConnectorProps.ActualLength)
                        {
                            // FREE(pPortConnectorProps);
                            // pPortConnectorProps = null;
                        }
                        USB_PORT_CONNECTOR_PROPERTIES* pPortConnectorPtr = ( ( USB_PORT_CONNECTOR_PROPERTIES* ) portConnectorPtr );
                        pPortConnectorProps.ConnectionIndex = pPortConnectorPtr->ConnectionIndex;
                        pPortConnectorProps.ActualLength = pPortConnectorPtr->ActualLength;
                        pPortConnectorProps.Properties = pPortConnectorPtr->Properties;
                        pPortConnectorProps.CompanionIndex = pPortConnectorPtr->CompanionIndex;
                        pPortConnectorProps.CompanionPortNumber = pPortConnectorPtr->CompanionPortNumber;
                        pPortConnectorProps.__ptr__CompanionHubSymbolicLinkName = pPortConnectorPtr->__ptr__CompanionHubSymbolicLinkName;
                    }
                }

                connectionInfoExV2.ConnectionIndex = index;
                connectionInfoExV2.Length = ( uint ) Marshal.SizeOf(typeof(USB_NODE_CONNECTION_INFORMATION_EX_V2));
                connectionInfoExV2.SupportedUsbProtocols = USB_PROTOCOLS.Usb300;

                success = Kernel32.DeviceIoControl(hHubDevice,
                                          Kernel32.IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2,
                                          &connectionInfoExV2,
                                          Marshal.SizeOf(typeof(USB_NODE_CONNECTION_INFORMATION_EX_V2)),
                                          &connectionInfoExV2,
                                          Marshal.SizeOf(typeof(USB_NODE_CONNECTION_INFORMATION_EX_V2)),
                                          &nBytes,
                                          IntPtr.Zero);

                if (!success || nBytes < Marshal.SizeOf(typeof(USB_NODE_CONNECTION_INFORMATION_EX_V2)) ) 
                {
                    // FREE(connectionInfoExV2);
                    connectionInfoExV2HasValue = false;
                }

                connectionInfoEx.ConnectionIndex = index;

                success = Kernel32.DeviceIoControl(hHubDevice,
                                          Kernel32.IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX,
                                          ref connectionInfoEx,
                                          nBytesEx,
                                          out connectionInfoEx,
                                          nBytesEx,
                                          out nBytesEx,
                                          IntPtr.Zero);

                if (success)
                {
                    //
                    // Since the USB_NODE_CONNECTION_INFORMATION_EX is used to display
                    // the device speed, but the hub driver doesn't support indication
                    // of superspeed, we overwrite the value if the super speed
                    // data structures are available and indicate the device is operating
                    // at SuperSpeed.
                    // 
            
                    if (connectionInfoEx.Speed == USB_DEVICE_SPEED.UsbHighSpeed 
                        && connectionInfoExV2HasValue == true
                        && ((connectionInfoExV2.Flags & USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS.DeviceIsOperatingAtSuperSpeedOrHigher) == USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS.DeviceIsOperatingAtSuperSpeedOrHigher ||
                            (connectionInfoExV2.Flags & USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS.DeviceIsOperatingAtSuperSpeedPlusOrHigher) == USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS.DeviceIsOperatingAtSuperSpeedPlusOrHigher ) )
                    {
                        connectionInfoEx.Speed = USB_DEVICE_SPEED.UsbSuperSpeed;
                    }
                } 
                else 
                {
                    USB_NODE_CONNECTION_INFORMATION    connectionInfo = new USB_NODE_CONNECTION_INFORMATION();

                    // Try using IOCTL_USB_GET_NODE_CONNECTION_INFORMATION
                    // instead of IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX
                    //

                    nBytes = (SIZE_USB_NODE_CONNECTION_INFORMATION +
                             Marshal.SizeOf(typeof(USB_PIPE_INFO)) * 30 );

                    // connectionInfo = (PUSB_NODE_CONNECTION_INFORMATION)ALLOC(nBytes);

                    connectionInfo.ConnectionIndex = index;

                    success = Kernel32.DeviceIoControl(hHubDevice, 
                                              Kernel32.IOCTL_USB_GET_NODE_CONNECTION_INFORMATION, 
                                              &connectionInfo, 
                                              nBytes, 
                                              &connectionInfo, 
                                              nBytes, 
                                              &nBytes, 
                                              IntPtr.Zero); 

                    if (!success)
                    {
                        HandleNativeFailure();

                        // FREE(connectionInfo);
                        // FREE(connectionInfoEx);
                        // if (pPortConnectorProps != null)
                        // {
                        //     FREE(pPortConnectorProps);
                        // }
                        // if (connectionInfoExV2 != null )
                        // {
                        //     FREE(connectionInfoExV2);
                        // }
                        continue;
                    }

                    // Copy IOCTL_USB_GET_NODE_CONNECTION_INFORMATION into
                    // IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX structure.
                    //
                    connectionInfoEx.ConnectionIndex = connectionInfo.ConnectionIndex;
                    connectionInfoEx.DeviceDescriptor = connectionInfo.DeviceDescriptor;
                    connectionInfoEx.CurrentConfigurationValue = connectionInfo.CurrentConfigurationValue;
                    connectionInfoEx.Speed = connectionInfo.LowSpeed != 0 ? USB_DEVICE_SPEED.UsbLowSpeed : USB_DEVICE_SPEED.UsbFullSpeed;
                    connectionInfoEx.DeviceIsHub = connectionInfo.DeviceIsHub;
                    connectionInfoEx.DeviceAddress = connectionInfo.DeviceAddress;
                    connectionInfoEx.NumberOfOpenPipes = connectionInfo.NumberOfOpenPipes;
                    connectionInfoEx.ConnectionStatus = connectionInfo.ConnectionStatus;

                    // memcpy(&connectionInfoEx.PipeList[0],
                    //        &connectionInfo.PipeList[0],
                    //        sizeof(USB_PIPE_INFO) * 30);

                    // FREE(connectionInfo);
                }

                // Update the count of connected devices, take only connected devices which aren't usb hubs
                if (connectionInfoEx.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                {
                    if ( connectionInfoEx.DeviceIsHub == 0 ) {
                        s_totalPeripheralDevices++;
                    }
                }

                // If there is a device connected, get the Device Description
                //
                if (connectionInfoEx.ConnectionStatus != USB_CONNECTION_STATUS.NoDeviceConnected)
                {
                    driverKeyName = GetDriverKeyName(hHubDevice, index);

                    if (driverKeyName != null)
                    {
                        int cbDriverName = 0;

                        if ( driverKeyName.Length < MAX_DRIVER_KEY_NAME ) {
                            DevProps = DeviceNode.DriverNameToDeviceProperties(driverKeyName, cbDriverName);
                            pNode = FindMatchingDeviceNodeForDriverName(driverKeyName, connectionInfoEx.DeviceIsHub != 0);
                        }
                        // FREE(driverKeyName);
                    }

                }

                // If there is a device connected to the port, try to retrieve the
                // Configuration Descriptor from the device.
                //
                if (gDoConfigDesc &&
                    connectionInfoEx.ConnectionStatus == USB_CONNECTION_STATUS.DeviceConnected)
                {
                    configDesc = GetConfigDescriptor(hHubDevice,
                                                     index,
                                                     0);
                }
                else
                {
                    configDesc = null;
                }

                if (configDesc != null &&
                    connectionInfoEx.DeviceDescriptor.bcdUSB > 0x0200)
                {
                    bosDesc = GetBOSDescriptor(hHubDevice,
                                               index);
                }
                else
                {
                    bosDesc = null;
                }

                if ( configDesc.HasValue ) {

                    USB_CONFIGURATION_DESCRIPTOR cfgDesc = configDesc.Value;

                    if ( configDesc != null &&
                        AreThereStringDescriptors(connectionInfoEx.DeviceDescriptor,
                                                  ( USB_CONFIGURATION_DESCRIPTOR* ) ( &cfgDesc + 1 )) ) {
                        stringDescs = GetAllStringDescriptors(
                                          hHubDevice,
                                          index,
                                          connectionInfoEx.DeviceDescriptor,
                                          ( USB_CONFIGURATION_DESCRIPTOR* ) ( &cfgDesc + 1 ));
                    } else {
                        stringDescs = null;
                    }
                }

                USB_NODE_CONNECTION_INFORMATION_EX_V2? usbNodeInfoExV2 = null;
                if ( connectionInfoExV2HasValue )
                    usbNodeInfoExV2 = connectionInfoExV2;

                // If the device connected to the port is an external hub, get the
                // name of the external hub and recursively enumerate it.
                //
                if (connectionInfoEx.DeviceIsHub != 0)
                {
                    // PCHAR extHubName;
                    // size_t cbHubName = 0;

                    string extHubName = GetExternalHubName(hHubDevice, index);
                    if (extHubName != null)
                    {
                        // hr = StringCbLength(extHubName, MAX_DRIVER_KEY_NAME, &cbHubName);
                        // if (SUCCEEDED(hr))
                        {
                            EnumerateHub(hTreeParent, //hPortItem,
                                    extHubName,
                                    // cbHubName,
                                    connectionInfoEx,
                                    usbNodeInfoExV2,
                                    pPortConnectorProps,
                                    configDesc,
                                    bosDesc,
                                    stringDescs,
                                    DevProps);
                        }
                    }
                }
                else
                {
                    // Allocate some space for a USBDEVICEINFO structure to hold the
                    // hub info, hub name, and connection info pointers.  GPTR zero
                    // initializes the structure for us.
                    //
                    info = new USBDEVICEINFO();

                    // if (info == null)
                    // {
                    //     HandleNativeFailure();
                    //     if (configDesc != null)
                    //     {
                    //         FREE(configDesc);
                    //     }
                    //     if (bosDesc != null)
                    //     {
                    //         FREE(bosDesc);
                    //     }
                    //     FREE(connectionInfoEx);
                    // 
                    //     if (pPortConnectorProps != null)
                    //     {
                    //         FREE(pPortConnectorProps);
                    //     }
                    //     if (connectionInfoExV2 != null)
                    //     {
                    //         FREE(connectionInfoExV2);
                    //     }
                    //     break;
                    // }

                    info.DeviceInfoType = UsbDeviceInfoType.DeviceInfo;
                    info.ConnectionInfo = connectionInfoEx;
                    info.PortConnectorProps = pPortConnectorProps;
                    info.ConfigDesc = configDesc;
                    info.StringDescs = stringDescs;
                    info.BosDesc = bosDesc;
                    info.ConnectionInfoV2 = usbNodeInfoExV2;
                    info.UsbDeviceProperties = DevProps;
                    if ( info.UsbDeviceProperties != null ) {
                        info.UsbDeviceProperties.DriverKey = driverKeyName;
                        info.UsbDeviceProperties.DevicePath = devicePath;
                    }
                    info.DeviceInfoNode = pNode;

                    leafName = $"[Port{index}] {ConnectionStatuses[(int) info.ConnectionInfo.ConnectionStatus]}";
                    // StringCchPrintf(leafName, sizeof(leafName), "[Port%d] ", index);

                    // Add error description if ConnectionStatus is other than NoDeviceConnected / DeviceConnected
                    // StringCchCat(leafName, 
                    //     sizeof(leafName), 
                    //     ConnectionStatuses[connectionInfoEx->ConnectionStatus]);

                    if (DevProps != null)
                    {
                        // size_t cchDeviceDesc = 0;
                        // 
                        // hr = StringCbLength(DevProps->DeviceDesc, MAX_DEVICE_PROP, &cchDeviceDesc);
                        // if (FAILED(hr))
                        // {
                        //     HandleNativeFailure();
                        // }
                        leafName = leafName + " :  " + DeviceNameUtil.GetFriendlyUsbDeviceName(info);
                        ;

                        // dwSizeOfLeafName = sizeof(leafName);
                        // StringCchCatN(leafName, 
                        //     dwSizeOfLeafName - 1, 
                        //     " :  ",
                        //     sizeof(" :  "));
                        // StringCchCatN(leafName, 
                        //     dwSizeOfLeafName - 1, 
                        //     DevProps->DeviceDesc,
                        //     cchDeviceDesc );
                    }

                    bool isDeviceConnected = false;

                    if (connectionInfoEx.ConnectionStatus == USB_CONNECTION_STATUS.NoDeviceConnected )
                    {
                        if (connectionInfoExV2HasValue &&
                            ( connectionInfoExV2.SupportedUsbProtocols & USB_PROTOCOLS.Usb300 ) == USB_PROTOCOLS.Usb300 )
                        {
                            icon = UsbTreeIcon.NoSsDevice;
                            isDeviceConnected = false;
                        }
                        else
                        {
                            icon = UsbTreeIcon.NoDevice;
                            isDeviceConnected = false;
                        }
                    }
                    else if (connectionInfoEx.CurrentConfigurationValue != 0)
                    {
                        if (connectionInfoEx.Speed == USB_DEVICE_SPEED.UsbSuperSpeed )
                        {
                            icon = UsbTreeIcon.GoodSsDevice;
                            isDeviceConnected = true;
                        }
                        else
                        {
                            icon = UsbTreeIcon.GoodDevice;
                            isDeviceConnected = true;
                        }
                    }
                    else
                    {
                        icon = UsbTreeIcon.BadDevice;
                        isDeviceConnected = true;
                    }

                    MainWindow.Instance.AddLeaf(hTreeParent, info, leafName, icon, isDeviceConnected);
                }
            } // for
        }

        private static string GetExternalHubName(IntPtr Hub, uint ConnectionIndex) {
            bool                        success = false;
            int                         nBytes = 0;
            USB_NODE_CONNECTION_NAME    extHubName = new USB_NODE_CONNECTION_NAME();

            // Get the length of the name of the external hub attached to the
            // specified port.
            extHubName.ConnectionIndex = ConnectionIndex;

            nBytes = Marshal.SizeOf(extHubName);
            IntPtr ptrNodeName = Marshal.AllocHGlobal(nBytes);
            Marshal.StructureToPtr(extHubName, ptrNodeName, true);
            success = Kernel32.DeviceIoControl(Hub, Kernel32.IOCTL_USB_GET_NODE_CONNECTION_NAME, ptrNodeName, nBytes, ptrNodeName, nBytes, out nBytes, IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                Marshal.FreeHGlobal(ptrNodeName);
                return null;
            }

            extHubName = ( USB_NODE_CONNECTION_NAME ) Marshal.PtrToStructure(ptrNodeName, typeof(USB_NODE_CONNECTION_NAME));

            // free memory
            Marshal.FreeHGlobal(ptrNodeName);

            return extHubName.NodeName;
        }

        private static void EnumerateAllDevices() {
            EnumerateAllDevicesWithGuid(s_DeviceList, WinApiGuids.GUID_DEVINTERFACE_USB_DEVICE);
            EnumerateAllDevicesWithGuid(s_HubList, WinApiGuids.GUID_DEVINTERFACE_USB_HUB);
        }

        private static unsafe int GetHostControllerPowerMap(IntPtr hHCDev, ref UsbHostControllerInfo hcInfo) {
            USBUSER_POWER_INFO_REQUEST UsbPowerInfoRequest = new USBUSER_POWER_INFO_REQUEST();
            USB_POWER_INFO             pUPI = UsbPowerInfoRequest.PowerInformation ;
            int                        dwError = 0;
            int                        dwBytes = 0;
            bool                       bSuccess = false;
            int                        nIndex = 0;
            int                        nPowerState = (int)WDMUSB_POWER_STATE.SystemWorking;

            for ( ; nPowerState <= ( int ) WDMUSB_POWER_STATE.SystemShutdown; nIndex++, nPowerState++ ) {
                // set the header and request sizes
                UsbPowerInfoRequest.Header.UsbUserRequest = UsbApi.USBUSER_GET_POWER_STATE_MAP;
                UsbPowerInfoRequest.Header.RequestBufferLength = Marshal.SizeOf(UsbPowerInfoRequest);
                UsbPowerInfoRequest.PowerInformation.SystemState = (WDMUSB_POWER_STATE) nPowerState;

                // Now query USBHUB for the USB_POWER_INFO structure for this hub.
                // For Selective Suspend support
                // IntPtr UsbPowerInfoRequestPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USBUSER_POWER_INFO_REQUEST)));
                int sizeOfPowerRequest = Marshal.SizeOf(typeof(USBUSER_POWER_INFO_REQUEST));
                // Marshal.StructureToPtr(UsbPowerInfoRequest, UsbPowerInfoRequestPtr, false);
                // bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, UsbPowerInfoRequestPtr, sizeOfPowerRequest, UsbPowerInfoRequestPtr, sizeOfPowerRequest, out dwBytes, IntPtr.Zero);
                // UsbPowerInfoRequest = ( USBUSER_POWER_INFO_REQUEST ) Marshal.PtrToStructure(UsbPowerInfoRequestPtr, typeof(USBUSER_POWER_INFO_REQUEST));
                // Marshal.FreeHGlobal(UsbPowerInfoRequestPtr);
                // UsbPowerInfoRequestPtr = IntPtr.Zero;
                bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, &UsbPowerInfoRequest, sizeOfPowerRequest, &UsbPowerInfoRequest, sizeOfPowerRequest, &dwBytes, IntPtr.Zero);

                if ( !bSuccess ) {
                    dwError = Marshal.GetLastWin32Error();
                    HandleNativeFailure();
                } else {
                    // copy the data into our USB Host Controller's info structure
                    hcInfo.USBPowerInfo[nIndex] = UsbPowerInfoRequest.PowerInformation.Clone();
                }
            }

            return dwError;
        }

        private static int GetHostControllerInfo(IntPtr hHCDev, ref UsbHostControllerInfo hcInfo) {
            USBUSER_CONTROLLER_INFO_0 UsbControllerInfo = new USBUSER_CONTROLLER_INFO_0();
            int                        dwError = 0;
            int                        dwBytes = 0;
            bool                       bSuccess = false;

            // set the header and request sizes
            UsbControllerInfo.Header.UsbUserRequest = UsbApi.USBUSER_GET_CONTROLLER_INFO_0;
            UsbControllerInfo.Header.RequestBufferLength = Marshal.SizeOf(UsbControllerInfo);

            // Query for the USB_CONTROLLER_INFO_0 structure
            int sizeUsbControllerInfo = Marshal.SizeOf(typeof(USBUSER_CONTROLLER_INFO_0));
            // IntPtr UsbControllerInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USBUSER_CONTROLLER_INFO_0)));
            // Marshal.StructureToPtr(UsbControllerInfo, UsbControllerInfoPtr, false);
            // bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, UsbControllerInfoPtr, sizeUsbControllerInfo, UsbControllerInfoPtr, sizeUsbControllerInfo, out dwBytes, IntPtr.Zero);
            // UsbControllerInfo = ( USBUSER_CONTROLLER_INFO_0 ) Marshal.PtrToStructure(UsbControllerInfoPtr, typeof(USBUSER_CONTROLLER_INFO_0));
            // Marshal.FreeHGlobal(UsbControllerInfoPtr);
            // UsbControllerInfoPtr = IntPtr.Zero;
            bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, ref UsbControllerInfo, sizeUsbControllerInfo, out UsbControllerInfo, sizeUsbControllerInfo, out dwBytes, IntPtr.Zero);

            if ( !bSuccess ) {
                dwError = Marshal.GetLastWin32Error();
                HandleNativeFailure();
            } else {
                hcInfo.ControllerInfo = UsbControllerInfo.Info0.Clone();
            }
            return dwError;
        }

        private static int GetHostControllerBandwidth(IntPtr hHCDev, ref UsbHostControllerInfo hcInfo) {
            USBUSER_BANDWIDTH_INFO_REQUEST  UsbBandInfoRequest = new USBUSER_BANDWIDTH_INFO_REQUEST();
            int                             dwError = 0;
            int                             dwBytes = 0;
            bool                            bSuccess = false;

            // set the header and request sizes
            UsbBandInfoRequest.Header.UsbUserRequest = UsbApi.USBUSER_GET_BANDWIDTH_INFORMATION;
            UsbBandInfoRequest.Header.RequestBufferLength = Marshal.SizeOf(UsbBandInfoRequest);

            // Query for the USBUSER_BANDWIDTH_INFO_REQUEST structure
            int sizeUsbControllerInfo = Marshal.SizeOf(typeof(USBUSER_BANDWIDTH_INFO_REQUEST));
            IntPtr UsbControllerInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USBUSER_BANDWIDTH_INFO_REQUEST)));
            Marshal.StructureToPtr(UsbBandInfoRequest, UsbControllerInfoPtr, false);
            bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, UsbControllerInfoPtr, sizeUsbControllerInfo, UsbControllerInfoPtr, sizeUsbControllerInfo, out dwBytes, IntPtr.Zero);
            UsbBandInfoRequest = ( USBUSER_BANDWIDTH_INFO_REQUEST ) Marshal.PtrToStructure(UsbControllerInfoPtr, typeof(USBUSER_BANDWIDTH_INFO_REQUEST));
            Marshal.FreeHGlobal(UsbControllerInfoPtr);
            UsbControllerInfoPtr = IntPtr.Zero;

            if ( !bSuccess ) {
                dwError = Marshal.GetLastWin32Error();
                HandleNativeFailure();
            } else {
                hcInfo.BandwidthInfo = UsbBandInfoRequest.BandwidthInformation.Clone();
            }
            return dwError;
        }

        private static string GetRootHubNameThroughTreeStructure(string parentDevicePath) {

            string modifiedParentPath = parentDevicePath.Substring(4, parentDevicePath.IndexOf("#{") - 4).Replace('#', '\\').ToUpperInvariant();
            uint size = 1024;
            uint parentInst = uint.MaxValue;
            StringBuilder deviceIdBuffer = new StringBuilder((int)size);

            foreach ( DeviceInfoNode pEntry in s_HubList.ListHead ) {

                parentInst = uint.MaxValue;
                CfgMgr32.CR_RESULT result = CfgMgr32.CM_Get_Parent(out parentInst, pEntry.DeviceInfoData.DevInst, 0);
                if ( result != CfgMgr32.CR_RESULT.CR_SUCCESS ) {
                    HandleNativeFailure();
                    continue;
                }

                deviceIdBuffer.Clear();
                size = 1024;

                result = CfgMgr32.CM_Get_Device_ID(parentInst, deviceIdBuffer, ( int ) size);
                if ( result != CfgMgr32.CR_RESULT.CR_SUCCESS ) {
                    HandleNativeFailure();
                    continue;
                }
                string deviceId = deviceIdBuffer.ToString();

                if ( modifiedParentPath == deviceId.ToUpperInvariant() ) {
                    string devName = pEntry.DeviceDetailData.DevicePath.Replace("\\\\?\\", "");
                    int idx1 = devName.IndexOf('#');
                    int idx2 = devName.IndexOf('#', idx1 + 1);
                    string firstBit = devName.Substring(0, idx2).ToUpper();
                    string rest = devName.Substring(idx2);
                    return firstBit + rest;
                }
            }

            return null;
        }

        private static string GetRootHubName(IntPtr HostController) {
            bool                    success = false;
            int                     nBytes = 0;
            USB_ROOT_HUB_NAME       rootHubName = new USB_ROOT_HUB_NAME();
            USB_ROOT_HUB_NAME       rootHubNameW = new USB_ROOT_HUB_NAME();

            // Get the length of the name of the driver key of the HCD
            int rootHubNameSize = Marshal.SizeOf(typeof(USB_ROOT_HUB_NAME));
            IntPtr rootHubNamePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_ROOT_HUB_NAME)));
            Marshal.StructureToPtr(rootHubName, rootHubNamePtr, false);
            success = Kernel32.DeviceIoControl(HostController, Kernel32.IOCTL_USB_GET_ROOT_HUB_NAME, rootHubNamePtr, rootHubNameSize, rootHubNamePtr, rootHubNameSize, out nBytes, IntPtr.Zero);
            rootHubName = ( USB_ROOT_HUB_NAME ) Marshal.PtrToStructure(rootHubNamePtr, typeof(USB_ROOT_HUB_NAME));
            Marshal.FreeHGlobal(rootHubNamePtr);
            rootHubNamePtr = IntPtr.Zero;

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            // Allocate space to hold the driver key name
            nBytes = rootHubName.ActualLength;
            if ( nBytes <= 6 /* sizeof(rootHubName) */ ) {
                HandleNativeFailure();
                return null;
            }

            // Get the name of the driver key of the device attached to
            // the specified port.
            // IntPtr rootHubNameWPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_ROOT_HUB_NAME)));
            // Marshal.StructureToPtr(rootHubNameW, rootHubNameWPtr, false);
            // success = Kernel32.DeviceIoControl(HostController, Kernel32.IOCTL_USB_GET_ROOT_HUB_NAME, rootHubNameWPtr, nBytes, rootHubNameWPtr, nBytes, out nBytes, IntPtr.Zero);
            // rootHubNameW = ( USB_ROOT_HUB_NAME ) Marshal.PtrToStructure(rootHubNameWPtr, typeof(USB_ROOT_HUB_NAME));
            // Marshal.FreeHGlobal(rootHubNameWPtr);
            // rootHubNameWPtr = IntPtr.Zero;
            success = Kernel32.DeviceIoControl(HostController, Kernel32.IOCTL_USB_GET_ROOT_HUB_NAME, ref rootHubNameW, nBytes, out rootHubNameW, nBytes, out nBytes, IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            return rootHubNameW.RootHubName;
        }

        private static string GetHCDDriverKeyName(IntPtr HCD) {
            bool                    success = false;
            int                     nBytes = 0;
            USB_HCD_DRIVERKEY_NAME  driverKeyName = new USB_HCD_DRIVERKEY_NAME();
            USB_HCD_DRIVERKEY_NAME  driverKeyNameW = new USB_HCD_DRIVERKEY_NAME();

            // Get the length of the name of the driver key of the HCD
            int driverKeyNameSize = Marshal.SizeOf(typeof(USB_HCD_DRIVERKEY_NAME));
            // IntPtr driverKeyNamePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_HCD_DRIVERKEY_NAME)));
            // Marshal.StructureToPtr(driverKeyName, driverKeyNamePtr, false);
            // success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_GET_HCD_DRIVERKEY_NAME, driverKeyNamePtr, ( uint ) driverKeyNameSize, driverKeyNamePtr, ( uint ) driverKeyNameSize, out nBytes, IntPtr.Zero);
            // driverKeyName = (USB_HCD_DRIVERKEY_NAME) Marshal.PtrToStructure(driverKeyNamePtr, typeof(USB_HCD_DRIVERKEY_NAME));
            // Marshal.FreeHGlobal(driverKeyNamePtr);
            // driverKeyNamePtr = IntPtr.Zero;
            success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_GET_HCD_DRIVERKEY_NAME, ref driverKeyName, driverKeyNameSize, out driverKeyName, driverKeyNameSize, out nBytes, IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            // Allocate space to hold the driver key name
            nBytes = driverKeyName.ActualLength;
            if ( nBytes <= 6 /* sizeof(driverKeyName) */ ) {
                HandleNativeFailure();
                return null;
            }

            // Get the name of the driver key of the device attached to
            // the specified port.
            // IntPtr driverKeyNameWPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_HCD_DRIVERKEY_NAME)));
            // Marshal.StructureToPtr(driverKeyNameW, driverKeyNameWPtr, false);
            // success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_GET_HCD_DRIVERKEY_NAME, driverKeyNameWPtr, nBytes, driverKeyNameWPtr, nBytes, out nBytes, IntPtr.Zero);
            // driverKeyNameW = ( USB_HCD_DRIVERKEY_NAME ) Marshal.PtrToStructure(driverKeyNameWPtr, typeof(USB_HCD_DRIVERKEY_NAME));
            // Marshal.FreeHGlobal(driverKeyNameWPtr);
            // driverKeyNameWPtr = IntPtr.Zero;
            success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_GET_HCD_DRIVERKEY_NAME, ref driverKeyNameW, nBytes, out driverKeyNameW, nBytes, out nBytes, IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            return driverKeyNameW.DriverKeyName;
        }

        private static unsafe string GetDriverKeyName(IntPtr HCD, uint connectionIndex) {
            bool                    success = false;
            int                     nBytes = 0;
            int                     nBytesReturned = 0;

            Logger.Info("Getting driver keyname...");

            USB_NODE_CONNECTION_DRIVERKEY_NAME driverKeyName = new USB_NODE_CONNECTION_DRIVERKEY_NAME() {
                ConnectionIndex = connectionIndex,
            };
            nBytes = 10;
            // IntPtr ptrDriverKey = Marshal.AllocHGlobal(nBytes);
            // Marshal.StructureToPtr(driverKeyName, ptrDriverKey, true);
            // success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME, ptrDriverKey, nBytes, ptrDriverKey, nBytes, out nBytesReturned, IntPtr.Zero);
            success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME, &driverKeyName, nBytes, &driverKeyName, nBytes, &nBytesReturned, IntPtr.Zero);
            
            // handle failure
            if ( !success ) {
                Logger.Fatal($"HCD: {HCD}, connectionIndex: {connectionIndex}");
                HandleNativeFailure();
                return null;
            }

            nBytes = driverKeyName.ActualLength;
            driverKeyName = new USB_NODE_CONNECTION_DRIVERKEY_NAME() {
                ConnectionIndex = connectionIndex,
            };
            IntPtr ptrDriverKey = Marshal.AllocHGlobal(nBytes);
            Marshal.StructureToPtr(driverKeyName, ptrDriverKey, true);
            success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME, ptrDriverKey, nBytes, ptrDriverKey, nBytes, out nBytesReturned, IntPtr.Zero);
            // success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME, &driverKeyName, nBytes, &driverKeyName, nBytes, &nBytesReturned, IntPtr.Zero);

            // handle failure
            if ( !success ) {
                Logger.Fatal($"HCD: {HCD}, connectionIndex: {connectionIndex}");
                HandleNativeFailure();
                return null;
            }

            var driverKeyNameDecoded = ( USB_NODE_CONNECTION_DRIVERKEY_NAME_STRING ) Marshal.PtrToStructure(ptrDriverKey, typeof(USB_NODE_CONNECTION_DRIVERKEY_NAME_STRING));
            return driverKeyNameDecoded.DriverKeyName;
            // return Marshal.PtrToStringAuto(driverKeyName.DriverKeyNamePtr);
        }

        // specialisation for strings
        /*
        public static bool GetDevicePropertyString(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, DevRegProperty Property, out string ppBuffer) {
            bool bResult;
            uint requiredLength = 0;
            int lastError;


            bResult = SetupApi.SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out _, IntPtr.Zero, 0, out requiredLength);
            lastError = Marshal.GetLastWin32Error();

            // Property does not exist
            if ( lastError == Kernel32.ERROR_INVALID_DATA ) {
                ppBuffer = "";
                return false;
            }

            if ( ( requiredLength == 0 ) || ( bResult != false && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = "";
                HandleNativeFailure();
                return false;
            }


            StringBuilder ppBufferStringBuilder = new StringBuilder( ( int ) requiredLength );
            bResult = SetupApi.SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out _, ppBufferStringBuilder, requiredLength, out requiredLength);

            if ( bResult == false ) {
                ppBuffer = "";
                HandleNativeFailure();
                return false;
            }

            ppBuffer = ppBufferStringBuilder.ToString();

            return true;
        }
        */
        public static bool GetDevicePropertyString(uint devInst, DEVPROPKEY Property, out string ppBuffer) {
            CR_RESULT bResult;
            uint requiredLength = 0;
            DEVPROPTYPE PropertyType;
            int lastError;

            bResult = CfgMgr32.CM_Get_DevNode_Property(devInst, Property, out PropertyType, IntPtr.Zero, ref requiredLength);
            lastError = Marshal.GetLastWin32Error();

            // Property does not exist
            if ( bResult == CR_RESULT.CR_NO_SUCH_VALUE || lastError == Kernel32.ERROR_INVALID_DATA ) {
                ppBuffer = "";
                return false;
            }

            if ( ( requiredLength == 0 ) || ( bResult != CR_RESULT.CR_BUFFER_SMALL && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = "";
                HandleNativeFailure();
                return false;
            }


            StringBuilder ppBufferStringBuilder = new StringBuilder( ( int ) requiredLength );
            bResult = CfgMgr32.CM_Get_DevNode_Property(devInst, Property, out PropertyType, ppBufferStringBuilder, ref requiredLength);

            if ( bResult != CR_RESULT.CR_SUCCESS ) {
                ppBuffer = "";
                HandleNativeFailure();
                return false;
            }

            ppBuffer = ppBufferStringBuilder.ToString();

            return true;
        }
        public static bool GetDevicePropertyFileTime(uint devInst, DEVPROPKEY Property, out FILETIME ppBuffer) {
            CR_RESULT bResult;
            uint requiredLength = 0;
            DEVPROPTYPE PropertyType;
            int lastError;

            bResult = CfgMgr32.CM_Get_DevNode_Property(devInst, Property, out PropertyType, IntPtr.Zero, ref requiredLength);
            lastError = Marshal.GetLastWin32Error();

            // Property does not exist
            if ( bResult == CR_RESULT.CR_NO_SUCH_VALUE || lastError == Kernel32.ERROR_INVALID_DATA ) {
                ppBuffer = new FILETIME();
                return false;
            }

            if ( ( requiredLength == 0 ) || ( bResult != CR_RESULT.CR_BUFFER_SMALL && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = new FILETIME();
                HandleNativeFailure();
                return false;
            }

            int size = Marshal.SizeOf(typeof(FILETIME));
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.AllocHGlobal(size);
                bResult = CfgMgr32.CM_Get_DevNode_Property(devInst, Property, out PropertyType, ptr, ref requiredLength);
                ppBuffer = ( FILETIME ) Marshal.PtrToStructure(ptr, typeof(FILETIME));
            } finally {
                Marshal.FreeHGlobal(ptr);
            }

            if ( bResult != CR_RESULT.CR_SUCCESS ) {
                ppBuffer = new FILETIME();
                HandleNativeFailure();
                return false;
            }

            return true;
        }
        public static bool GetDevicePropertyString(uint devInst, CM_DRP Property, out string ppBuffer) {
            CR_RESULT bResult;
            uint requiredLength = 0;
            REG_VALUE_TYPE pulRegDataType;
            int lastError;

            //                 CM_Get_DevNode_Registry_Property(uint dnDevInst, CM_DRP ulProperty, out REG_VALUE_TYPE pulRegDataType, [Out, Optional] IntPtr Buffer, ref uint pulLength, uint ulFlags = 0);
            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, IntPtr.Zero, ref requiredLength);
            lastError = Marshal.GetLastWin32Error();

            // Property does not exist
            if ( bResult == CR_RESULT.CR_NO_SUCH_VALUE || lastError == Kernel32.ERROR_INVALID_DATA ) {
                ppBuffer = "";
                return false;
            }

            if ( ( requiredLength == 0 ) || ( bResult != CR_RESULT.CR_BUFFER_SMALL && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = "";
                HandleNativeFailure();
                return false;
            }


            StringBuilder ppBufferStringBuilder = new StringBuilder( ( int ) requiredLength );
            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, ppBufferStringBuilder, ref requiredLength);

            if ( bResult != CR_RESULT.CR_SUCCESS ) {
                ppBuffer = "";
                HandleNativeFailure();
                return false;
            }

            ppBuffer = ppBufferStringBuilder.ToString();

            return true;
        }

        public static bool GetDevicePropertyStringList(uint devInst, CM_DRP Property, out string[] ppBuffer) {
            CR_RESULT bResult;
            uint requiredLength = 0;
            REG_VALUE_TYPE pulRegDataType;
            int lastError;

            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, IntPtr.Zero, ref requiredLength);
            lastError = Marshal.GetLastWin32Error();

            // Property does not exist
            if ( bResult == CR_RESULT.CR_NO_SUCH_VALUE || lastError == Kernel32.ERROR_INVALID_DATA ) {
                ppBuffer = new string[] { };
                return false;
            }

            if ( ( requiredLength == 0 ) || ( bResult != CR_RESULT.CR_BUFFER_SMALL && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = new string[] { };
                HandleNativeFailure();
                return false;
            }

            IntPtr buffer = Marshal.AllocHGlobal( ( int ) requiredLength);
            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, buffer, ref requiredLength);
            string ret = Marshal.PtrToStringUni(buffer, (int)requiredLength/2);
            Marshal.FreeHGlobal(buffer);

            if ( bResult != CR_RESULT.CR_SUCCESS ) {
                ppBuffer = new string[] { };
                HandleNativeFailure();
                return false;
            }

            ppBuffer = ret.Substring(0, ret.Length - 2).Split('\0');

            return true;
        }

        public static bool GetDevicePropertyStruct<T>(uint devInst, CM_DRP Property, out T ppBuffer) where T : struct {
            CR_RESULT bResult;
            uint requiredLength = 0;
            REG_VALUE_TYPE pulRegDataType;
            int lastError;

            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, IntPtr.Zero, ref requiredLength);
            lastError = Marshal.GetLastWin32Error();

            if ( ( requiredLength == 0 ) || ( bResult != CR_RESULT.CR_BUFFER_SMALL && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = new T();
                HandleNativeFailure();
                return false;
            }

            ppBuffer = new T();

            // memory safety yippee
            byte[] dataBuffer = new byte[Marshal.SizeOf(typeof(T))];
            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, dataBuffer, ref requiredLength);
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = IntPtr.Zero;
            try {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(dataBuffer, 0, ptr, size);
                ppBuffer = ( T ) Marshal.PtrToStructure(ptr, ppBuffer.GetType());
            } finally {
                Marshal.FreeHGlobal(ptr);
            }

            if ( bResult != CR_RESULT.CR_SUCCESS ) {
                ppBuffer = new T();
                HandleNativeFailure();
                return false;
            }

            return true;
        }

        public static bool GetDevicePropertyInt32(uint devInst, CM_DRP Property, out int ppBuffer) {
            CR_RESULT bResult;
            uint requiredLength = 0;
            REG_VALUE_TYPE pulRegDataType;
            int lastError;

            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, IntPtr.Zero, ref requiredLength);
            lastError = Marshal.GetLastWin32Error();

            if ( ( requiredLength == 0 ) || ( bResult != CR_RESULT.CR_BUFFER_SMALL && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = -1;
                HandleNativeFailure();
                return false;
            }

            ppBuffer = 0;

            // work around for enums
            // bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, ppBuffer, ref requiredLength);
            byte[] dataBuffer = new byte[requiredLength];
            bResult = CfgMgr32.CM_Get_DevNode_Registry_Property(devInst, Property, out pulRegDataType, dataBuffer, ref requiredLength);

            ppBuffer = BitConverter.ToInt32(dataBuffer, 0);

            if ( bResult != CR_RESULT.CR_SUCCESS ) {
                ppBuffer = -1;
                HandleNativeFailure();
                return false;
            }

            return true;
        }

        private static void EnumerateAllDevicesWithGuid(DeviceGuidList DeviceList, Guid Guid) {
            if ( DeviceList.DeviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {

                if ( DeviceList.ListHead.First.Value.DeviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {
                    SetupApi.SetupDiDestroyDeviceInfoList(DeviceList.DeviceInfo);
                }

                DeviceList.ListHead.Clear();
            }

            DeviceList.DeviceInfo = SetupApi.SetupDiGetClassDevs(ref Guid, null, IntPtr.Zero, ( DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE ));

            if ( DeviceList.DeviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {
                uint index;
                int error;

                error = 0;
                index = 0;

                while ( error != Kernel32.ERROR_NO_MORE_ITEMS ) {
                    bool success;
                    DeviceInfoNode pNode;

                    pNode = new DeviceInfoNode();
                    // if ( pNode == null ) {
                    //     HandleNativeFailure();
                    //     break;
                    // }
                    pNode.DeviceInfo = DeviceList.DeviceInfo;
                    pNode.DeviceInterfaceData.cbSize = Marshal.SizeOf(pNode.DeviceInterfaceData);
                    pNode.DeviceInfoData.cbSize = Marshal.SizeOf(pNode.DeviceInfoData);

                    success = SetupApi.SetupDiEnumDeviceInfo(DeviceList.DeviceInfo, index, ref pNode.DeviceInfoData);

                    index++;

                    if ( success == false ) {
                        error = Marshal.GetLastWin32Error();

                        if ( error != Kernel32.ERROR_NO_MORE_ITEMS ) {
                            HandleNativeFailure();
                        }
                    } else {
                        bool   bResult;
                        uint  requiredLength = 0;

                        bResult = GetDevicePropertyString(pNode.DeviceInfoData.DevInst, CM_DRP.CM_DRP_DEVICEDESC, out pNode.DeviceDescName);
                        if ( bResult == false ) {
                            HandleNativeFailure();
                            break;
                        }

                        bResult = GetDevicePropertyString(pNode.DeviceInfoData.DevInst, CM_DRP.CM_DRP_DRIVER, out pNode.DeviceDriverName);
                        if ( bResult == false ) {
                            HandleNativeFailure();
                            break;
                        }

                        success = SetupApi.SetupDiEnumDeviceInterfaces(DeviceList.DeviceInfo, IntPtr.Zero, ref Guid, index - 1, ref pNode.DeviceInterfaceData);
                        error = Marshal.GetLastWin32Error();
                        if ( error == Kernel32.ERROR_NO_MORE_ITEMS ) {
                            break;
                        }
                        if ( !success ) {
                            HandleNativeFailure();
                            break;
                        }

                        success = SetupApi.SetupDiGetDeviceInterfaceDetail(DeviceList.DeviceInfo, ref pNode.DeviceInterfaceData, IntPtr.Zero, 0, ref requiredLength, IntPtr.Zero);
                        error = Marshal.GetLastWin32Error();
                        if ( !success && error != Kernel32.ERROR_INSUFFICIENT_BUFFER ) {
                            HandleNativeFailure();
                            break;
                        }

                        pNode.DeviceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                        pNode.DeviceDetailData.cbSize = 8; /* sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA) */

                        success = SetupApi.SetupDiGetDeviceInterfaceDetail(DeviceList.DeviceInfo, ref pNode.DeviceInterfaceData, ref pNode.DeviceDetailData, requiredLength, ref requiredLength, IntPtr.Zero);
                        if ( !success ) {
                            HandleNativeFailure();
                            break;
                        }

                        DeviceList.ListHead.AddLast(pNode);
                    }
                }
            }
        }

        private static unsafe USB_CONFIGURATION_DESCRIPTOR? GetConfigDescriptor(
            IntPtr hHubDevice,
            uint ConnectionIndex,
            byte DescriptorIndex
        ) {

            USB_CONFIGURATION_DESCRIPTOR? configDesc =  GetUsbDescriptor<USB_CONFIGURATION_DESCRIPTOR>(
                hHubDevice,
                ConnectionIndex,
                ( ushort ) ( ( ( int ) USB_DESCRIPTOR_TYPE.USB_CONFIGURATION_DESCRIPTOR_TYPE << 8 ) | DescriptorIndex ),
                0);

            return configDesc;
#if false
            bool    success = false;
            int     nBytes = 0;
            int     nBytesReturned = 0;

            byte[]   configDescReqBuf = new byte[Marshal.SizeOf(typeof(USB_DESCRIPTOR_REQUEST)) + Marshal.SizeOf(typeof(USB_CONFIGURATION_DESCRIPTOR))];

            USB_DESCRIPTOR_REQUEST*        configDescReq = null;
            USB_CONFIGURATION_DESCRIPTOR*  configDesc = null;

            fixed ( byte* configDescReqBufPtr = configDescReqBuf ) {
                configDescReq = ( USB_DESCRIPTOR_REQUEST* ) &configDescReqBufPtr;
                configDesc = ( USB_CONFIGURATION_DESCRIPTOR* ) ( &configDescReqBufPtr + SIZE_USB_DESCRIPTOR_REQUEST );

                // Request the Configuration Descriptor the first time using our
                // local buffer, which is just big enough for the Cofiguration
                // Descriptor itself.
                nBytes = configDescReqBuf.Length;

                // configDescReq = ( USB_DESCRIPTOR_REQUEST ) configDescReqBuf;
                // configDesc = ( USB_CONFIGURATION_DESCRIPTOR ) ( configDescReq + 1 );

                // Zero fill the entire request structure
                //
                // memset(configDescReq, 0, nBytes);

                // Indicate the port from which the descriptor will be requested
                //
                configDescReq->ConnectionIndex = ConnectionIndex;

                //
                // USBHUB uses URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE to process this
                // IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION request.
                //
                // USBD will automatically initialize these fields:
                //     bmRequest = 0x80
                //     bRequest  = 0x06
                //
                // We must inititialize these fields:
                //     wValue    = Descriptor Type (high) and Descriptor Index (low byte)
                //     wIndex    = Zero (or Language ID for String Descriptors)
                //     wLength   = Length of descriptor buffer
                //
                configDescReq->SetupPacket.wValue = ( ushort ) ( ( ( int ) USB_DESCRIPTOR_TYPE.USB_CONFIGURATION_DESCRIPTOR_TYPE << 8 ) | DescriptorIndex );

                configDescReq->SetupPacket.wLength = ( ushort ) ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST );
            }

            // Now issue the get descriptor request.
            //
            success = Kernel32.DeviceIoControl(hHubDevice,
                                      Kernel32.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      &configDescReqBuf,
                                      nBytes,
                                      &configDescReqBuf,
                                      nBytes,
                                      &nBytesReturned,
                                      IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            if ( nBytes != nBytesReturned ) {
                HandleNativeFailure();
                return null;
            }

            if ( configDesc->wTotalLength < SIZE_USB_DESCRIPTOR_REQUEST ) {
                HandleNativeFailure();
                return null;
            }

            // Now request the entire Configuration Descriptor using a dynamically
            // allocated buffer which is sized big enough to hold the entire descriptor
            //
            nBytes = ( SIZE_USB_DESCRIPTOR_REQUEST + configDesc->wTotalLength );

            // Reallocate buffer
            configDescReqBuf = new byte[nBytes];

            fixed ( byte* configDescReqBufPtr = configDescReqBuf ) {
                configDescReq = ( USB_DESCRIPTOR_REQUEST* ) &configDescReqBufPtr;
                configDesc = ( USB_CONFIGURATION_DESCRIPTOR* ) ( &configDescReqBufPtr + SIZE_USB_DESCRIPTOR_REQUEST );

                // if ( configDescReq == null ) {
                //     HandleNativeFailure();
                //     return null;
                // }

                // Indicate the port from which the descriptor will be requested
                //
                configDescReq->ConnectionIndex = ConnectionIndex;

                configDesc = ( USB_CONFIGURATION_DESCRIPTOR* ) ( &configDescReq + SIZE_USB_DESCRIPTOR_REQUEST );

                //
                // USBHUB uses URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE to process this
                // IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION request.
                //
                // USBD will automatically initialize these fields:
                //     bmRequest = 0x80
                //     bRequest  = 0x06
                //
                // We must inititialize these fields:
                //     wValue    = Descriptor Type (high) and Descriptor Index (low byte)
                //     wIndex    = Zero (or Language ID for String Descriptors)
                //     wLength   = Length of descriptor buffer
                //
                configDescReq->SetupPacket.wValue = ( ushort ) ( ( ( int ) USB_DESCRIPTOR_TYPE.USB_CONFIGURATION_DESCRIPTOR_TYPE << 8 ) | DescriptorIndex );

                configDescReq->SetupPacket.wLength = ( ushort ) ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST );
            }

            // Now issue the get descriptor request.
            //

            success = Kernel32.DeviceIoControl(hHubDevice,
                                      Kernel32.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      &configDescReqBuf,
                                      nBytes,
                                      &configDescReqBuf,
                                      nBytes,
                                      &nBytesReturned,
                                      IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                // FREE(configDescReq);
                return null;
            }

            if ( nBytes != nBytesReturned ) {
                HandleNativeFailure();
                // FREE(configDescReq);
                return null;
            }

            if ( configDesc->wTotalLength != ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST ) ) {
                HandleNativeFailure();
                // FREE(configDescReq);
                return null;
            }

            USB_DESCRIPTOR_REQUEST outRequest = new USB_DESCRIPTOR_REQUEST();
            outRequest.ConnectionIndex  = configDescReq->ConnectionIndex;
            outRequest.SetupPacket      = configDescReq->SetupPacket;
            outRequest.Data             = configDescReq->Data;


            return outRequest;
#endif
        }



        //*****************************************************************************
        //
        // GetBOSDescriptor()
        //
        // hHubDevice - Handle of the hub device containing the port from which the
        // Configuration Descriptor will be requested.
        //
        // ConnectionIndex - Identifies the port on the hub to which a device is
        // attached from which the BOS Descriptor will be requested.
        //
        //*****************************************************************************

        private static unsafe USB_BOS_DESCRIPTOR? GetBOSDescriptor( IntPtr hHubDevice, uint ConnectionIndex ) {


            USB_BOS_DESCRIPTOR? bosDesc =  GetUsbDescriptor<USB_BOS_DESCRIPTOR>(
                hHubDevice,
                ConnectionIndex,
                (int) USB_DESCRIPTOR_TYPE.USB_BOS_DESCRIPTOR_TYPE << 8 ,
                0);

            return bosDesc;

#if false
            bool    success = false;
            int     nBytes = 0;
            int     nBytesReturned = 0;

            USB_DESCRIPTOR_REQUEST bosDescReq = new USB_DESCRIPTOR_REQUEST();
            USB_BOS_DESCRIPTOR*     bosDesc = null;


            // Request the BOS Descriptor the first time using our
            // local buffer, which is just big enough for the BOS
            // Descriptor itself.
            //
            // nBytes = sizeof(bosDescReqBuf);

            // bosDescReq = ( USB_DESCRIPTOR_REQUEST ) bosDescReqBuf;
            bosDesc = ( USB_BOS_DESCRIPTOR* ) ( &bosDescReq + SIZE_USB_DESCRIPTOR_REQUEST );

            // Zero fill the entire request structure
            //
            // memset(bosDescReq, 0, nBytes);

            // Indicate the port from which the descriptor will be requested
            //
            bosDescReq.ConnectionIndex = ConnectionIndex;

            //
            // USBHUB uses URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE to process this
            // IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION request.
            //
            // USBD will automatically initialize these fields:
            //     bmRequest = 0x80
            //     bRequest  = 0x06
            //
            // We must inititialize these fields:
            //     wValue    = Descriptor Type (high) and Descriptor Index (low byte)
            //     wIndex    = Zero (or Language ID for String Descriptors)
            //     wLength   = Length of descriptor buffer
            //
            bosDescReq.SetupPacket.wValue = ( (int) USB_DESCRIPTOR_TYPE.USB_BOS_DESCRIPTOR_TYPE << 8 );

            bosDescReq.SetupPacket.wLength = ( ushort ) ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST );

            // Now issue the get descriptor request.
            //
            success = Kernel32.DeviceIoControl(hHubDevice,
                                      Kernel32.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      ref bosDescReq,
                                      nBytes,
                                      out bosDescReq,
                                      nBytes,
                                      out nBytesReturned,
                                      IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            if ( nBytes != nBytesReturned ) {
                HandleNativeFailure();
                return null;
            }

            if ( bosDesc->wTotalLength < sizeof(USB_BOS_DESCRIPTOR) ) {
                HandleNativeFailure();
                return null;
            }

            // Now request the entire BOS Descriptor using a dynamically
            // allocated buffer which is sized big enough to hold the entire descriptor
            //
            nBytes = ( SIZE_USB_DESCRIPTOR_REQUEST + bosDesc->wTotalLength );

            bosDescReq = new USB_DESCRIPTOR_REQUEST();

            // if ( bosDescReq == null ) {
            //     HandleNativeFailure();
            //     return null;
            // }

            bosDesc = ( USB_BOS_DESCRIPTOR* ) ( &bosDescReq + SIZE_USB_DESCRIPTOR_REQUEST );

            // Indicate the port from which the descriptor will be requested
            //
            bosDescReq.ConnectionIndex = ConnectionIndex;

            //
            // USBHUB uses URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE to process this
            // IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION request.
            //
            // USBD will automatically initialize these fields:
            //     bmRequest = 0x80
            //     bRequest  = 0x06
            //
            // We must inititialize these fields:
            //     wValue    = Descriptor Type (high) and Descriptor Index (low byte)
            //     wIndex    = Zero (or Language ID for String Descriptors)
            //     wLength   = Length of descriptor buffer
            //
            bosDescReq.SetupPacket.wValue = ( (int) USB_DESCRIPTOR_TYPE.USB_BOS_DESCRIPTOR_TYPE << 8 );

            bosDescReq.SetupPacket.wLength = ( ushort ) ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST );

            // Now issue the get descriptor request.
            //

            success = Kernel32.DeviceIoControl(hHubDevice,
                                      Kernel32.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      ref bosDescReq,
                                      nBytes,
                                      out bosDescReq,
                                      nBytes,
                                      out nBytesReturned,
                                      IntPtr.Zero);

            if ( !success ) {
                HandleNativeFailure();
                // FREE(bosDescReq);
                return null;
            }

            if ( nBytes != nBytesReturned ) {
                HandleNativeFailure();
                // FREE(bosDescReq);
                return null;
            }

            if ( bosDesc->wTotalLength != ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST ) ) {
                HandleNativeFailure();
                // FREE(bosDescReq);
                return null;
            }

            return bosDescReq;
#endif
        }

        //*****************************************************************************
        //
        // AreThereStringDescriptors()
        //
        // DeviceDesc - Device Descriptor for which String Descriptors should be
        // checked.
        //
        // ConfigDesc - Configuration Descriptor (also containing Interface Descriptor)
        // for which String Descriptors should be checked.
        //
        //*****************************************************************************

        private static unsafe bool AreThereStringDescriptors(
            USB_DEVICE_DESCRIPTOR DeviceDesc,
            USB_CONFIGURATION_DESCRIPTOR* ConfigDesc
        ) {
            void*                  descEnd = null;
            USB_COMMON_DESCRIPTOR* commonDesc = null;

            // Check Device Descriptor strings
            if ( DeviceDesc.iManufacturer != 0 || DeviceDesc.iProduct != 0|| DeviceDesc.iSerialNumber != 0 ) {
                return true;
            }


            // Check the Configuration and Interface Descriptor strings

            descEnd = &ConfigDesc + ConfigDesc->wTotalLength;

            commonDesc = ( USB_COMMON_DESCRIPTOR* ) ConfigDesc;

            while ( ( byte* ) commonDesc + sizeof(USB_COMMON_DESCRIPTOR) < descEnd &&
                   ( byte* ) commonDesc + commonDesc->bLength <= descEnd ) {
                switch ( commonDesc->bDescriptorType ) {
                    case USB_DESCRIPTOR_TYPE.USB_CONFIGURATION_DESCRIPTOR_TYPE:
                    case USB_DESCRIPTOR_TYPE.USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE:
                        if ( commonDesc->bLength != sizeof(USB_CONFIGURATION_DESCRIPTOR) ) {
                            HandleNativeFailure();
                            break;
                        }
                        if ( ( ( USB_CONFIGURATION_DESCRIPTOR* ) commonDesc )->iConfiguration != 0 ) {
                            return true;
                        }
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;

                    case USB_DESCRIPTOR_TYPE.USB_INTERFACE_DESCRIPTOR_TYPE:
                        if ( commonDesc->bLength != sizeof(USB_INTERFACE_DESCRIPTOR) &&
                            commonDesc->bLength != sizeof(USB_INTERFACE_DESCRIPTOR2) ) {
                            HandleNativeFailure();
                            break;
                        }
                        if ( ( ( USB_INTERFACE_DESCRIPTOR* ) commonDesc )->iInterface != 0 ) {
                            return true;
                        }
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;

                    default:
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;
                }
                break;
            }

            return false;
        }


        //*****************************************************************************
        //
        // GetAllStringDescriptors()
        //
        // hHubDevice - Handle of the hub device containing the port from which the
        // String Descriptors will be requested.
        //
        // ConnectionIndex - Identifies the port on the hub to which a device is
        // attached from which the String Descriptors will be requested.
        //
        // DeviceDesc - Device Descriptor for which String Descriptors should be
        // requested.
        //
        // ConfigDesc - Configuration Descriptor (also containing Interface Descriptor)
        // for which String Descriptors should be requested.
        //
        //*****************************************************************************

        private static unsafe StringDescriptorsCollection GetAllStringDescriptors(
            IntPtr hHubDevice,
            uint ConnectionIndex,
            USB_DEVICE_DESCRIPTOR DeviceDesc,
            USB_CONFIGURATION_DESCRIPTOR* ConfigDesc
        ) {
            StringDescriptorsCollection stringDescriptors = new StringDescriptorsCollection() {
                Strings = new List<StringDescriptorNode>()
            };
            USB_STRING_DESCRIPTOR_LANG supportedLanguagesString = new USB_STRING_DESCRIPTOR_LANG();
            uint                        numLanguageIDs = 0;
            short[]                     languageIDs = null;

            byte*                       descEnd = null;
            USB_COMMON_DESCRIPTOR*      commonDesc = null;
            byte                        uIndex = 1;
            byte                        bInterfaceClass = 0;
            bool                        getMoreStrings = false;
            // HRESULT                     hr = S_OK;
            bool                        hr = true;

            //
            // Get the array of supported Language IDs, which is returned
            // in String Descriptor 0
            //
            USB_STRING_DESCRIPTOR_LANG? langDesc = GetStringLanguagesDescriptor(hHubDevice,
                                                           ConnectionIndex,
                                                           0,
                                                           0);

            if ( langDesc == null ) {
                return null;
            }

            supportedLanguagesString = langDesc.Value;

            numLanguageIDs = (uint) (( supportedLanguagesString.bLength - 2 ) / 2);

            languageIDs = new short[numLanguageIDs];
            short[] tmpLangs = new short[numLanguageIDs];
            Marshal.Copy((IntPtr) supportedLanguagesString.bString, tmpLangs, 0, ( int ) numLanguageIDs);
            Buffer.BlockCopy(tmpLangs, 0, languageIDs, 0, (int)numLanguageIDs);

            // Prepare for string descriptors
            stringDescriptors.Lang_bLength = supportedLanguagesString.bLength;
            stringDescriptors.Lang_bDescriptorType = supportedLanguagesString.bDescriptorType;
            stringDescriptors.LanguageIds = new short[numLanguageIDs];
            Marshal.Copy(( IntPtr ) supportedLanguagesString.bString, stringDescriptors.LanguageIds, 0, ( int ) numLanguageIDs);

            // Get the Device Descriptor strings

            if ( DeviceDesc.iManufacturer != 0) {
                GetStringDescriptors(hHubDevice,
                                     ConnectionIndex,
                                     DeviceDesc.iManufacturer,
                                     numLanguageIDs,
                                     languageIDs,
                                     stringDescriptors);
            }

            if ( DeviceDesc.iProduct != 0) {
                GetStringDescriptors(hHubDevice,
                                     ConnectionIndex,
                                     DeviceDesc.iProduct,
                                     numLanguageIDs,
                                     languageIDs,
                                     stringDescriptors);
            }

            if ( DeviceDesc.iSerialNumber != 0 ) {
                GetStringDescriptors(hHubDevice,
                                     ConnectionIndex,
                                     DeviceDesc.iSerialNumber,
                                     numLanguageIDs,
                                     languageIDs,
                                     stringDescriptors);
            }

            // Get the Configuration and Interface Descriptor strings

            descEnd = ( byte* ) ConfigDesc + ConfigDesc->wTotalLength;

            commonDesc = ( USB_COMMON_DESCRIPTOR* ) ConfigDesc;

            while ( ( byte* ) commonDesc + Marshal.SizeOf(typeof(USB_COMMON_DESCRIPTOR)) < descEnd &&
                   ( byte* ) commonDesc + commonDesc->bLength <= descEnd ) {
                switch ( commonDesc->bDescriptorType ) {
                    case USB_DESCRIPTOR_TYPE.USB_CONFIGURATION_DESCRIPTOR_TYPE:
                        if ( commonDesc->bLength != Marshal.SizeOf(typeof(USB_CONFIGURATION_DESCRIPTOR)) ) {
                            HandleNativeFailure();
                            break;
                        }
                        if ( ( ( USB_CONFIGURATION_DESCRIPTOR* ) commonDesc )->iConfiguration != 0 ) {
                            GetStringDescriptors(hHubDevice,
                                                 ConnectionIndex,
                                                 ( ( USB_CONFIGURATION_DESCRIPTOR* ) commonDesc )->iConfiguration,
                                                 numLanguageIDs,
                                                 languageIDs,
                                                 stringDescriptors);
                        }
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;

                    case USB_DESCRIPTOR_TYPE.USB_INTERFACE_ASSOCIATION_DESCRIPTOR_TYPE:
                        if ( commonDesc->bLength < sizeof(USB_IAD_DESCRIPTOR) ) {
                            HandleNativeFailure();
                            break;
                        }
                        if ( ( ( USB_IAD_DESCRIPTOR* ) commonDesc )->iFunction != 0 ) {
                            GetStringDescriptors(hHubDevice,
                                                 ConnectionIndex,
                                                 ( ( USB_IAD_DESCRIPTOR* ) commonDesc )->iFunction,
                                                 numLanguageIDs,
                                                 languageIDs,
                                                 stringDescriptors);
                        }
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;

                    case USB_DESCRIPTOR_TYPE.USB_INTERFACE_DESCRIPTOR_TYPE:
                        if ( commonDesc->bLength != sizeof(USB_INTERFACE_DESCRIPTOR) &&
                            commonDesc->bLength != sizeof(USB_INTERFACE_DESCRIPTOR2) ) {
                            HandleNativeFailure();
                            break;
                        }
                        if ( ( ( USB_INTERFACE_DESCRIPTOR* ) commonDesc )->iInterface != 0 ) {
                            GetStringDescriptors(hHubDevice,
                                                 ConnectionIndex,
                                                 ( ( USB_INTERFACE_DESCRIPTOR* ) commonDesc )->iInterface,
                                                 numLanguageIDs,
                                                 languageIDs,
                                                 stringDescriptors);
                        }

                        // We need to display more string descriptors for the following
                        // interface classes
                        bInterfaceClass = ( ( USB_INTERFACE_DESCRIPTOR* ) commonDesc )->bInterfaceClass;
                        if ( bInterfaceClass == UsbApi.USB_DEVICE_CLASS_VIDEO ) {
                            getMoreStrings = true;
                        }
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;

                    default:
                        commonDesc = ( USB_COMMON_DESCRIPTOR* ) ( ( byte* ) commonDesc + commonDesc->bLength );
                        continue;
                }
                break;
            }

            if ( getMoreStrings ) {
                //
                // We might need to display strings later that are referenced only in
                // class-specific descriptors. Get String Descriptors 1 through 32 (an
                // arbitrary upper limit for Strings needed due to "bad devices"
                // returning an infinite repeat of Strings 0 through 4) until one is not
                // found.
                //
                // There are also "bad devices" that have issues even querying 1-32, but
                // historically USBView made this query, so the query should be safe for
                // video devices.
                //
                for ( uIndex = 1; hr && ( uIndex < NUM_STRING_DESC_TO_GET ); uIndex++ ) {
                    hr = GetStringDescriptors(hHubDevice,
                                              ConnectionIndex,
                                              uIndex,
                                              numLanguageIDs,
                                              languageIDs,
                                              stringDescriptors);
                }
            }

            return stringDescriptors;
        }


        private static unsafe USB_STRING_DESCRIPTOR_LANG? GetStringLanguagesDescriptor(
            IntPtr hHubDevice,
            uint ConnectionIndex,
            byte DescriptorIndex,
            short LanguageID
        ) {

            USB_STRING_DESCRIPTOR_LANG? langDesc =  GetUsbDescriptor<USB_STRING_DESCRIPTOR_LANG>(hHubDevice, ConnectionIndex, ( ushort ) ( ( ( int ) USB_DESCRIPTOR_TYPE.USB_STRING_DESCRIPTOR_TYPE << 8 ) | DescriptorIndex ), LanguageID);

            return langDesc;
        }

        //*****************************************************************************
        //
        // GetStringDescriptor()
        //
        // hHubDevice - Handle of the hub device containing the port from which the
        // String Descriptor will be requested.
        //
        // ConnectionIndex - Identifies the port on the hub to which a device is
        // attached from which the String Descriptor will be requested.
        //
        // DescriptorIndex - String Descriptor index.
        //
        // LanguageID - Language in which the string should be requested.
        //
        //*****************************************************************************

        private static unsafe StringDescriptorNode GetStringDescriptor(
            IntPtr hHubDevice,
            uint ConnectionIndex,
            byte DescriptorIndex,
            short LanguageID
        ) {

            USB_STRING_DESCRIPTOR? stringDesc =  GetUsbDescriptor<USB_STRING_DESCRIPTOR>(hHubDevice, ConnectionIndex, ( ushort ) ( ( ( int ) USB_DESCRIPTOR_TYPE.USB_STRING_DESCRIPTOR_TYPE << 8 ) | DescriptorIndex ), LanguageID);

            if ( stringDesc.HasValue ) {

                StringDescriptorNode stringDescNode = new StringDescriptorNode();

                stringDescNode.DescriptorIndex = DescriptorIndex;
                stringDescNode.LanguageID = LanguageID;
                stringDescNode.StringDescriptor = stringDesc.Value.Clone();
                return stringDescNode;
            } else {
                return null;
            }

#if false
            bool    success = false;
            int     nBytes = 0;
            int     nBytesReturned = 0;

            byte[]   stringDescReqBuf = new byte[SIZE_USB_DESCRIPTOR_REQUEST + MAXIMUM_USB_STRING_LENGTH];

            USB_DESCRIPTOR_REQUEST* stringDescReq = null;
            USB_STRING_DESCRIPTOR* stringDesc = null;
            StringDescriptorNode stringDescNode = null;

            nBytes = stringDescReqBuf.Length;

            stringDescReq = ( USB_DESCRIPTOR_REQUEST* ) &stringDescReqBuf;
            stringDesc = ( USB_STRING_DESCRIPTOR* ) ( &stringDescReqBuf + SIZE_USB_DESCRIPTOR_REQUEST );

            // Zero fill the entire request structure
            //
            // memset(stringDescReq, 0, nBytes);

            // Indicate the port from which the descriptor will be requested
            //
            stringDescReq->ConnectionIndex = ConnectionIndex;

            //
            // USBHUB uses URB_FUNCTION_GET_DESCRIPTOR_FROM_DEVICE to process this
            // IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION request.
            //
            // USBD will automatically initialize these fields:
            //     bmRequest = 0x80
            //     bRequest  = 0x06
            //
            // We must inititialize these fields:
            //     wValue    = Descriptor Type (high) and Descriptor Index (low byte)
            //     wIndex    = Zero (or Language ID for String Descriptors)
            //     wLength   = Length of descriptor buffer
            //
            stringDescReq->SetupPacket.wValue = (ushort) (( (int)USB_DESCRIPTOR_TYPE.USB_STRING_DESCRIPTOR_TYPE << 8 ) | DescriptorIndex);

            stringDescReq->SetupPacket.wIndex = LanguageID;

            stringDescReq->SetupPacket.wLength = ( ushort ) ( nBytes - SIZE_USB_DESCRIPTOR_REQUEST );

            // Now issue the get descriptor request.
            //
            success = Kernel32.DeviceIoControl(hHubDevice,
                                      Kernel32.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION,
                                      &stringDescReq,
                                      nBytes,
                                      &stringDescReq,
                                      nBytes,
                                      &nBytesReturned,
                                      IntPtr.Zero);

            // Do some sanity checks on the return from the get descriptor request.

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            if ( nBytesReturned < 2 ) {
                HandleNativeFailure();
                return null;
            }

            if ( stringDesc->bDescriptorType != USB_DESCRIPTOR_TYPE.USB_STRING_DESCRIPTOR_TYPE ) {
                HandleNativeFailure();
                return null;
            }

            if ( stringDesc->bLength != nBytesReturned - Marshal.SizeOf(typeof( USB_DESCRIPTOR_REQUEST )) ) {
                HandleNativeFailure();
                return null;
            }

            if ( stringDesc->bLength % 2 != 0 ) {
                HandleNativeFailure();
                return null;
            }

            //
            // Looks good, allocate some (zero filled) space for the string descriptor
            // node and copy the string descriptor to it.
            //

            // stringDescNode = ( StringDescriptorNode ) ALLOC(sizeof(STRING_DESCRIPTOR_NODE) +
            //                                                 stringDesc.bLength);

            // if ( stringDescNode == null ) {
            //     HandleNativeFailure();
            //     return null;
            // }
            stringDescNode = new StringDescriptorNode();

            stringDescNode.DescriptorIndex = DescriptorIndex;
            stringDescNode.LanguageID = LanguageID;
            stringDescNode.StringDescriptor = new USB_STRING_DESCRIPTOR();
            stringDescNode.StringDescriptor.bLength = stringDesc->bLength;
            stringDescNode.StringDescriptor.bDescriptorType = stringDesc->bDescriptorType;
            stringDescNode.StringDescriptor.bString = stringDesc->bString;

            return stringDescNode;
#endif
        }


        //*****************************************************************************
        //
        // GetStringDescriptors()
        //
        // hHubDevice - Handle of the hub device containing the port from which the
        // String Descriptor will be requested.
        //
        // ConnectionIndex - Identifies the port on the hub to which a device is
        // attached from which the String Descriptor will be requested.
        //
        // DescriptorIndex - String Descriptor index.
        //
        // NumLanguageIDs -  Number of languages in which the string should be
        // requested.
        //
        // LanguageIDs - Languages in which the string should be requested.
        //
        // StringDescNodeHead - First node in linked list of device's string descriptors
        //
        // Return Value: HRESULT indicating whether the string is on the list
        //
        //*****************************************************************************

        private static bool GetStringDescriptors(
            IntPtr                         hHubDevice,
            uint                           ConnectionIndex,
            byte                           DescriptorIndex,
            uint                           NumLanguageIDs,
            short[]                        LanguageIDs,
            StringDescriptorsCollection    DescriptorsList
        ) {
            StringDescriptorNode tail = null;
            StringDescriptorNode trailing = null;
            uint i = 0;

            //
            // Go to the end of the linked list, searching for the requested index to
            // see if we've already retrieved it
            //
            // for ( tail = StringDescNodeHead; tail != null; tail = tail->Next ) {
            //     if ( tail.DescriptorIndex == DescriptorIndex ) {
            //         return true;
            //     }
            // 
            //     trailing = tail;
            // }
            foreach (var node in DescriptorsList.Strings ) {
                if (node.DescriptorIndex == DescriptorIndex ) {
                    return true;
                }
            }

            // tail = StringDescNodeHead.Node.List.Last.Value;

            //
            // Get the next String Descriptor. If this is null, then we're done (return)
            // Otherwise, loop through all Language IDs
            bool foundDescriptor = false;
            for ( i = 0; ( i < NumLanguageIDs ); i++ ) {
                StringDescriptorNode descriptor = GetStringDescriptor(hHubDevice,
                                                 ConnectionIndex,
                                                 DescriptorIndex,
                                                 LanguageIDs[i]);
                if ( descriptor != null) {
                    DescriptorsList.Strings.Add( descriptor );
                    foundDescriptor = true;
                }
                // tail = tail->Next;
            }

            return foundDescriptor;
        }

        private static T? GetUsbDescriptor<T>(IntPtr hDevice, uint ConnectionIndex, ushort wValue, short LangIndex) where T : struct {

            int nBytesReturned;
            int nBytes = 255;

            // build a request for string descriptor
            USB_DESCRIPTOR_REQUEST Request = new USB_DESCRIPTOR_REQUEST();
            Request.ConnectionIndex = ConnectionIndex;
            Request.SetupPacket.wValue = wValue;
            Request.SetupPacket.wLength = ( ushort ) ( nBytes - Marshal.SizeOf(Request) );
            Request.SetupPacket.wIndex = LangIndex;

            // Geez, I wish C# had a Marshal.MemSet() method
            string NullString = new string((char)0, nBytes / Marshal.SystemDefaultCharSize);
            IntPtr ptrRequest = Marshal.StringToHGlobalAuto(NullString);
            Marshal.StructureToPtr(Request, ptrRequest, true);

            // Use an IOCTL call to request the String Descriptor
            if ( Kernel32.DeviceIoControl(hDevice, Kernel32.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION, ptrRequest, nBytes, ptrRequest, nBytes, out nBytesReturned, IntPtr.Zero) ) {
                // The location of the string descriptor is immediately after
                // the Request structure.  Because this location is not "covered"
                // by the structure allocation, we're forced to zero out this
                // chunk of memory by using the StringToHGlobalAuto() hack above
                IntPtr ptrStringDesc = new IntPtr(ptrRequest.ToInt64() + Marshal.SizeOf(Request));
                T descriptorData = (T)Marshal.PtrToStructure(ptrStringDesc, typeof(T));
                return descriptorData;
            } else {
                HandleNativeFailure();
            }
            Marshal.FreeHGlobal(ptrRequest);

            return null;
        }

        private static DeviceInfoNode FindMatchingDeviceNodeForDriverName(string DriverKeyName, bool IsHub) {
            DeviceInfoNode pNode            = new DeviceInfoNode();
            DeviceGuidList pList            = null;
            // LinkedListNode<DeviceInfoNode>  pEntry = null;

            pList = IsHub ? s_HubList : s_DeviceList;

            foreach ( var pEntry in pList.ListHead ) {
                if ( DriverKeyName == pNode.DeviceDriverName ) {
                    return pNode;
                }
            }

            // pEntry = pList.ListHead.First;
            // 
            // while ( pEntry != &pList.ListHead ) {
            //     pNode = CONTAINING_RECORD(pEntry,
            //                               DEVICE_INFO_NODE,
            //                               ListEntry);
            //     if ( DriverKeyName, pNode.DeviceDriverName ) {
            //         return pNode;
            //     }
            // 
            //     pEntry = pEntry->Flink;
            // }

            return null;
        }

        private static DeviceInfoNode FindMatchingDeviceNodeForDevicePath(string DevicePath, bool IsHub) {
            DeviceGuidList pList            = null;
            // LinkedListNode<DeviceInfoNode>  pEntry = null;

            pList = IsHub ? s_HubList : s_DeviceList;

            foreach ( DeviceInfoNode pEntry in pList.ListHead ) {
                if ( DevicePath.ToLowerInvariant() == pEntry.DeviceDetailData.DevicePath.ToLowerInvariant() ) {
                    DeviceInfoNode targetNode = pEntry;
                    return pEntry;
                }
            }

            return null;
        }


        internal static void HandleNativeFailure([CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            int error = Marshal.GetLastWin32Error();
            Logger.Fatal($"Failed to execute win32Function, got error \"{new Win32Exception(error).Message}\" ({error})", lineNumber, filePath, memberName);
            // throw new Win32Exception(error);
        }
    }
}
