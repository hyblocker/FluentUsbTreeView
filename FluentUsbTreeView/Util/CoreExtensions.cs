using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FluentUsbTreeView.PInvoke.UsbApi;

namespace FluentUsbTreeView {
    public static class CoreExtensions {
        public static T Clone<T>(this T val) where T : struct => val;

        public static string CompanionHubSymbolicLinkName(this USB_PORT_CONNECTOR_PROPERTIES props) {
            return Marshal.PtrToStringAuto(props.__ptr__CompanionHubSymbolicLinkName);
        }
    }
}
