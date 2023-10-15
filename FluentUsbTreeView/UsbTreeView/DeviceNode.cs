using FluentUsbTreeView.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

            Guid nullGuid = Guid.Empty;
            deviceInfo = SetupApi.SetupDiGetClassDevs(ref nullGuid, null, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_PRESENT);

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
                bResult = UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_DRIVER, out buf);

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
        public static USB_DEVICE_PNP_STRINGS DriverNameToDeviceProperties(string DriverName, int cbDriverName) {
            IntPtr          deviceInfo = Kernel32.INVALID_HANDLE_VALUE;
            SP_DEVINFO_DATA deviceInfoData;
            int            len;
            bool            status;
            USB_DEVICE_PNP_STRINGS DevProps = new USB_DEVICE_PNP_STRINGS();
            int             lastError;

            // Get device instance
            status = DriverNameToDeviceInst(DriverName, cbDriverName, out deviceInfo, out deviceInfoData);
            if ( status == false ) {
                goto Done;
            }

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

            status = UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_DEVICEDESC, out DevProps.DeviceDesc);

            if ( status == false ) {
                goto Done;
            }
            
            // We don't fail if the following registry query fails as these fields are additional information only
            UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_HARDWAREID, out DevProps.HwId);
            UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_SERVICE, out DevProps.Service);
            UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_CLASS, out DevProps.DeviceClass);
            UsbEnumator.GetDeviceProperty(deviceInfo, deviceInfoData, DevRegProperty.SPDRP_DEVICE_POWER_DATA, out DevProps.PowerState);
        Done:

            if ( deviceInfo != Kernel32.INVALID_HANDLE_VALUE ) {
                SetupApi.SetupDiDestroyDeviceInfoList(deviceInfo);
            }

            return DevProps;
        }

    }
}
