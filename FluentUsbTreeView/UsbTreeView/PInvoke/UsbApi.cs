using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace FluentUsbTreeView.PInvoke {
    public static class UsbApi {

        public const int USBUSER_GET_CONTROLLER_INFO_0      = 0x00000001;
        public const int USBUSER_GET_CONTROLLER_DRIVER_KEY  = 0x00000002;
        public const int USBUSER_PASS_THRU                  = 0x00000003;
        public const int USBUSER_GET_POWER_STATE_MAP        = 0x00000004;
        public const int USBUSER_GET_BANDWIDTH_INFORMATION  = 0x00000005;
        public const int USBUSER_GET_BUS_STATISTICS_0       = 0x00000006;
        public const int USBUSER_GET_ROOTHUB_SYMBOLIC_NAME  = 0x00000007;
        public const int USBUSER_GET_USB_DRIVER_VERSION     = 0x00000008;
        public const int USBUSER_GET_USB2_HW_VERSION        = 0x00000009;
        public const int USBUSER_USB_REFRESH_HCT_REG        = 0x0000000a;

        public const int USB_HC_FEATURE_FLAG_PORT_POWER_SWITCHING   = 0x00000001;
        public const int USB_HC_FEATURE_FLAG_SEL_SUSPEND            = 0x00000002;
        public const int USB_HC_FEATURE_LEGACY_BIOS                 = 0x00000004;
        public const int USB_HC_FEATURE_TIME_SYNC_API               = 0x00000008;

        public const int PDCAP_D0_SUPPORTED              = 0x00000001;
        public const int PDCAP_D1_SUPPORTED              = 0x00000002;
        public const int PDCAP_D2_SUPPORTED              = 0x00000004;
        public const int PDCAP_D3_SUPPORTED              = 0x00000008;
        public const int PDCAP_WAKE_FROM_D0_SUPPORTED    = 0x00000010;
        public const int PDCAP_WAKE_FROM_D1_SUPPORTED    = 0x00000020;
        public const int PDCAP_WAKE_FROM_D2_SUPPORTED    = 0x00000040;
        public const int PDCAP_WAKE_FROM_D3_SUPPORTED    = 0x00000080;
        public const int PDCAP_WARM_EJECT_SUPPORTED      = 0x00000100;

        public const int USB_ENDPOINT_SUPERSPEED_BULK_MAX_PACKET_SIZE       = 1024;
        public const int USB_ENDPOINT_SUPERSPEED_CONTROL_MAX_PACKET_SIZE    =  512;
        public const int USB_ENDPOINT_SUPERSPEED_ISO_MAX_PACKET_SIZE        = 1024;
        public const int USB_ENDPOINT_SUPERSPEED_INTERRUPT_MAX_PACKET_SIZE  = 1024;

        public const int MAXIMUM_USB_STRING_LENGTH = 255;

        //
        // USB 1.1: 9.4 Standard Device Requests, Table 9-5. Descriptor Types
        //
        public const int USB_DEVICE_DESCRIPTOR_TYPE                                     = 0x01;
        public const int USB_CONFIGURATION_DESCRIPTOR_TYPE                              = 0x02;
        public const int USB_STRING_DESCRIPTOR_TYPE                                     = 0x03;
        public const int USB_INTERFACE_DESCRIPTOR_TYPE                                  = 0x04;
        public const int USB_ENDPOINT_DESCRIPTOR_TYPE                                   = 0x05;
        //
        // USB 2.0: 9.4 Standard Device Requests, Table 9-5. Descriptor Types
        //
        public const int USB_DEVICE_QUALIFIER_DESCRIPTOR_TYPE                           = 0x06;
        public const int USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE                  = 0x07;
        public const int USB_INTERFACE_POWER_DESCRIPTOR_TYPE                            = 0x08;
        //
        // USB 3.0: 9.4 Standard Device Requests, Table 9-5. Descriptor Types
        //
        public const int USB_OTG_DESCRIPTOR_TYPE                                        = 0x09;
        public const int USB_DEBUG_DESCRIPTOR_TYPE                                      = 0x0A;
        public const int USB_INTERFACE_ASSOCIATION_DESCRIPTOR_TYPE                      = 0x0B;
        public const int USB_BOS_DESCRIPTOR_TYPE                                        = 0x0F;
        public const int USB_DEVICE_CAPABILITY_DESCRIPTOR_TYPE                          = 0x10;
        public const int USB_SUPERSPEED_ENDPOINT_COMPANION_DESCRIPTOR_TYPE              = 0x30;
        //
        // USB 3.1: 9.4 Standard Device Requests, Table 9-6. Descriptor Types
        //
        public const int USB_SUPERSPEEDPLUS_ISOCH_ENDPOINT_COMPANION_DESCRIPTOR_TYPE    = 0x31;


        public enum USB_DEVICE_SPEED : uint {
            UsbLowSpeed,
            UsbFullSpeed,
            UsbHighSpeed,
            UsbSuperSpeed
        };

        public enum WDMUSB_POWER_STATE : int {
            NotMapped = 0,
            SystemUnspecified = 100,
            SystemWorking,
            SystemSleeping1,
            SystemSleeping2,
            SystemSleeping3,
            SystemHibernate,
            SystemShutdown,
            DeviceUnspecified = 200,
            DeviceD0,
            DeviceD1,
            DeviceD2,
            DeviceD3
        }

        public enum USB_CONNECTION_STATUS {
            NoDeviceConnected,
            DeviceConnected,

            /* failure codes, these map to fail reasons */
            DeviceFailedEnumeration,
            DeviceGeneralFailure,
            DeviceCausedOvercurrent,
            DeviceNotEnoughPower,
            DeviceNotEnoughBandwidth,
            DeviceHubNestedTooDeeply,
            DeviceInLegacyHub,
            DeviceEnumerating,
            DeviceReset
        }

        public enum USB_CONTROLLER_FLAVOR {
            USB_HcGeneric,
            OHCI_Generic,
            OHCI_Hydra,
            OHCI_NEC,
            UHCI_Generic,
            UHCI_Piix4,
            UHCI_Piix3,
            UHCI_Ich2,
            UHCI_Reserved204,
            UHCI_Ich1,
            UHCI_Ich3m,
            UHCI_Ich4,
            UHCI_Ich5,
            UHCI_Ich6,
            UHCI_Intel,
            UHCI_VIA,
            UHCI_VIA_x01,
            UHCI_VIA_x02,
            UHCI_VIA_x03,
            UHCI_VIA_x04,
            UHCI_VIA_x0E_FIFO,
            EHCI_Generic,
            EHCI_NEC,
            EHCI_Lucent,
            EHCI_NVIDIA_Tegra2,
            EHCI_NVIDIA_Tegra3,
            EHCI_Intel_Medfield
        };

        public enum USB_USER_ERROR_CODE {
            UsbUserSuccess = 0,
            UsbUserNotSupported,
            UsbUserInvalidRequestCode,
            UsbUserFeatureDisabled,
            UsbUserInvalidHeaderParameter,
            UsbUserInvalidParameter,
            UsbUserMiniportError,
            UsbUserBufferTooSmall,
            UsbUserErrorNotMapped,
            UsbUserDeviceNotStarted,
            UsbUserNoDeviceConnected
        };

        public enum DEVICE_POWER_STATE {
            PowerDeviceUnspecified,
            PowerDeviceD0,
            PowerDeviceD1,
            PowerDeviceD2,
            PowerDeviceD3,
            PowerDeviceMaximum
        }

        public enum SYSTEM_POWER_STATE {
            PowerSystemUnspecified,
            PowerSystemWorking,
            PowerSystemSleeping1,
            PowerSystemSleeping2,
            PowerSystemSleeping3,
            PowerSystemHibernate,
            PowerSystemShutdown,
            PowerSystemMaximum
        }

        public enum USB_DESCRIPTOR_TYPE : byte {
            // USB 1.1: 9.4 Standard Device Requests, Table 9-5. Descriptor Types
            USB_DEVICE_DESCRIPTOR_TYPE                                  = 0x01,
            USB_CONFIGURATION_DESCRIPTOR_TYPE                           = 0x02,
            USB_STRING_DESCRIPTOR_TYPE                                  = 0x03,
            USB_INTERFACE_DESCRIPTOR_TYPE                               = 0x04,
            USB_ENDPOINT_DESCRIPTOR_TYPE                                = 0x05,
            // USB 2.0: 9.4 Standard Device Requests, Table 9-5. Descriptor Types
            USB_DEVICE_QUALIFIER_DESCRIPTOR_TYPE                        = 0x06,
            USB_OTHER_SPEED_CONFIGURATION_DESCRIPTOR_TYPE               = 0x07,
            USB_INTERFACE_POWER_DESCRIPTOR_TYPE                         = 0x08,
            // USB 3.0: 9.4 Standard Device Requests, Table 9-5. Descriptor Types
            USB_OTG_DESCRIPTOR_TYPE                                     = 0x09,
            USB_DEBUG_DESCRIPTOR_TYPE                                   = 0x0A,
            USB_INTERFACE_ASSOCIATION_DESCRIPTOR_TYPE                   = 0x0B,
            USB_BOS_DESCRIPTOR_TYPE                                     = 0x0F,
            USB_DEVICE_CAPABILITY_DESCRIPTOR_TYPE                       = 0x10,
            USB_SUPERSPEED_ENDPOINT_COMPANION_DESCRIPTOR_TYPE           = 0x30,
            // USB 3.1: 9.4 Standard Device Requests, Table 9-6. Descriptor Types
            USB_SUPERSPEEDPLUS_ISOCH_ENDPOINT_COMPANION_DESCRIPTOR_TYPE = 0x31
        }

        public enum USB_HUB_NODE : uint {
            UsbHub,
            UsbMIParent
        }

        public enum USB_HUB_DESCRIPTOR_TYPE : uint {
            USB_20_HUB_DESCRIPTOR_TYPE = 0x29,
            USB_30_HUB_DESCRIPTOR_TYPE = 0x2A
        }

        public enum USB_HUB_TYPE : uint {
            UsbRootHub        = 1,
            Usb20Hub          = 2,
            Usb30Hub          = 3
        }

        [Flags]
        public enum USB_PROTOCOLS : uint {
            UsbNone = 0b0000,
            Usb110  = 0b0001,
            Usb200  = 0b0010,
            Usb300  = 0b0100,
        }

        [Flags]
        public enum USB_HUB_CAP_FLAGS : uint {
            HubIsNone               = 0b00000000,
            HubIsHighSpeedCapable   = 0b00000001,
            HubIsHighSpeed          = 0b00000010,
            HubIsMultiTtCapable     = 0b00000100,
            HubIsMultiTt            = 0b00001000,
            HubIsRoot               = 0b00010000,
            HubIsArmedWakeOnConnect = 0b00100000,
            HubIsBusPowered         = 0b01000000,
        }

        [Flags]
        public enum USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS : uint {
            DeviceInfoNone                              = 0b0000,
            DeviceIsOperatingAtSuperSpeedOrHigher       = 0b0001,
            DeviceIsSuperSpeedCapableOrHigher           = 0b0010,
            DeviceIsOperatingAtSuperSpeedPlusOrHigher   = 0b0100,
            DeviceIsSuperSpeedPlusCapableOrHigher       = 0b1000,
        }
        [Flags]
        public enum USB_PORT_PROPERTIES : uint {
            PortNone                    = 0b0000,
            PortIsUserConnectable       = 0b0001,
            PortIsDebugCapable          = 0b0010,
            PortHasMultipleCompanions   = 0b0100,
            PortConnectorIsTypeC        = 0b1000,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct USB_PORT_CONNECTOR_PROPERTIES {
            public uint ConnectionIndex;
            public int ActualLength;
            public USB_PORT_PROPERTIES Properties;
            public ushort CompanionIndex;
            public ushort CompanionPortNumber;
            // MICROSOFT WHY DID YOU USE THIS HOW THE FUCK AM I SUPPOSED TO MARSHAL THIS
            // WCHAR CompanionHubSymbolicLinkName[1];
            internal IntPtr __ptr__CompanionHubSymbolicLinkName;
            // public string CompanionHubSymbolicLinkName { get { return Marshal.PtrToStringAnsi(__ptr__CompanionHubSymbolicLinkName); } }
        }

        public const int SIZE_USB_PORT_CONNECTOR_PROPERTIES = 18;

        // USB 1.1: 9.6.4 Endpoint, Table 9-10. Standard Endpoint Descriptor
        // USB 2.0: 9.6.6 Endpoint, Table 9-13. Standard Endpoint Descriptor
        // USB 3.0: 9.6.6 Endpoint, Table 9-18. Standard Endpoint Descriptor
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct USB_ENDPOINT_DESCRIPTOR {
            byte    bLength;
            byte    bDescriptorType;
            byte    bEndpointAddress;
            byte    bmAttributes;
            ushort  wMaxPacketSize;
            byte    bInterval;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_DEVICE_DESCRIPTOR {
            public byte bLength;
            public byte bDescriptorType;
            public ushort bcdUSB;
            public byte bDeviceClass;
            public byte bDeviceSubClass;
            public byte bDeviceProtocol;
            public byte bMaxPacketSize0;
            public ushort idVendor;
            public ushort idProduct;
            public ushort bcdDevice;
            public byte iManufacturer;
            public byte iProduct;
            public byte iSerialNumber;
            public byte bNumConfigurations;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_NODE_INFORMATION {
            public USB_HUB_NODE NodeType;
            public _UsbNodeUnion u;
        }

        [StructLayout(LayoutKind.Explicit, Size = 72)]
        public struct _UsbNodeUnion {
            [FieldOffset(0)]
            public USB_HUB_INFORMATION HubInformation;
            [FieldOffset(0)]
            public USB_MI_PARENT_INFORMATION MiParentInformation;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_HUB_INFORMATION {
            public USB_HUB_DESCRIPTOR HubDescriptor;
            public byte HubIsBusPowered;
        }

        // USB 1.1: 11.15.2.1 Hub Descriptor, Table 11-8. Hub Descriptor
        // USB 2.0: 11.23.2.1 Hub Descriptor, Table 11-13. Hub Descriptor
        // [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Size = 72)]
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 71)]
        public unsafe struct USB_HUB_DESCRIPTOR {
            public byte bDescriptorLength;
            public byte bDescriptorType;
            public byte bNumberOfPorts;
            public ushort wHubCharacteristics;
            public byte bPowerOnToPowerGood;
            public byte bHubControlCurrent;
            public fixed byte bRemoveAndPowerMask[64];
        }

        // USB 3.0: 10.13.2.1 Hub Descriptor, Table 10-3. SuperSpeed Hub Descriptor
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_30_HUB_DESCRIPTOR {
            public byte     bLength;
            public byte     bDescriptorType;
            public byte     bNumberOfPorts;
            public ushort   wHubCharacteristics;
            public byte     bPowerOnToPowerGood;
            public byte     bHubControlCurrent;
            public byte     bHubHdrDecLat;
            public ushort   wHubDelay;
            public ushort   DeviceRemovable;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct USB_HUB_INFORMATION_EX {
            [FieldOffset(0)]
            public USB_HUB_TYPE HubType;
            [FieldOffset(4)]
            public ushort HighestPortNumber;
            // this is a union in the C api
            [FieldOffset(6)]
            public USB_HUB_DESCRIPTOR UsbHubDescriptor;
            [FieldOffset(6)]
            public USB_30_HUB_DESCRIPTOR Usb30HubDescriptor;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_NODE_CONNECTION_NAME {
            public uint ConnectionIndex;
            public int ActualLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
            public string NodeName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_HUB_CAPABILITIES_EX {
            public USB_HUB_CAP_FLAGS CapabilityFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_MI_PARENT_INFORMATION {
            public uint NumberOfInterfaces;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct USB_PIPE_INFO {
            public USB_ENDPOINT_DESCRIPTOR EndpointDescriptor;
            public uint ScheduleOffset;
        }

        // @TODO: PipeList
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public unsafe struct USB_NODE_CONNECTION_INFORMATION_EX {
            public uint ConnectionIndex;
            public USB_DEVICE_DESCRIPTOR DeviceDescriptor;
            public byte CurrentConfigurationValue;
            public USB_DEVICE_SPEED Speed;
            public byte DeviceIsHub;
            public ushort DeviceAddress;
            public uint NumberOfOpenPipes;
            public USB_CONNECTION_STATUS ConnectionStatus;
            public USB_PIPE_INFO* PipeList;
            // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30 )]
            // public USB_PIPE_INFO[] PipeList;
        }
        public const int SIZE_USB_NODE_CONNECTION_INFORMATION_EX = 35;

        // @TODO: PipeList
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Size = 35, Pack = 1)]
        public unsafe struct USB_NODE_CONNECTION_INFORMATION {
            public uint ConnectionIndex;  /* INPUT */
            /* usb device descriptor returned by this device
               during enumeration */
            public USB_DEVICE_DESCRIPTOR DeviceDescriptor; /* OUTPUT */
            public byte CurrentConfigurationValue;/* OUTPUT */
            public byte LowSpeed;/* OUTPUT */
            public byte DeviceIsHub;/* OUTPUT */
            public ushort DeviceAddress;/* OUTPUT */
            public uint NumberOfOpenPipes;/* OUTPUT */
            public USB_CONNECTION_STATUS ConnectionStatus;/* OUTPUT */
            // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30 )]
            public IntPtr PipeList;/* OUTPUT */
        }
        public const int SIZE_USB_NODE_CONNECTION_INFORMATION = 35;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_NODE_CONNECTION_INFORMATION_EX_V2 {
            // one based port number
            public uint  ConnectionIndex;

            // length of the structure
            public uint  Length;

            // On input a bitmask that indicates which USB protocols are understood by the caller
            // On output a bitmask that indicates which USB signaling protocols are supported by the port
            public USB_PROTOCOLS SupportedUsbProtocols;

            // A bitmask indicating properties of the connected device or port
            public USB_NODE_CONNECTION_INFORMATION_EX_V2_FLAGS Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USB_SETUP_PACKET {
            public byte bmRequest;
            public byte bRequest;
            public ushort wValue;
            public ushort wIndex;
            public ushort wLength;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct USB_DESCRIPTOR_REQUEST {
            public uint ConnectionIndex;
            public USB_SETUP_PACKET SetupPacket;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.I1 )]
            public byte[] Data;
        }

        public const int SIZE_USB_DESCRIPTOR_REQUEST = 12;

        // USB 1.1: 9.6.2 Configuration, Table 9-8. Standard Configuration Descriptor
        // USB 2.0: 9.6.3 Configuration, Table 9-10. Standard Configuration Descriptor
        // USB 3.0: 9.6.3 Configuration, Table 9-15. Standard Configuration Descriptor
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_CONFIGURATION_DESCRIPTOR {
            public byte     bLength;
            public byte     bDescriptorType;
            public ushort   wTotalLength;
            public byte     bNumInterfaces;
            public byte     bConfigurationValue;
            public byte     iConfiguration;
            public byte     bmAttributes;
            public byte     MaxPower;
        }

        // USB 3.0: 9.6.2 Binary Device Object Store (BOS), Table 9-9. BOS Descriptor
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_BOS_DESCRIPTOR {
            public byte bLength;
            public byte bDescriptorType;
            public ushort wTotalLength;
            public byte bNumDeviceCaps;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
        public struct USB_COMMON_DESCRIPTOR {
            public byte bLength;
            public USB_DESCRIPTOR_TYPE bDescriptorType;
        }

        // USB 1.1: 9.6.5 String, Table 9-12. UNICODE String Descriptor
        // USB 2.0: 9.6.7 String, Table 9-16. UNICODE String Descriptor
        // USB 3.0: 9.6.8 String, Table 9-22. UNICODE String Descriptor

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_STRING_DESCRIPTOR {
            public byte bLength;
            public byte bDescriptorType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAXIMUM_USB_STRING_LENGTH)]
            public string bString;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_POWER_INFO {
            public WDMUSB_POWER_STATE SystemState;
            public WDMUSB_POWER_STATE HcDevicePowerState;
            public WDMUSB_POWER_STATE HcDeviceWake;
            public WDMUSB_POWER_STATE HcSystemWake;
            public WDMUSB_POWER_STATE RhDevicePowerState;
            public WDMUSB_POWER_STATE RhDeviceWake;
            public WDMUSB_POWER_STATE RhSystemWake;
            public WDMUSB_POWER_STATE LastSystemSleepState;
            public byte               CanWakeup;
            public byte               IsPowered;
        }

        [StructLayout(LayoutKind.Explicit, Size = 56)]
        public struct CM_POWER_DATA {
            [FieldOffset(0)]
            public uint              PD_Size;
            [FieldOffset(4)]
            public DEVICE_POWER_STATE PD_MostRecentPowerState;
            [FieldOffset(8)]
            public uint              PD_Capabilities;
            [FieldOffset(12)]
            public uint              PD_D1Latency;
            [FieldOffset(16)]
            public uint              PD_D2Latency;
            [FieldOffset(20)]
            public uint              PD_D3Latency;
            [FieldOffset(24)]
            [MarshalAs(UnmanagedType.ByValArray,SizeConst = 7)] /* POWER_SYSTEM_MAXIMUM */
            public DEVICE_POWER_STATE[] PD_PowerStateMapping;
            [FieldOffset(52)]
            public SYSTEM_POWER_STATE PD_DeepestSystemWake;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_CONTROLLER_INFO_0 {
            public uint                     PciVendorId;
            public uint                     PciDeviceId;
            public uint                     PciRevision;
            public uint                     NumberOfRootPorts;
            public USB_CONTROLLER_FLAVOR    ControllerFlavor;
            public uint                     HcFeatureFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USBUSER_REQUEST_HEADER {
            public uint UsbUserRequest;
            public USB_USER_ERROR_CODE UsbUserStatusCode;
            public int RequestBufferLength;
            public int ActualBufferLength;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USBUSER_POWER_INFO_REQUEST {
            public USBUSER_REQUEST_HEADER Header;
            public USB_POWER_INFO         PowerInformation;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USBUSER_CONTROLLER_INFO_0 {
            public USBUSER_REQUEST_HEADER Header;
            public USB_CONTROLLER_INFO_0 Info0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_BANDWIDTH_INFO {
            public uint DeviceCount;
            public uint TotalBusBandwidth;
            public uint Total32secBandwidth;
            public uint AllocedBulkAndControl;
            public uint AllocedIso;
            public uint AllocedInterrupt_1ms;
            public uint AllocedInterrupt_2ms;
            public uint AllocedInterrupt_4ms;
            public uint AllocedInterrupt_8ms;
            public uint AllocedInterrupt_16ms;
            public uint AllocedInterrupt_32ms;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USBUSER_BANDWIDTH_INFO_REQUEST {
            public USBUSER_REQUEST_HEADER Header;
            public USB_BANDWIDTH_INFO BandwidthInformation;
        }
    }
}
