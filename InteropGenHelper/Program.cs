using System.Runtime.InteropServices;
using Windows.Win32;

namespace InteropGenHelper {
    internal class Program {
        static void Main(string[] args) {

            IntPtr hHCDev = IntPtr.Zero;

            Windows.Win32.Devices.Usb.USBUSER_CONTROLLER_INFO_0 UsbControllerInfo = new Windows.Win32.Devices.Usb.USBUSER_CONTROLLER_INFO_0();
            int                        dwError = 0;
            int                        dwBytes = 0;
            bool                       bSuccess = false;

            // set the header and request sizes
            int sizeUsbControllerInfo = Marshal.SizeOf(typeof(Windows.Win32.Devices.Usb.USBUSER_CONTROLLER_INFO_0));

            UsbControllerInfo.Header.UsbUserRequest = PInvoke.USBUSER_GET_CONTROLLER_INFO_0;
            UsbControllerInfo.Header.RequestBufferLength = (uint)sizeUsbControllerInfo;

            bSuccess = PInvoke.DeviceIoControl(hHCDev, PInvoke.IOCTL_USB_USER_REQUEST, ref UsbControllerInfo, sizeUsbControllerInfo, out UsbControllerInfo, sizeUsbControllerInfo, out dwBytes, IntPtr.Zero);

            if ( !bSuccess ) {
                dwError = Marshal.GetLastWin32Error();
                // HandleNativeFailure();
            } else {
                // hcInfo.ControllerInfo = UsbControllerInfo.Info0;
            }
            // return dwError;
        }
    }
}
