using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.Ui {
    public static class DeviceNameUtil {
        public static string GetFriendlyUsbHostControllerName(UsbHostControllerInfo usbHostControllerInfo) {
            // @TODO: Prettier selector
            return usbHostControllerInfo.UsbDeviceProperties.DeviceDesc;
        }
    }
}
