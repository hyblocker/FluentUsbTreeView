using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.PInvoke {
    public static class UsbApi {

        public static readonly Guid GUID_CLASS_USB_HOST_CONTROLLER = new Guid(0x3abf6f2d, 0x71c4, 0x462a, 0x8a, 0x92, 0x1e, 0x68, 0x61, 0xe6, 0xaf, 0x27);

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
            public bool               CanWakeup;
            public bool               IsPowered;
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
            public uint RequestBufferLength;
            public uint ActualBufferLength;
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
