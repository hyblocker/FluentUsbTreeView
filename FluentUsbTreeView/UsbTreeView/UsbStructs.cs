using FluentUsbTreeView.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FluentUsbTreeView.PInvoke.UsbApi;

namespace FluentUsbTreeView {

    public struct USB_DEVICE_PNP_STRINGS {
        public string DeviceId;
        public string DeviceDesc;
        public string HwId;
        public string Service;
        public string DeviceClass;
        public CM_POWER_DATA PowerState;
    }

    public class USBHOSTCONTROLLERINFO {
        public DeviceInfoType                           DeviceInfoType;
        public LinkedListNode<USBHOSTCONTROLLERINFO>    ListEntry;
        public string                                   DriverKey;
        public uint                                     VendorID;
        public uint                                     DeviceID;
        public uint                                     SubSysID;
        public UsbApi.USB_DEVICE_SPEED                  Revision;
        public UsbApi.USB_POWER_INFO[]                  USBPowerInfo;
        public bool                                     BusDeviceFunctionValid;
        public uint                                     BusNumber;
        public ushort                                   BusDevice;
        public ushort                                   BusFunction;
        public UsbApi.USB_CONTROLLER_INFO_0             ControllerInfo;
        public UsbApi.USB_BANDWIDTH_INFO                BandwidthInfo;
        public USB_DEVICE_PNP_STRINGS                   UsbDeviceProperties;

        public USBHOSTCONTROLLERINFO() {
            this.DeviceInfoType = DeviceInfoType.HostController;
            this.ListEntry = null;
            this.DriverKey = null;
            this.VendorID = 0;
            this.DeviceID = 0;
            this.SubSysID = 0;
            this.Revision = UsbApi.USB_DEVICE_SPEED.UsbLowSpeed;
            this.USBPowerInfo = new UsbApi.USB_POWER_INFO[6];
            for (int i = 0; i < 6; i++) {
                this.USBPowerInfo[i] = new UsbApi.USB_POWER_INFO();
            }
            this.BusDeviceFunctionValid = false;
            this.BusNumber = 0;
            this.BusDevice = 0;
            this.BusFunction = 0;
            this.ControllerInfo = new UsbApi.USB_CONTROLLER_INFO_0();
            this.UsbDeviceProperties = new USB_DEVICE_PNP_STRINGS();
        }
    }
}
