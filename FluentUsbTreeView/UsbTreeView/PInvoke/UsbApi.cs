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

        public const int USB_DEVICE_CLASS_VIDEO = 0x0E;

        public const int MAXIMUM_USB_STRING_LENGTH = 255;


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
            USB_HcGeneric           = 0,
            OHCI_Generic            = 100,
            OHCI_Hydra              = 101,
            OHCI_NEC                = 102,
            UHCI_Generic            = 200,
            UHCI_Piix4              = 201,
            UHCI_Piix3              = 202,
            UHCI_Ich2               = 203,
            UHCI_Reserved204        = 204,
            UHCI_Ich1               = 205,
            UHCI_Ich3m              = 206,
            UHCI_Ich4               = 207,
            UHCI_Ich5               = 208,
            UHCI_Ich6               = 209,
            UHCI_Intel              = 249,
            UHCI_VIA                = 250,
            UHCI_VIA_x01            = 251,
            UHCI_VIA_x02            = 252,
            UHCI_VIA_x03            = 253,
            UHCI_VIA_x04            = 254,
            UHCI_VIA_x0E_FIFO       = 264,
            EHCI_Generic            = 1000,
            EHCI_NEC                = 2000,
            EHCI_Lucent             = 3000,
            EHCI_NVIDIA_Tegra2      = 4000,
            EHCI_NVIDIA_Tegra3      = 4001,
            EHCI_Intel_Medfield     = 5001,
        };

        /// <summary>The USB_USER_ERROR_CODE enumeration lists the error codes that a USB user-mode request reports when it fails.</summary>
        /// <remarks>
        /// <para><see href="https://learn.microsoft.com/windows/win32/api/usbuser/ne-usbuser-usb_user_error_code">Learn more about this API from docs.microsoft.com</see>.</para>
        /// </remarks>
        public enum USB_USER_ERROR_CODE {
            /// <summary>The user request succeeded.</summary>
            UsbUserSuccess                  = 0,
            /// <summary>The user request was not supported.</summary>
            UsbUserNotSupported             = 1,
            /// <summary>The user request code was invalid.</summary>
            UsbUserInvalidRequestCode       = 2,
            /// <summary>The feature that was specified by user request is disabled.</summary>
            UsbUserFeatureDisabled          = 3,
            /// <summary>The user request contains an invalid header parameter.</summary>
            UsbUserInvalidHeaderParameter   = 4,
            /// <summary>The user request contains an invalid parameter.</summary>
            UsbUserInvalidParameter         = 5,
            /// <summary>The user request failed because of a miniport driver error.</summary>
            UsbUserMiniportError            = 6,
            /// <summary>The user request failed because the data buffer was too small.</summary>
            UsbUserBufferTooSmall           = 7,
            /// <summary>The USB stack could not map the error to one of the errors that are listed in this enumeration.</summary>
            UsbUserErrorNotMapped           = 8,
            /// <summary>The device was not started.</summary>
            UsbUserDeviceNotStarted         = 9,
            /// <summary>The device was not connected.</summary>
            UsbUserNoDeviceConnected        = 10,
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

        [Flags]
        public enum PDCAP_CAPABILITIES : uint {
            PDCAP_NONE                      = 0x00000000,
            PDCAP_D0_SUPPORTED              = 0x00000001,
            PDCAP_D1_SUPPORTED              = 0x00000002,
            PDCAP_D2_SUPPORTED              = 0x00000004,
            PDCAP_D3_SUPPORTED              = 0x00000008,
            PDCAP_WAKE_FROM_D0_SUPPORTED    = 0x00000010,
            PDCAP_WAKE_FROM_D1_SUPPORTED    = 0x00000020,
            PDCAP_WAKE_FROM_D2_SUPPORTED    = 0x00000040,
            PDCAP_WAKE_FROM_D3_SUPPORTED    = 0x00000080,
            PDCAP_WARM_EJECT_SUPPORTED      = 0x00000100,
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
            USB_IAD_DESCRIPTOR_TYPE                                     = 0x0B,
            USB_INTERFACE_ASSOCIATION_DESCRIPTOR_TYPE                   = 0x0B,
            USB_BOS_DESCRIPTOR_TYPE                                     = 0x0F,
            USB_DEVICE_CAPABILITY_DESCRIPTOR_TYPE                       = 0x10,
            USB_SUPERSPEED_ENDPOINT_COMPANION_DESCRIPTOR_TYPE           = 0x30,
            // USB 3.1: 9.4 Standard Device Requests, Table 9-6. Descriptor Types
            USB_SUPERSPEEDPLUS_ISOCH_ENDPOINT_COMPANION_DESCRIPTOR_TYPE = 0x31,
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
            public byte    bLength;
            public byte    bDescriptorType;
            public byte    bEndpointAddress;
            public byte    bmAttributes;
            public ushort  wMaxPacketSize;
            public byte    bInterval;
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

        public const int HUB_CHAR_LPSM		        = 0x0003; /* Logical Power Switching Mode mask */
        public const int HUB_CHAR_COMMON_LPSM	    = 0x0000; /* All ports power control at once */
        public const int HUB_CHAR_INDV_PORT_LPSM	= 0x0001; /* per-port power control */
        public const int HUB_CHAR_NO_LPSM	        = 0x0002; /* no power switching */
        public const int HUB_CHAR_COMPOUND	        = 0x0004; /* hub is part of a compound device */
        public const int HUB_CHAR_OCPM		        = 0x0018; /* Over-Current Protection Mode mask */
        public const int HUB_CHAR_COMMON_OCPM	    = 0x0000; /* All ports Over-Current reporting */
        public const int HUB_CHAR_INDV_PORT_OCPM	= 0x0008; /* per-port Over-current reporting */
        public const int HUB_CHAR_NO_OCPM	        = 0x0010; /* No Over-current Protection support */
        public const int HUB_CHAR_TTTT		        = 0x0060; /* TT Think Time mask */
        public const int HUB_CHAR_PORTIND	        = 0x0080; /* per-port indicators (LEDs) */

        // USB 1.1: 11.15.2.1 Hub Descriptor, Table 11-8. Hub Descriptor
        // USB 2.0: 11.23.2.1 Hub Descriptor, Table 11-13. Hub Descriptor
        // https://manuais.iessanclemente.net/images/b/bc/USB_3_1_r1.0.pdf
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 71)]
        public unsafe struct USB_HUB_DESCRIPTOR {
            public byte bDescriptorLength;
            public byte bDescriptorType;
            public byte bNumberOfPorts;
            /// <summary>
            /// D1...D0: Logical Power Switching Mode
            ///     00: Ganged power switching (all ports’ power at
            ///         once)
            ///     01: Individual port power switching
            ///     1X: Reserved. Used only on 1.0 compliant hubs
            ///         that implement no power switching
            /// D2: Identifies a Compound Device
            ///     0: Hub is not part of a compound device.
            ///     1: Hub is part of a compound device.
            /// D4...D3: Over-current Protection Mode
            ///     00: Global Over-current Protection. The hub
            ///         reports over-current as a summation of all
            ///         ports’ current draw, without a breakdown of
            ///         individual port over-current status.
            ///     01: Individual Port Over-current Protection. The
            ///         hub reports over-current on a per-port basis.
            ///         Each port has an over-current status.
            ///     1X: No Over-current Protection. This option is
            ///         allowed only for bus-powered hubs that do not
            ///         implement over-current protection.
            /// D6...D5: TT Think TIme
            ///     00: TT requires at most 8 FS bit times of inter
            ///         transaction gap on a full-/low-speed
            ///         downstream bus.
            ///     01: TT requires at most 16 FS bit times.
            ///     10: TT requires at most 24 FS bit times.
            ///     11: TT requires at most 32 FS bit times.
            /// D7: Port Indicators Supported
            ///     0: Port Indicators are not supported on its
            ///        downstream facing ports and the
            ///        PORT_INDICATOR request has no effect.
            ///     1: Port Indicators are supported on its
            ///        downstream facing ports and the
            ///        PORT_INDICATOR request controls the
            ///        indicators.See Section 11.5.3.
            /// D15...D8: Reserved
            /// </summary>
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
        public const ushort USB_ENDPOINT_DIRECTION_MASK                 = 0x80;
        public static bool USB_ENDPOINT_DIRECTION_OUT(uint addr)        { return (addr & USB_ENDPOINT_DIRECTION_MASK) == 0;}
        public static bool USB_ENDPOINT_DIRECTION_IN(uint addr)         { return ( addr & USB_ENDPOINT_DIRECTION_MASK ) != 0; }

        public const ushort USB_ENDPOINT_ADDRESS_MASK                   = 0x0F;


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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_SETUP_PACKET {
            public byte bmRequest;
            public byte bRequest;
            public ushort wValue;
            public short wIndex;
            public ushort wLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_DESCRIPTOR_REQUEST {
            public uint ConnectionIndex;
            public USB_SETUP_PACKET SetupPacket;
            // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.I1 )]
            // public byte[] Data;
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_INTERFACE_DESCRIPTOR {
            public byte bLength;
            public byte bDescriptorType;
            public byte bInterfaceNumber;
            public byte bAlternateSetting;
            public byte bNumEndpoints;
            public byte bInterfaceClass;
            public byte bInterfaceSubClass;
            public byte bInterfaceProtocol;
            public byte iInterface;
        }

        // Common Class Interface Descriptor
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_INTERFACE_DESCRIPTOR2 {
            public byte     bLength;             // offset 0, size 1
            public byte     bDescriptorType;     // offset 1, size 1
            public byte     bInterfaceNumber;    // offset 2, size 1
            public byte     bAlternateSetting;   // offset 3, size 1
            public byte     bNumEndpoints;       // offset 4, size 1
            public byte     bInterfaceClass;     // offset 5, size 1
            public byte     bInterfaceSubClass;  // offset 6, size 1
            public byte     bInterfaceProtocol;  // offset 7, size 1
            public byte     iInterface;          // offset 8, size 1
            public ushort   wNumClasses;         // offset 9, size 2
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct USB_STRING_DESCRIPTOR {
            public byte bLength;
            public USB_DESCRIPTOR_TYPE bDescriptorType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_USB_STRING_LENGTH, ArraySubType = UnmanagedType.U1 )]
            public byte[] bString;
            // public IntPtr bString;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public unsafe struct USB_STRING_DESCRIPTOR_LANG {
            public byte bLength;
            public USB_DESCRIPTOR_TYPE bDescriptorType;
            public fixed ushort bString[MAXIMUM_USB_STRING_LENGTH / 2];
        }

        // IAD Descriptor
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct USB_IAD_DESCRIPTOR {
            public byte bLength;
            public byte bDescriptorType;
            public byte bFirstInterface;
            public byte bInterfaceCount;
            public byte bFunctionClass;
            public byte bFunctionSubClass;
            public byte bFunctionProtocol;
            public byte iFunction;
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
            public uint                 PD_Size;
            [FieldOffset(4)]
            public DEVICE_POWER_STATE   PD_MostRecentPowerState;
            [FieldOffset(8)]
            public PDCAP_CAPABILITIES   PD_Capabilities;
            [FieldOffset(12)]
            public uint                 PD_D1Latency;
            [FieldOffset(16)]
            public uint                 PD_D2Latency;
            [FieldOffset(20)]
            public uint                 PD_D3Latency;
            [FieldOffset(24)]
            [MarshalAs(UnmanagedType.ByValArray,SizeConst = 7)] /* POWER_SYSTEM_MAXIMUM */
            public DEVICE_POWER_STATE[] PD_PowerStateMapping;
            [FieldOffset(52)]
            public SYSTEM_POWER_STATE   PD_DeepestSystemWake;
        }
        /// <summary>The USB_CONTROLLER_INFO_0 structure is used with the IOCTL_USB_USER_REQUEST I/O control request to retrieve information about the USB host controller.</summary>
		/// <remarks>The <b>USB_CONTROLLER_INFO_0</b> structure is used with the USBUSER_GET_CONTROLLER_INFO_0 user-mode request. For a description of this request, see <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a>.</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_CONTROLLER_INFO_0 {
            /// <summary>The vendor identifier that is associated with the host controller device.</summary>
            public uint PciVendorId;

            /// <summary>The device identifier that is associated with the host controller.</summary>
            public uint PciDeviceId;

            /// <summary>The revision number of the host controller device.</summary>
            public uint PciRevision;

            /// <summary>
            /// <para>The number of root hub ports that the host controller has. <div class="alert"><b>Note</b>  In Windows 8, the USB 3.0 driver stack does not include the number of SuperSpeed hubs in the reported <b>NumberOfRootPorts</b> value.</div> <div> </div></para>
            /// <para><see href="https://learn.microsoft.com/windows/win32/api/usbuser/ns-usbuser-usb_controller_info_0#members">Read more on docs.microsoft.com</see>.</para>
            /// </summary>
            public uint NumberOfRootPorts;

            /// <summary>A <a href="https://docs.microsoft.com/windows-hardware/drivers/ddi/content/usb/ne-usb-_usb_controller_flavor">USB_CONTROLLER_FLAVOR</a>-typed enumerator  that specifies the type of controller.</summary>
            public USB_CONTROLLER_FLAVOR ControllerFlavor;

            /// <summary>
            /// <para>A bitwise OR of some combination of the following host controller feature flags. </para>
            /// <para>This doc was truncated.</para>
            /// <para><see href="https://learn.microsoft.com/windows/win32/api/usbuser/ns-usbuser-usb_controller_info_0#members">Read more on docs.microsoft.com</see>.</para>
            /// </summary>
            public uint HcFeatureFlags;
        }

        /// <summary>The USBUSER_REQUEST_HEADER structure is used with the IOCTL_USB_USER_REQUEST I/O control request to send a user-mode request to the USB host controller driver.</summary>
		/// <remarks>The <b>USBUSER_REQUEST_HEADER</b> structure is used with the <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a> I/O control request to send a user-mode request to the USB port driver.</remarks>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USBUSER_REQUEST_HEADER {
            /// <summary>The user-mode request. For a list and description of possible values for this member, see <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a>.</summary>
            public uint UsbUserRequest;

            /// <summary>The status code that is returned by port driver.</summary>
            public USB_USER_ERROR_CODE UsbUserStatusCode;

            /// <summary>The size, in bytes, of the data buffer. The same buffer is used for both input and output.</summary>
            public uint RequestBufferLength;

            /// <summary>The size, in bytes, of the data that is retrieved by the request.</summary>
            public uint ActualBufferLength;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USBUSER_POWER_INFO_REQUEST {
            public USBUSER_REQUEST_HEADER Header;
            public USB_POWER_INFO         PowerInformation;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USBUSER_CONTROLLER_INFO_0 {
            public USBUSER_REQUEST_HEADER Header;
            public USB_CONTROLLER_INFO_0 Info0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USBUSER_BANDWIDTH_INFO_REQUEST {
            public USBUSER_REQUEST_HEADER Header;
            public USB_BANDWIDTH_INFO BandwidthInformation;
        }

        /// <summary>The USB_DRIVER_VERSION_PARAMETERS structure is used with the IOCTL_USB_USER_REQUEST I/O control request to retrieve version information.</summary>
        /// <remarks>The <b>USB_DRIVER_VERSION_PARAMETERS</b> structure is used with the USBUSER_GET_USB_DRIVER_VERSION user-mode request. For a description of this request, see <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a>.</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_DRIVER_VERSION_PARAMETERS {
            /// <summary>A tracking code that identifies the revision of the USB stack.</summary>
            public uint DriverTrackingCode;

            /// <summary>The version of the USB driver interface that the USB stack supports.</summary>
            public uint USBDI_Version;

            /// <summary>The version of the USB user interface that the USB stack supports.</summary>
            public uint USBUSER_Version;

            /// <summary>A Boolean value that indicates whether the checked version of the host controller driver is loaded. If <b>TRUE</b>, the checked version of the host controller driver is loaded. If <b>FALSE</b>, the checked version is not loaded.</summary>
            public byte bCheckedPortDriver;

            /// <summary>A Boolean value that indicates whether the checked version of the host controller miniport driver is loaded. If <b>TRUE</b>, the checked version of the host controller miniport driver is loaded. If <b>FALSE</b>, the checked version is not loaded.</summary>
            public byte bCheckedMiniportDriver;

            /// <summary>The USB version that the USB stack supports. A value of 0x0110 indicates that the USB stack supports version 1.1. A value of 0x0200 indicates the USB stack supports version 2.0.</summary>
            public ushort USB_Version;
        }

        /// <summary>The USBUSER_GET_DRIVER_VERSION structure is used with the IOCTL_USB_USER_REQUEST I/O control request to read driver and interface version information.</summary>
        /// <remarks>The <b>USBUSER_GET_DRIVER_VERSION</b> structure is used with the USBUSER_GET_USB_DRIVER_VERSION user-mode request. For more information about this request, see <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a>.</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USBUSER_GET_DRIVER_VERSION {
            /// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ns-usbuser-usbuser_request_header">USBUSER_REQUEST_HEADER</a> structure that specifies the user-mode request on input to <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a> and provides buffer and status information on output.</summary>
            public USBUSER_REQUEST_HEADER Header;

            /// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ns-usbuser-usb_driver_version_parameters">USB_DRIVER_VERSION_PARAMETERS</a> structure that specifies the parameters that are associated with this request.</summary>
            public USB_DRIVER_VERSION_PARAMETERS Parameters;
        }

        /// <summary>The USBUSER_BUS_STATISTICS_0_REQUEST structure is used with the IOCTL_USB_USER_REQUEST I/O control request to retrieve bus statistics.</summary>
		/// <remarks>The <b>USBUSER_BUS_STATISTICS_0_REQUEST</b> structure is used with the USBUSER_GET_BUS_STATISTICS_0 user-mode request. For more information about this request, see <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a>.</remarks>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USBUSER_BUS_STATISTICS_0_REQUEST {
            /// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ns-usbuser-usbuser_request_header">USBUSER_REQUEST_HEADER</a> structure that specifies the user-mode request on input to IOCTL_USB_USER_REQUEST and provides buffer and status information on output.</summary>
            public USBUSER_REQUEST_HEADER Header;

            /// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ns-usbuser-usb_bus_statistics_0">USB_BUS_STATISTICS_0</a> structure that reports bus statistics.</summary>
            public USB_BUS_STATISTICS_0 BusStatistics0;
        }

        /// <summary>The USB_BUS_STATISTICS_0 structure is used with the IOCTL_USB_USER_REQUEST I/O control request to retrieve bus statistics.</summary>
		/// <remarks>
		/// <para>The <b>USB_BUS_STATISTICS_0</b> structure is used with the <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ns-usbuser-usbuser_bus_statistics_0_request">USBUSER_BUS_STATISTICS_0</a> user-mode request. For a description of this request, see <a href="https://docs.microsoft.com/windows/desktop/api/usbuser/ni-usbuser-ioctl_usb_user_request">IOCTL_USB_USER_REQUEST</a>. In Windows 8, this request completes successfully. However, the values retrieved from the underlying USB 3.0 driver stack do not reflect actual  bus statistics.</para>
		/// <para><see href="https://learn.microsoft.com/windows/win32/api/usbuser/ns-usbuser-usb_bus_statistics_0#">Read more on docs.microsoft.com</see>.</para>
		/// </remarks>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_BUS_STATISTICS_0 {
            /// <summary>The number of devices on the bus.</summary>
            public uint DeviceCount;

            /// <summary>The current system time.</summary>
            public long CurrentSystemTime;

            /// <summary>The number of the current USB frame.</summary>
            public uint CurrentUsbFrame;

            /// <summary>The amount, in bytes, of bulk transfer data.</summary>
            public uint BulkBytes;

            /// <summary>The amount, in bytes, of isochronous data.</summary>
            public uint IsoBytes;

            /// <summary>The amount, in bytes, of interrupt data.</summary>
            public uint InterruptBytes;

            /// <summary>The amount, in bytes, of control data.</summary>
            public uint ControlDataBytes;

            /// <summary>The amount, in bytes, of interrupt data.</summary>
            public uint PciInterruptCount;

            /// <summary>The number of hard bus resets that have occurred.</summary>
            public uint HardResetCount;

            /// <summary>The number of times that a worker thread has signaled completion of a task.</summary>
            public uint WorkerSignalCount;

            /// <summary>The number of bytes that are transferred by common buffer.</summary>
            public uint CommonBufferBytes;

            /// <summary>The amount of time, in milliseconds, that worker threads have been idle.</summary>
            public uint WorkerIdleTimeMs;

            /// <summary>A Boolean value that indicates whether the root hub is enabled. If <b>TRUE</b>, he root hub is enabled. If <b>FALSE</b>, the root hub is disabled.</summary>
            public byte RootHubEnabled;

            /// <summary></summary>
            public byte RootHubDevicePowerState;

            /// <summary>If this member is 1, the bus is active. If 0, the bus is inactive.</summary>
            public byte Unused;

            /// <summary>The index that is used to generate a symbolic link name for the hub PDO. This format of the symbolic link is USBPDO-<i>n</i>, where <i>n</i> is the value in <b>NameIndex</b>.</summary>
            public byte NameIndex;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct USB_CYCLE_PORT_PARAMS {
            public uint ConnectionIndex;
            public uint StatusReturned;
        }
    }
}
