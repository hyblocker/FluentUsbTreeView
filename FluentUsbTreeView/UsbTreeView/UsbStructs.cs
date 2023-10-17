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

    public class DeviceGuidList {
        public IntPtr                           DeviceInfo;
        public LinkedList<DEVICE_INFO_NODE>     ListHead;

        public DeviceGuidList() {
            this.DeviceInfo = Kernel32.INVALID_HANDLE_VALUE;
            this.ListHead = new LinkedList<DEVICE_INFO_NODE>();
        }
    }

    public struct DEVICE_INFO_NODE {
        public IntPtr                              DeviceInfo;
        public SP_DEVINFO_DATA                     DeviceInfoData;
        public SP_DEVICE_INTERFACE_DATA            DeviceInterfaceData;
        public SP_DEVICE_INTERFACE_DETAIL_DATA     DeviceDetailData;
        public string                              DeviceDescName;
        public string                              DeviceDriverName;
        public DEVICE_POWER_STATE                  LatestDevicePowerState;
    }


    /// <summary>
    /// Device Manager DevRegProperty properties
    /// </summary>
    public class USB_DEVICE_PNP_STRINGS {
        public string DeviceId;
        public string DeviceDesc;
        public string HwId;
        public string Service;
        public string DeviceClass;
        public string Enumerator;
        public int Address;
        public string ContainerId;
        public string LocationInfo;
        public string LocationPaths;
        public Guid DeviceClassGuid;
        public INTERFACE_TYPE LegacyBusType;
        public CM_POWER_DATA PowerState;
        public CM_DEVCAP Capabilities;
    }

    public class STRING_DESCRIPTOR_NODE {
        public LinkedList<STRING_DESCRIPTOR_NODE> LinkedList;
        public byte                            DescriptorIndex;
        public ushort                          LanguageID;
        public List<USB_STRING_DESCRIPTOR>     StringDescriptor;

        public STRING_DESCRIPTOR_NODE(LinkedList<STRING_DESCRIPTOR_NODE> list) {
            this.LinkedList = list;
            this.StringDescriptor = new List<USB_STRING_DESCRIPTOR>();
        }

        public void Append(STRING_DESCRIPTOR_NODE nextNode) {
            nextNode.LinkedList = this.LinkedList;
            LinkedList.AddLast(nextNode);
        }
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
            for ( int i = 0; i < 6; i++ ) {
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


    public class UsbHubInfoUnion {
        public DeviceInfoType                           DeviceInfoType;
        public USBEXTERNALHUBINFO ExternalHubInfo;
        public USBDEVICEINFO RootHubInfo;
    }

    public struct USBEXTERNALHUBINFO {
        public USB_NODE_INFORMATION                     HubInfo;
        public USB_HUB_INFORMATION_EX?                  HubInfoEx;
        public string                                   HubName;
        public USB_NODE_CONNECTION_INFORMATION_EX?      ConnectionInfo;
        public USB_PORT_CONNECTOR_PROPERTIES?           PortConnectorProps;
        public USB_DESCRIPTOR_REQUEST?                  ConfigDesc;
        public USB_DESCRIPTOR_REQUEST?                  BosDesc;
        public STRING_DESCRIPTOR_NODE                   StringDescs;
        public USB_NODE_CONNECTION_INFORMATION_EX_V2?   ConnectionInfoV2; // NULL if root HUB
        public USB_DEVICE_PNP_STRINGS                   UsbDeviceProperties;
        public DEVICE_INFO_NODE                         DeviceInfoNode;
        public USB_HUB_CAPABILITIES_EX?                 HubCapabilityEx;
        public string                                   DriverKey;
    }


    // HubInfo, HubName may be in USBDEVICEINFOTYPE, so they can be removed
    public struct USBDEVICEINFO {
        public USB_NODE_INFORMATION?                    HubInfo;          // NULL if not a HUB
        public USB_HUB_INFORMATION_EX?                  HubInfoEx;        // NULL if not a HUB
        public string                                   HubName;          // NULL if not a HUB
        public USB_NODE_CONNECTION_INFORMATION_EX?      ConnectionInfo;   // NULL if root HUB
        public USB_PORT_CONNECTOR_PROPERTIES?           PortConnectorProps;
        public USB_DESCRIPTOR_REQUEST?                  ConfigDesc;       // NULL if root HUB
        public USB_DESCRIPTOR_REQUEST?                  BosDesc;          // NULL if root HUB
        public STRING_DESCRIPTOR_NODE                   StringDescs;
        public USB_NODE_CONNECTION_INFORMATION_EX_V2?   ConnectionInfoV2; // NULL if root HUB
        public USB_DEVICE_PNP_STRINGS                   UsbDeviceProperties;
        public DEVICE_INFO_NODE                         DeviceInfoNode;
        public USB_HUB_CAPABILITIES_EX?                 HubCapabilityEx;  // NULL if not a HUB
        public string                                   DriverKey;
    }
}
