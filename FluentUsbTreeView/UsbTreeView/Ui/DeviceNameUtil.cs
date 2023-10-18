using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.Ui {
    public static class DeviceNameUtil {

        private static string GetFriendlyName(string friendlyName, string manufacturer, string deviceDesc) {
            // @TODO: Respect settings
            if ( friendlyName != null && friendlyName.Trim().Length > 0 )
                return friendlyName;

            if ( manufacturer != null ) {
                return $"{manufacturer} {deviceDesc}";
            }

            return deviceDesc;
        }

        public static string GetFriendlyUsbHostControllerName(UsbHostControllerInfo usbHostControllerInfo) {
            return GetFriendlyName(usbHostControllerInfo.UsbDeviceProperties.FriendlyName, usbHostControllerInfo.UsbDeviceProperties.Manufacturer, usbHostControllerInfo.UsbDeviceProperties.DeviceDesc);
        }

        public static string GetFriendlyUsbHubName(UsbHubInfo usbHubInfo) {
            return GetFriendlyName(usbHubInfo.UsbDeviceProperties.FriendlyName, usbHubInfo.UsbDeviceProperties.Manufacturer, usbHubInfo.UsbDeviceProperties.DeviceDesc);
        }

        public static string GetFriendlyUsbDeviceName(USBDEVICEINFO usbDeviceInfo) {
            return GetFriendlyName(usbDeviceInfo.UsbDeviceProperties.FriendlyName, usbDeviceInfo.UsbDeviceProperties.Manufacturer, usbDeviceInfo.UsbDeviceProperties.DeviceDesc);
        }
    }
}
