using FluentUsbTreeView.PInvoke;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static FluentUsbTreeView.PInvoke.CfgMgr32;
using static FluentUsbTreeView.PInvoke.UsbApi;
using static FluentUsbTreeView.PInvoke.WindowsTools;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace FluentUsbTreeView.UsbTreeView {
    public static class DeviceNode {

        public static bool DriverNameToDeviceInst(string DriverName, int cbDriverName, out IntPtr pDevInfo, out SP_DEVINFO_DATA pDevInfoData) {
            IntPtr           deviceInfo = Kernel32.INVALID_HANDLE_VALUE;
            bool             status = true;
            uint             deviceIndex;
            SP_DEVINFO_DATA  deviceInfoData = new SP_DEVINFO_DATA();
            bool             bResult = false;
            string           pDriverName = null;
            string           buf = null;
            bool             done = false;

            pDevInfoData = new SP_DEVINFO_DATA();
            pDevInfo = Kernel32.INVALID_HANDLE_VALUE;

            // Use local string to guarantee zero termination
            pDriverName = string.Copy(DriverName);
            // We cannot walk the device tree with CM_Get_Sibling etc. unless we assume
            // the device tree will stabilize. Any devnode removal (even outside of USB)
            // would force us to retry. Instead we use Setup API to snapshot all
            // devices.

            // Examine all present devices to see if any match the given DriverName

            deviceInfo = SetupApi.SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_PRESENT);

            if ( deviceInfo == Kernel32.INVALID_HANDLE_VALUE ) {
                status = false;
                goto Done;
            }

            deviceIndex = 0;
            deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

            while ( done == false ) {
                // Get devinst of the next device
                status = SetupApi.SetupDiEnumDeviceInfo(deviceInfo, deviceIndex, ref deviceInfoData);

                deviceIndex++;

                if ( !status ) {
                    // This could be an error, or indication that all devices have been
                    // processed. Either way the desired device was not found.

                    done = true;
                    break;
                }

                // Get the DriverName value
                bResult = UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_DRIVER, out buf);

                // If the DriverName value matches, return the DeviceInstance
                if ( bResult == true && buf != null && pDriverName == buf) {
                    done = true;
                    pDevInfo = new IntPtr(deviceInfo.ToInt64());
                    pDevInfoData = deviceInfoData.Clone();
                    break;
                }

                if ( buf != null ) {
                    buf = null;
                }
            }

        Done:

            if ( bResult == false ) {
                if ( deviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {
                    SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);
                }
            }

            return status;
        }


        public static bool DevicePathToDrvierKeyName(string devicePath, out string driverKeyName) {
            IntPtr                      deviceInfo = Kernel32.INVALID_HANDLE_VALUE;
            bool                        status = true;
            uint                        deviceIndex;
            SP_DEVINFO_DATA             deviceInfoData = new SP_DEVINFO_DATA();
            SP_DEVICE_INTERFACE_DATA    deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            IntPtr                      deviceDetailData = IntPtr.Zero;
            uint                        requiredLength = 0;
            bool                        bResult = false;
            string                      pDevicePath = null;
            string                      buf = null;
            bool                        done = false;

            // Set to null initially
            driverKeyName = null;

            // We use the registry here because the windows api seems to fail randomly...

            // First extract the full path to the registry node we wish to visit
            if ( devicePath.StartsWith("\\\\.\\") ) {
                RegistryKey registryNode = null;
                try {
                    // HKLM\SYSTEM\CurrentControlSet\Enum\{FULLPATH}::Driver
                    string FULLPATH = devicePath.Substring(4, devicePath.LastIndexOf('#') - 4).Replace('#', '\\');
                    registryNode = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Enum\\{FULLPATH}", false);
                    if ( registryNode != null ) {
                        if (registryNode.GetValueKind("Driver") == RegistryValueKind.String) {
                            string driverKeyRegistryValue = (string)registryNode.GetValue("Driver");
                            driverKeyName = driverKeyRegistryValue;
                            status = true;
                        }
                    }
                } finally {
                    if ( registryNode != null ) {
                        registryNode.Close();
                    }
                }
            }

            // Use local string to guarantee zero termination
            pDevicePath = string.Copy(devicePath);
            // We cannot walk the device tree with CM_Get_Sibling etc. unless we assume
            // the device tree will stabilize. Any devnode removal (even outside of USB)
            // would force us to retry. Instead we use Setup API to snapshot all
            // devices.

            // Examine all present devices to see if any match the given DriverName


            return status;
        }

        public static UsbDevicePnpStrings DriverNameToDeviceProperties(string DriverName, int cbDriverName) {
            IntPtr          deviceInfo = Kernel32.INVALID_HANDLE_VALUE;
            SP_DEVINFO_DATA deviceInfoData;
            int            len;
            bool            status;
            UsbDevicePnpStrings DevProps = new UsbDevicePnpStrings();
            int             lastError;

            // Get device instance
            status = DriverNameToDeviceInst(DriverName, cbDriverName, out deviceInfo, out deviceInfoData);
            if ( status == false ) {
                // goto Done;
            }

            return PollDeviceProperties(deviceInfo, deviceInfoData);

            /*
            len = 0;
            status = SetupApi.SetupDiGetDeviceInstanceId(deviceInfo, ref deviceInfoData, null, 0, out len);
            lastError = Marshal.GetLastWin32Error();


            if ( status != false && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) {
                status = false;
                goto Done;
            }

            // An extra byte is required for the terminating character
            len++;
            StringBuilder deviceIdStringBuilder = new StringBuilder( len);

            status = SetupApi.SetupDiGetDeviceInstanceId(deviceInfo, ref deviceInfoData, deviceIdStringBuilder, len, out len);
            if ( status == false ) {
                goto Done;
            }
            DevProps.DeviceId = deviceIdStringBuilder.ToString();

            // Get device desc
            status = UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_DEVICEDESC, out DevProps.DeviceDesc);
            if ( status == false ) {
                goto Done;
            }

            // Get device problem code
            CR_RESULT pollStatus = Cfgmgr32.CM_Get_DevNode_Status(out DevProps.Status, out DevProps.ProblemCode, deviceInfoData.DevInst, 0);

            // We don't fail if the following registry query fails as these fields are additional information only
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_HARDWAREID, out DevProps.HwId);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_SERVICE, out DevProps.Service);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_CLASS, out DevProps.DeviceClass);
            UsbEnumator.GetDevicePropertyStruct(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_CLASSGUID, out DevProps.DeviceClassGuid);
            UsbEnumator.GetDevicePropertyStruct(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_DEVICE_POWER_DATA, out DevProps.PowerState);
            // to cast to enum since generic enums are awful
            int legacyBusType = 0;
            int capabilities = 0;
            UsbEnumator.GetDevicePropertyInt32(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_LEGACYBUSTYPE, out legacyBusType);
            UsbEnumator.GetDevicePropertyInt32(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_CAPABILITIES, out capabilities);
            DevProps.LegacyBusType = (INTERFACE_TYPE) legacyBusType;
            DevProps.Capabilities = (CM_DEVCAP) capabilities;
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_ENUMERATOR_NAME, out DevProps.Enumerator);
            UsbEnumator.GetDevicePropertyInt32(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_ADDRESS, out DevProps.Address);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_BASE_CONTAINERID, out DevProps.ContainerId);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_LOCATION_INFORMATION, out DevProps.LocationInfo);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_LOCATION_PATHS, out DevProps.LocationPaths);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_FRIENDLYNAME, out DevProps.FriendlyName);
            UsbEnumator.GetDevicePropertyString(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_MFG, out DevProps.Manufacturer);
            // UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_LOCATION_PATHS, out DevProps.LocationPaths);

            // Extract VEN, DEV, SUBSYS, REV from the device instance id
            // NOTE: ven = USB VID ; dev = USB PID, SUBSYS and REV are 0 on USB
            uint ven, dev, subsys, rev;
            ven = dev = subsys = rev = 0;

            if (DevProps.Enumerator == "USB") {
                Regex r = new Regex(@"^USB\\VID_(?<vid>[0-9a-fA-F]+)&PID_(?<pid>[0-9a-fA-F]+)", RegexOptions.None);

                Match m = r.Match(DevProps.DeviceId);
                if ( m.Success ) {
                    ven     = Convert.ToUInt32(m.Groups["vid"].Value, 16);
                    dev     = Convert.ToUInt32(m.Groups["pid"].Value, 16);
                }
            } else if (DevProps.Enumerator == "PCI") {
                Regex r = new Regex(@"^PCI\\VEN_(?<ven>[0-9a-fA-F]+)&DEV_(?<dev>[0-9a-fA-F]+)&SUBSYS_(?<subsys>[0-9a-fA-F]+)&REV_(?<rev>[0-9a-fA-F]+)", RegexOptions.None);

                Match m = r.Match(DevProps.DeviceId);
                if ( m.Success ) {
                    ven     = Convert.ToUInt32(m.Groups["ven"].Value, 16);
                    dev     = Convert.ToUInt32(m.Groups["dev"].Value, 16);
                    subsys  = Convert.ToUInt32(m.Groups["subsys"].Value, 16);
                    rev     = Convert.ToUInt32(m.Groups["rev"].Value, 16);
                }
            }

            DevProps.VendorID = ven;
            DevProps.ProductID = dev;
            DevProps.SubSysID = subsys;
            DevProps.Revision = ( USB_DEVICE_SPEED ) rev;
        Done:

            if ( deviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);
            }

            return DevProps;
            */
        }

        public static UsbDevicePnpStrings PollDeviceProperties(IntPtr deviceInfo, SP_DEVINFO_DATA deviceInfoData) {
            int             len;
            bool            bStatus;
            CR_RESULT       eStatus;
            UsbDevicePnpStrings DevProps = new UsbDevicePnpStrings();
            int             lastError;

            // Get device instance
            len = 0;
            // Errors on usb hubs?
            eStatus = CfgMgr32.CM_Get_Device_ID_Size(out len, deviceInfoData.DevInst);
            lastError = Marshal.GetLastWin32Error();


            if ( eStatus != CR_RESULT.CR_SUCCESS && lastError != Kernel32.ERROR_INSUFFICIENT_BUFFER ) {
                goto Done;
            }

            // An extra byte is required for the terminating character
            len++;
            StringBuilder deviceIdStringBuilder = new StringBuilder( len);

            eStatus = CfgMgr32.CM_Get_Device_ID(deviceInfoData.DevInst, deviceIdStringBuilder, len);
            if ( eStatus != CR_RESULT.CR_SUCCESS ) {
                goto Err;
            }
            DevProps.DeviceId = deviceIdStringBuilder.ToString();

            // Get device desc
            bStatus = UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_DEVICEDESC, out DevProps.DeviceDesc);
            if ( bStatus == false ) {
                goto Err;
            }

            // Get device problem code
            CR_RESULT pollStatus = CfgMgr32.CM_Get_DevNode_Status(out DevProps.Status, out DevProps.ProblemCode, deviceInfoData.DevInst, 0);

            // We don't fail if the following registry query fails as these fields are additional information only
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_HARDWAREID, out DevProps.HwId);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_SERVICE, out DevProps.Service);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_CLASS, out DevProps.DeviceClass);
            UsbEnumator.GetDevicePropertyStruct(deviceInfoData.DevInst, CM_DRP.CM_DRP_CLASSGUID, out DevProps.DeviceClassGuid);
            UsbEnumator.GetDevicePropertyStruct(deviceInfoData.DevInst, CM_DRP.CM_DRP_DEVICE_POWER_DATA, out DevProps.PowerState);
            // to cast to enum since generic enums are awful
            int legacyBusType = 0;
            int capabilities = 0;
            UsbEnumator.GetDevicePropertyInt32(deviceInfoData.DevInst, CM_DRP.CM_DRP_LEGACYBUSTYPE, out legacyBusType);
            UsbEnumator.GetDevicePropertyInt32(deviceInfoData.DevInst, CM_DRP.CM_DRP_CAPABILITIES, out capabilities);
            DevProps.LegacyBusType = ( INTERFACE_TYPE ) legacyBusType;
            DevProps.Capabilities = ( CM_DEVCAP ) capabilities;
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_ENUMERATOR_NAME, out DevProps.Enumerator);
            UsbEnumator.GetDevicePropertyInt32(deviceInfoData.DevInst,  CM_DRP.CM_DRP_ADDRESS, out DevProps.Address);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_BASE_CONTAINERID, out DevProps.ContainerId);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_LOCATION_INFORMATION, out DevProps.LocationInfo);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_LOCATION_PATHS, out DevProps.LocationPaths);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_FRIENDLYNAME, out DevProps.FriendlyName);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_MFG, out DevProps.Manufacturer);

            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, Devpkey.DEVPKEY_Device_PhysicalDeviceLocation, out DevProps.Kernel);

            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, CM_DRP.CM_DRP_DRIVER, out DevProps.Driver);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, Devpkey.DEVPKEY_Device_DriverInfPath, out DevProps.DriverInf);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, Devpkey.DEVPKEY_Device_DriverVersion, out DevProps.DriverVersion);
            UsbEnumator.GetDevicePropertyString(deviceInfoData.DevInst, Devpkey.DEVPKEY_Device_DriverDesc, out DevProps.DriverCompany);
            FILETIME driverDate;
            UsbEnumator.GetDevicePropertyFileTime(deviceInfoData.DevInst, Devpkey.DEVPKEY_Device_DriverDate, out driverDate);
            LargeIntegerStruct driverU64S = new LargeIntegerStruct() { HighPart = driverDate.dwHighDateTime, LowPart = (uint)driverDate.dwLowDateTime };
            DevProps.DriverDate = driverU64S.ToDateTime();
            // UsbEnumator.GetDeviceProperty(deviceInfoData.DevInst, CM_DRP.CM_DRP_LOCATION_PATHS, out DevProps.LocationPaths);

            // Extract VEN, DEV, SUBSYS, REV from the device instance id
            // NOTE: ven = USB VID ; dev = USB PID, SUBSYS and REV are 0 on USB
            uint ven, dev, subsys, rev;
            ven = dev = subsys = rev = 0;

            if ( DevProps.Enumerator == "USB" ) {
                Regex r = new Regex(@"^USB\\VID_(?<vid>[0-9a-fA-F]+)&PID_(?<pid>[0-9a-fA-F]+)", RegexOptions.None);

                Match m = r.Match(DevProps.DeviceId);
                if ( m.Success ) {
                    ven = Convert.ToUInt32(m.Groups["vid"].Value, 16);
                    dev = Convert.ToUInt32(m.Groups["pid"].Value, 16);
                }
            } else if ( DevProps.Enumerator == "PCI" ) {
                Regex r = new Regex(@"^PCI\\VEN_(?<ven>[0-9a-fA-F]+)&DEV_(?<dev>[0-9a-fA-F]+)&SUBSYS_(?<subsys>[0-9a-fA-F]+)&REV_(?<rev>[0-9a-fA-F]+)", RegexOptions.None);

                Match m = r.Match(DevProps.DeviceId);
                if ( m.Success ) {
                    ven = Convert.ToUInt32(m.Groups["ven"].Value, 16);
                    dev = Convert.ToUInt32(m.Groups["dev"].Value, 16);
                    subsys = Convert.ToUInt32(m.Groups["subsys"].Value, 16);
                    rev = Convert.ToUInt32(m.Groups["rev"].Value, 16);
                }
            }

            DevProps.VendorID = ven;
            DevProps.ProductID = dev;
            DevProps.SubSysID = subsys;
            DevProps.Revision = ( USB_DEVICE_SPEED ) rev;
            goto Done;
        Err:
            lastError = Marshal.GetLastWin32Error();
            Logger.Fatal($"Failed to execute win32Function, got error \"{new Win32Exception(lastError).Message}\" ({lastError})");

        Done:

            if ( deviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);
            }

            return DevProps;
        }
    }
}
