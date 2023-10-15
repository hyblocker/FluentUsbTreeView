using FluentUsbTreeView.PInvoke;
using FluentUsbTreeView.Ui;
using System;
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
using static FluentUsbTreeView.PInvoke.UsbApi;
using WpfTreeViewItem = System.Windows.Controls.TreeViewItem;

namespace FluentUsbTreeView.UsbTreeView {
    public class UsbEnumator {
        private const int NUM_STRING_DESC_TO_GET = 32;
        private const int MAX_DRIVER_KEY_NAME = 256;
        private static uint TotalDevicesConnected;
        private static uint TotalHubs;

        private static LinkedList<USBHOSTCONTROLLERINFO> EnumeratedHCListHead = new LinkedList<USBHOSTCONTROLLERINFO>();

        // private static DEVICE_GUID_LIST gHubList;
        // private static DEVICE_GUID_LIST gDeviceList;

        public static void EnumerateHostControllers(WpfTreeViewItem hTreeParent, ref uint DevicesConnected) {

            IntPtr                              hHCDev = IntPtr.Zero;
            IntPtr                              deviceInfo = IntPtr.Zero;
            SP_DEVINFO_DATA                     deviceInfoData = new SP_DEVINFO_DATA();
            SP_DEVICE_INTERFACE_DATA            deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            IntPtr                              deviceDetailData = IntPtr.Zero;
            uint                                index = 0;
            uint                                requiredLength = 0;
            bool                                success;

            EnumeratedHCListHead.Clear();

            TotalDevicesConnected = 0;
            TotalHubs = 0;
            EnumerateAllDevices();

            // Iterate over host controllers using the new GUID based interface
            Guid usbHostControllerClass = UsbApi.GUID_CLASS_USB_HOST_CONTROLLER;
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

            DevicesConnected = TotalDevicesConnected;
        }

        public static void EnumerateHostController(WpfTreeViewItem hTreeParent, IntPtr hHCDev, ref string leafName, IntPtr deviceInfo, ref SP_DEVINFO_DATA deviceInfoData) {
            string                  driverKeyName = null;
            WpfTreeViewItem         hHCItem = null;
            string                  rootHubName = null;
            LinkedListNode<USBHOSTCONTROLLERINFO>             listEntry = null;
            USBHOSTCONTROLLERINFO   hcInfo = new USBHOSTCONTROLLERINFO();
            // USBHOSTCONTROLLERINFO   hcInfoInList = new USBHOSTCONTROLLERINFO();
            Int32                   dwSuccess;
            bool                    success = false;
            uint                    deviceAndFunction = 0;
            USB_DEVICE_PNP_STRINGS  DevProps = new USB_DEVICE_PNP_STRINGS();


            // Allocate a structure to hold information about this host controller.
            hcInfo.DeviceInfoType = DeviceInfoType.HostController;

            // Obtain the driver key name for this host controller.
            driverKeyName = GetHCDDriverKeyName(hHCDev);

            if ( driverKeyName == null ) {
                // Failure obtaining driver key name.
                HandleNativeFailure();
                return;
            }

            // Don't enumerate this host controller again if it already
            // on the list of enumerated host controllers.
            foreach ( USBHOSTCONTROLLERINFO hcInfoInList in EnumeratedHCListHead ) {
                if ( driverKeyName == hcInfoInList.DriverKey ) {
                    // Already on the list, exit
                    return;
                }
            }

            // Prepare the node for this entry on the tree
            listEntry = new LinkedListNode<USBHOSTCONTROLLERINFO>(hcInfo);
            hcInfo.ListEntry = listEntry;

            // Obtain host controller device properties
            if ( driverKeyName.Length < MAX_DRIVER_KEY_NAME ) {
                DevProps = DeviceNode.DriverNameToDeviceProperties(driverKeyName, driverKeyName.Length);
            }

            hcInfo.DriverKey = driverKeyName;

            // Extract VEN, DEV, SUBSYS, REV from the device instance id
            uint ven, dev, subsys, rev;
            ven = dev = subsys = rev = 0;

            Regex r = new Regex(@"^PCI\\VEN_(?<ven>[0-9a-fA-F]+)&DEV_(?<dev>[0-9a-fA-F]+)&SUBSYS_(?<subsys>[0-9a-fA-F]+)&REV_(?<rev>[0-9a-fA-F]+)", RegexOptions.None);

            Match m = r.Match(DevProps.DeviceId);
            if ( m.Success ) {
                ven = Convert.ToUInt32(m.Groups["ven"].Value, 16);
                dev = Convert.ToUInt32(m.Groups["dev"].Value, 16);
                subsys = Convert.ToUInt32(m.Groups["subsys"].Value, 16);
                rev = Convert.ToUInt32(m.Groups["rev"].Value, 16);
            }

            hcInfo.VendorID = ven;
            hcInfo.DeviceID = dev;
            hcInfo.SubSysID = subsys;
            hcInfo.Revision = ( USB_DEVICE_SPEED ) rev;
            hcInfo.UsbDeviceProperties = DevProps;

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

            leafName = DeviceNameUtil.GetFriendlyUsbHostControllerName(hcInfo);

            // Add this host controller to the USB device tree view.
            hHCItem = MainWindow.Instance.AddLeaf(hTreeParent, hcInfo, leafName, UsbTreeIcon.HostController);

            if ( hHCItem == null ) {
                // Failure adding host controller to USB device tree
                // view.
                HandleNativeFailure();
                return;
            }

            // Add this host controller to the list of enumerated
            // host controllers.
            //
            // InsertTailList(&EnumeratedHCListHead, &hcInfo.ListEntry);
            EnumeratedHCListHead.AddLast(hcInfo.ListEntry);

            // Get the name of the root hub for this host
            // controller and then enumerate the root hub.
            rootHubName = GetRootHubName(hHCDev);

            // if ( rootHubName != NULL ) {
            //     size_t cbHubName = 0;
            //     HRESULT hr = S_OK;
            // 
            //     hr = StringCbLength(rootHubName, MAX_DRIVER_KEY_NAME, &cbHubName);
            //     if ( SUCCEEDED(hr) ) {
            //         EnumerateHub(hHCItem,
            //                      rootHubName,
            //                      cbHubName,
            //                      null,       // ConnectionInfo
            //                      null,       // ConnectionInfoV2
            //                      null,       // PortConnectorProps
            //                      null,       // ConfigDesc
            //                      null,       // BosDesc
            //                      null,       // StringDescs
            //                      null);      // We do not pass DevProps for RootHub
            //     }
            // } else {
            //     // Failure obtaining root hub name.
            //     HandleNativeFailure();
            // }
        }

