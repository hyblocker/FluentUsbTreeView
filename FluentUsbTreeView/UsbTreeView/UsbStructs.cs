using FluentUsbTreeView.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static FluentUsbTreeView.PInvoke.CfgMgr32;
using static FluentUsbTreeView.PInvoke.UsbApi;

namespace FluentUsbTreeView {

    public struct UsbTreeState {
        public uint HostControllers;
        public uint RootHubs;
        public uint ExternalHubs;
        public uint PeripheralDevices;
    }

    public class DeviceGuidList {
        public IntPtr                         DeviceInfo;
        public LinkedList<DeviceInfoNode>     ListHead;

        public DeviceGuidList() {
            this.DeviceInfo = Kernel32.INVALID_HANDLE_VALUE;
            this.ListHead = new LinkedList<DeviceInfoNode>();
        }
    }

    public class USBDEVICEINFO {
        public UsbDeviceInfoType                       DeviceInfoType;
        public string                                  DriverKey;
        public string                                  DevicePath;
        public USB_NODE_INFORMATION                    HubInfo;          // NULL if not a HUB
        public USB_HUB_INFORMATION_EX                  HubInfoEx;        // NULL if not a HUB
        public string                                  HubName;          // NULL if not a HUB
        public USB_NODE_CONNECTION_INFORMATION_EX      ConnectionInfo;   // NULL if root HUB
        public USB_PORT_CONNECTOR_PROPERTIES           PortConnectorProps;
        public USB_CONFIGURATION_DESCRIPTOR?           ConfigDesc;       // NULL if root HUB
        public USB_BOS_DESCRIPTOR?                     BosDesc;          // NULL if root HUB
        public StringDescriptorsCollection             StringDescs;
        public USB_NODE_CONNECTION_INFORMATION_EX_V2?  ConnectionInfoV2; // NULL if root HUB
        public UsbDevicePnpStrings                     UsbDeviceProperties;
        public DeviceInfoNode                          DeviceInfoNode;
        public USB_HUB_CAPABILITIES_EX                 HubCapabilityEx;  // NULL if not a HUB
    }

    public class DeviceInfoNode {
        public IntPtr                              DeviceInfo;
        public SP_DEVINFO_DATA                     DeviceInfoData;
        public SP_DEVICE_INTERFACE_DATA            DeviceInterfaceData;
        public SP_DEVICE_INTERFACE_DETAIL_DATA     DeviceDetailData;
        public string                              DeviceDescName;
        public string                              DeviceDriverName;
        public DEVICE_POWER_STATE                  LatestDevicePowerState;

#if DEBUG
        public override string ToString() {
            return $"({DeviceDescName}) {DeviceDetailData.DevicePath}";
        }
#endif
    }


    /// <summary>
    /// Device Manager DevRegProperty properties
    /// </summary>
    public class UsbDevicePnpStrings {
        public string DevicePath;
        public string DriverKey;
        public string FriendlyName;
        public string Manufacturer;
        public string DeviceId;
        public string Kernel;
        public string Driver;
        public string DriverInf;
        public DateTime DriverDate;
        public string DriverCompany;
        public string DriverVersion;
        public uint VendorID;
        public uint ProductID; // Also DeviceID for PCIe devices
        public uint SubSysID;
        public UsbApi.USB_DEVICE_SPEED Revision;
        public string DeviceDesc;
        public string[] HwId;
        public string Service;
        public string DeviceClass;
        public string Enumerator;
        public int Address;
        public string ContainerId;
        public string LocationInfo;
        public string[] LocationPaths;
        public DN_Status Status;
        public CM_PROB ProblemCode;
        public Guid DeviceClassGuid;
        public INTERFACE_TYPE LegacyBusType;
        public CM_POWER_DATA PowerState;
        public CM_DEVCAP Capabilities;
    }

    public class StringDescriptorsCollection {
        public byte Lang_bLength;
        public USB_DESCRIPTOR_TYPE Lang_bDescriptorType;
        public short[] LanguageIds;
        public List<StringDescriptorNode> Strings;
    }
    public class StringDescriptorNode {
        public byte                     DescriptorIndex;
        public short                   LanguageID;
        public USB_STRING_DESCRIPTOR    StringDescriptor;

        public string GetStringData() {
            string unicodeString = Encoding.Unicode.GetString(StringDescriptor.bString);
            unicodeString = unicodeString.Remove(unicodeString.IndexOf('\0'));
            return unicodeString;
        }

        public override string ToString() {
            return $"[{DescriptorIndex}] ({StringDescriptor.bDescriptorType}) (0x{LanguageID.ToString("X4")}) \"{GetStringData()}\" [{StringDescriptor.bLength}]";
        }
    }

    public class UsbHostControllerInfo {
        public UsbDeviceInfoType                        DeviceInfoType;
        public LinkedListNode<UsbHostControllerInfo>    ListEntry;
        // public string                                   DevicePath;
        // public string                                   DriverKey;
        public UsbApi.USB_POWER_INFO[]                  USBPowerInfo;
        public bool                                     BusDeviceFunctionValid;
        public uint                                     BusNumber;
        public ushort                                   BusDevice;
        public ushort                                   BusFunction;
        public UsbApi.USB_CONTROLLER_INFO_0             ControllerInfo;
        public UsbApi.USB_BUS_STATISTICS_0              BusStatistics;
        public string                                   SymbolicLink;
        public UsbApi.USB_BANDWIDTH_INFO                BandwidthInfo;
        public UsbApi.USB_DRIVER_VERSION_PARAMETERS     DriverVersionParams;
        public UsbDevicePnpStrings                      UsbDeviceProperties;

        public UsbHostControllerInfo() {
            this.DeviceInfoType = UsbDeviceInfoType.HostController;
            this.ListEntry = null;
            // this.DriverKey = null;
            this.USBPowerInfo = new UsbApi.USB_POWER_INFO[6];
            for ( int i = 0; i < 6; i++ ) {
                this.USBPowerInfo[i] = new UsbApi.USB_POWER_INFO();
            }
            this.BusDeviceFunctionValid = false;
            this.BusNumber = 0;
            this.BusDevice = 0;
            this.BusFunction = 0;
            this.ControllerInfo = new UsbApi.USB_CONTROLLER_INFO_0();
            this.UsbDeviceProperties = new UsbDevicePnpStrings();
        }
    }


    public class UsbHubInfo {
        public UsbDeviceInfoType                        DeviceInfoType;
        public USB_NODE_INFORMATION?                    HubInfo;          // NULL if not a HUB
        public USB_HUB_INFORMATION_EX?                  HubInfoEx;        // NULL if not a HUB
        public string                                   HubName;          // NULL if not a HUB
        // public string                                   DevicePath;
        public USB_NODE_CONNECTION_INFORMATION_EX?      ConnectionInfo;   // NULL if root HUB
        public USB_PORT_CONNECTOR_PROPERTIES?           PortConnectorProps;
        public USB_CONFIGURATION_DESCRIPTOR?            ConfigDesc;       // NULL if root HUB
        public USB_BOS_DESCRIPTOR?                      BosDesc;          // NULL if root HUB
        public StringDescriptorsCollection              StringDescs;
        public USB_NODE_CONNECTION_INFORMATION_EX_V2?   ConnectionInfoV2; // NULL if root HUB
        public UsbDevicePnpStrings                      UsbDeviceProperties;
        public DeviceInfoNode                           DeviceInfoNode;
        public USB_HUB_CAPABILITIES_EX?                 HubCapabilityEx;  // NULL if not a HUB
        // public string                                   DriverKey;
    }
}
