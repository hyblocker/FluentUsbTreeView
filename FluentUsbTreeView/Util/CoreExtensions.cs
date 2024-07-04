using FluentUsbTreeView.PInvoke;
using System.Runtime.InteropServices;
using static FluentUsbTreeView.PInvoke.UsbApi;

namespace FluentUsbTreeView {
    public static class CoreExtensions {
        public static T Clone<T>(this T val) where T : struct => val;

        public static string CompanionHubSymbolicLinkName(this USB_PORT_CONNECTOR_PROPERTIES props) {
            return Marshal.PtrToStringAuto(props.__ptr__CompanionHubSymbolicLinkName);
        }

        public static bool GetDirectionIn(this USB_ENDPOINT_DESCRIPTOR endpointDesc) {
            return UsbApi.USB_ENDPOINT_DIRECTION_IN(endpointDesc.bEndpointAddress);
        }
        public static byte GetAddress(this USB_ENDPOINT_DESCRIPTOR endpointDesc) {
            return (byte) (endpointDesc.bEndpointAddress & UsbApi.USB_ENDPOINT_ADDRESS_MASK);
            ;
        }
    }
}