        private static void EnumerateAllDevices() {

        }

        private static int GetHostControllerPowerMap(IntPtr hHCDev, ref USBHOSTCONTROLLERINFO hcInfo) {
            USBUSER_POWER_INFO_REQUEST UsbPowerInfoRequest = new USBUSER_POWER_INFO_REQUEST();
            USB_POWER_INFO             pUPI = UsbPowerInfoRequest.PowerInformation ;
            int                        dwError = 0;
            uint                       dwBytes = 0;
            bool                       bSuccess = false;
            int                        nIndex = 0;
            int                        nPowerState = (int)WDMUSB_POWER_STATE.SystemWorking;

            for ( ; nPowerState <= ( int ) WDMUSB_POWER_STATE.SystemShutdown; nIndex++, nPowerState++ ) {
                // set the header and request sizes
                UsbPowerInfoRequest.Header.UsbUserRequest = UsbApi.USBUSER_GET_POWER_STATE_MAP;
                UsbPowerInfoRequest.Header.RequestBufferLength = (uint) Marshal.SizeOf(UsbPowerInfoRequest);
                UsbPowerInfoRequest.PowerInformation.SystemState = (WDMUSB_POWER_STATE) nPowerState;

                // Now query USBHUB for the USB_POWER_INFO structure for this hub.
                // For Selective Suspend support
                IntPtr UsbPowerInfoRequestPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USBUSER_POWER_INFO_REQUEST)));
                uint sizeOfPowerRequest = (uint)Marshal.SizeOf(typeof(USBUSER_POWER_INFO_REQUEST));
                Marshal.StructureToPtr(UsbPowerInfoRequest, UsbPowerInfoRequestPtr, false);
                bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, UsbPowerInfoRequestPtr, sizeOfPowerRequest, UsbPowerInfoRequestPtr, sizeOfPowerRequest, out dwBytes, IntPtr.Zero);
                UsbPowerInfoRequest = ( USBUSER_POWER_INFO_REQUEST ) Marshal.PtrToStructure(UsbPowerInfoRequestPtr, typeof(USBUSER_POWER_INFO_REQUEST));
                Marshal.FreeHGlobal(UsbPowerInfoRequestPtr);
                UsbPowerInfoRequestPtr = IntPtr.Zero;

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

        private static int GetHostControllerInfo(IntPtr hHCDev, ref USBHOSTCONTROLLERINFO hcInfo) {
            USBUSER_CONTROLLER_INFO_0 UsbControllerInfo = new USBUSER_CONTROLLER_INFO_0();
            int                        dwError = 0;
            uint                       dwBytes = 0;
            bool                       bSuccess = false;

            // set the header and request sizes
            UsbControllerInfo.Header.UsbUserRequest = UsbApi.USBUSER_GET_CONTROLLER_INFO_0;
            UsbControllerInfo.Header.RequestBufferLength = (uint)Marshal.SizeOf(UsbControllerInfo);

            // Query for the USB_CONTROLLER_INFO_0 structure
            uint sizeUsbControllerInfo = (uint)Marshal.SizeOf(typeof(USBUSER_CONTROLLER_INFO_0));
            IntPtr UsbControllerInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USBUSER_CONTROLLER_INFO_0)));
            Marshal.StructureToPtr(UsbControllerInfo, UsbControllerInfoPtr, false);
            bSuccess = Kernel32.DeviceIoControl(hHCDev, Kernel32.IOCTL_USB_USER_REQUEST, UsbControllerInfoPtr, sizeUsbControllerInfo, UsbControllerInfoPtr, sizeUsbControllerInfo, out dwBytes, IntPtr.Zero);
            UsbControllerInfo = ( USBUSER_CONTROLLER_INFO_0 ) Marshal.PtrToStructure(UsbControllerInfoPtr, typeof(USBUSER_CONTROLLER_INFO_0));
            Marshal.FreeHGlobal(UsbControllerInfoPtr);
            UsbControllerInfoPtr = IntPtr.Zero;

            if ( !bSuccess ) {
                dwError = Marshal.GetLastWin32Error();
                HandleNativeFailure();
            } else {
                hcInfo.ControllerInfo = UsbControllerInfo.Info0.Clone();
            }
            return dwError;
        }

        private static int GetHostControllerBandwidth(IntPtr hHCDev, ref USBHOSTCONTROLLERINFO hcInfo) {
            USBUSER_BANDWIDTH_INFO_REQUEST  UsbBandInfoRequest = new USBUSER_BANDWIDTH_INFO_REQUEST();
            int                             dwError = 0;
            uint                            dwBytes = 0;
            bool                            bSuccess = false;

            // set the header and request sizes
            UsbBandInfoRequest.Header.UsbUserRequest = UsbApi.USBUSER_GET_BANDWIDTH_INFORMATION;
            UsbBandInfoRequest.Header.RequestBufferLength = ( uint ) Marshal.SizeOf(UsbBandInfoRequest);

            // Query for the USBUSER_BANDWIDTH_INFO_REQUEST structure
            uint sizeUsbControllerInfo = (uint)Marshal.SizeOf(typeof(USBUSER_BANDWIDTH_INFO_REQUEST));
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

        private static string GetRootHubName(IntPtr HostController) {
            bool                    success = false;
            uint                    nBytes = 0;
            USB_ROOT_HUB_NAME       rootHubName = new USB_ROOT_HUB_NAME();
            USB_ROOT_HUB_NAME       rootHubNameW = new USB_ROOT_HUB_NAME();

            // Get the length of the name of the driver key of the HCD
            int rootHubNameSize = Marshal.SizeOf(typeof(USB_ROOT_HUB_NAME));
            IntPtr rootHubNamePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_ROOT_HUB_NAME)));
            Marshal.StructureToPtr(rootHubName, rootHubNamePtr, false);
            success = Kernel32.DeviceIoControl(HostController, Kernel32.IOCTL_USB_GET_ROOT_HUB_NAME, rootHubNamePtr, ( uint ) rootHubNameSize, rootHubNamePtr, ( uint ) rootHubNameSize, out nBytes, IntPtr.Zero);
            rootHubName = ( USB_ROOT_HUB_NAME ) Marshal.PtrToStructure(rootHubNamePtr, typeof(USB_ROOT_HUB_NAME));
            Marshal.FreeHGlobal(rootHubNamePtr);
            rootHubNamePtr = IntPtr.Zero;

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            // Allocate space to hold the driver key name
            nBytes = ( uint ) rootHubName.ActualLength;
            if ( nBytes <= 6 /* sizeof(rootHubName) */ ) {
                HandleNativeFailure();
                return null;
            }

            // Get the name of the driver key of the device attached to
            // the specified port.
            IntPtr rootHubNameWPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_ROOT_HUB_NAME)));
            Marshal.StructureToPtr(rootHubNameW, rootHubNameWPtr, false);
            success = Kernel32.DeviceIoControl(HostController, Kernel32.IOCTL_USB_GET_ROOT_HUB_NAME, rootHubNameWPtr, nBytes, rootHubNameWPtr, nBytes, out nBytes, IntPtr.Zero);
            rootHubNameW = ( USB_ROOT_HUB_NAME ) Marshal.PtrToStructure(rootHubNameWPtr, typeof(USB_ROOT_HUB_NAME));
            Marshal.FreeHGlobal(rootHubNameWPtr);
            rootHubNameWPtr = IntPtr.Zero;

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            return rootHubNameW.RootHubName;
        }

        private static string GetHCDDriverKeyName(IntPtr HCD) {
            bool                    success = false;
            uint                    nBytes = 0;
            USB_HCD_DRIVERKEY_NAME  driverKeyName = new USB_HCD_DRIVERKEY_NAME();
            USB_HCD_DRIVERKEY_NAME  driverKeyNameW = new USB_HCD_DRIVERKEY_NAME();

            // Get the length of the name of the driver key of the HCD
            int driverKeyNameSize = Marshal.SizeOf(typeof(USB_HCD_DRIVERKEY_NAME));
            IntPtr driverKeyNamePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_HCD_DRIVERKEY_NAME)));
            Marshal.StructureToPtr(driverKeyName, driverKeyNamePtr, false);
            success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_GET_HCD_DRIVERKEY_NAME, driverKeyNamePtr, ( uint ) driverKeyNameSize, driverKeyNamePtr, ( uint ) driverKeyNameSize, out nBytes, IntPtr.Zero);
            driverKeyName = (USB_HCD_DRIVERKEY_NAME) Marshal.PtrToStructure(driverKeyNamePtr, typeof(USB_HCD_DRIVERKEY_NAME));
            Marshal.FreeHGlobal(driverKeyNamePtr);
            driverKeyNamePtr = IntPtr.Zero;

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            // Allocate space to hold the driver key name
            nBytes = (uint) driverKeyName.ActualLength;
            if ( nBytes <= 6 /* sizeof(driverKeyName) */ ) {
                HandleNativeFailure();
                return null;
            }

            // Get the name of the driver key of the device attached to
            // the specified port.
            IntPtr driverKeyNameWPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(USB_HCD_DRIVERKEY_NAME)));
            Marshal.StructureToPtr(driverKeyNameW, driverKeyNameWPtr, false);
            success = Kernel32.DeviceIoControl(HCD, Kernel32.IOCTL_GET_HCD_DRIVERKEY_NAME, driverKeyNameWPtr, nBytes, driverKeyNameWPtr, nBytes, out nBytes, IntPtr.Zero);
            driverKeyNameW = ( USB_HCD_DRIVERKEY_NAME ) Marshal.PtrToStructure(driverKeyNameWPtr, typeof(USB_HCD_DRIVERKEY_NAME));
            Marshal.FreeHGlobal(driverKeyNameWPtr);
            driverKeyNameWPtr = IntPtr.Zero;

            if ( !success ) {
                HandleNativeFailure();
                return null;
            }

            return driverKeyNameW.DriverKeyName;
        }

        public static bool GetDeviceProperty(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, DevRegProperty Property, out string ppBuffer) {
            bool bResult;
            uint requiredLength = 0;
            int lastError;

            bResult = SetupApi.SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out _, IntPtr.Zero, 0, out requiredLength);
            lastError = Marshal.GetLastWin32Error();

            if ( ( requiredLength == 0 ) || ( bResult != false && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = "";
                return false;
            }


            StringBuilder ppBufferStringBuilder = new StringBuilder( ( int ) requiredLength );
            bResult = SetupApi.SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out _, ppBufferStringBuilder, requiredLength, out requiredLength);

            if ( bResult == false ) {
                ppBuffer = "";
                return false;
            }

            ppBuffer = ppBufferStringBuilder.ToString();

            return true;
        }

        public static bool GetDeviceProperty<T>(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, DevRegProperty Property, out T ppBuffer) where T : struct {
            bool bResult;
            uint requiredLength = 0;
            int lastError;

            bResult = SetupApi.SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out _, IntPtr.Zero, 0, out requiredLength);
            lastError = Marshal.GetLastWin32Error();
            
            if ( ( requiredLength == 0 ) || ( bResult != false && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) ) {
                ppBuffer = new T();
                return false;
            }

            ppBuffer = new T();

            IntPtr ppBufferPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(ppBuffer, ppBufferPtr, false);
            bResult = SetupApi.SetupDiGetDeviceRegistryProperty(DeviceInfoSet, ref DeviceInfoData, Property, out _, ppBufferPtr, requiredLength, out requiredLength);
            ppBuffer = ( T ) Marshal.PtrToStructure(ppBufferPtr, typeof(T));
            Marshal.FreeHGlobal(ppBufferPtr);
            ppBufferPtr = IntPtr.Zero;


            if ( bResult == false ) {
                ppBuffer = new T();
                return false;
            }

            // ppBuffer = ppBufferStringBuilder.ToString();

            return true;
        }

        private static void HandleNativeFailure([CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            int error = Marshal.GetLastWin32Error();
            Logger.Fatal($"Failed to execute win32DiFunction, got error {error}", lineNumber, filePath, memberName);
            throw new Win32Exception(error);
        }
    }
}
