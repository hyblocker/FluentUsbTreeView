using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FluentUsbTreeView.PInvoke {

    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DEVINFO_DATA {
        public Int32 cbSize;
        public Guid ClassGuid;
        public UInt32 DevInst;
        public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DEVICE_INTERFACE_DATA {
        public  Int32    cbSize;
        public  Guid     interfaceClassGuid;
        public  Int32    flags;
        private UIntPtr  reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SP_DEVICE_INTERFACE_DETAIL_DATA {
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DevicePath;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DEVPROPKEY {
        public Guid fmtid;
        public uint pid;

        public DEVPROPKEY(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k, uint pid) {
            this.pid = pid;
            this.fmtid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }
    }

    /// <summary>
    /// An SP_DRVINFO_DATA structure contains information about a driver.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DRVINFO_DATA_V1 {
        public int cbSize;
        public int DriverType;
        private IntPtr Reserved;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string MfgName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string ProviderName;
    }

    /// <summary>
    /// An SP_DRVINFO_DATA structure contains information about a driver.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DRVINFO_DATA_V2 {
        public int cbSize;
        public int DriverType;
        private IntPtr Reserved;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string MfgName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string ProviderName;
        public System.Runtime.InteropServices.ComTypes.FILETIME DriverDate;
        public long DriverVersion;
    }

    /// <summary>
    /// Flags controlling what is included in the device information set built by SetupDiGetClassDevs
    /// </summary>
    [Flags]
    public enum DIGCF : int {
        DIGCF_DEFAULT = 0x00000001,    // only valid with DIGCF_DEVICEINTERFACE
        DIGCF_PRESENT = 0x00000002,
        DIGCF_ALLCLASSES = 0x00000004,
        DIGCF_PROFILE = 0x00000008,
        DIGCF_DEVICEINTERFACE = 0x00000010,
    }

    /// <summary>
    /// Flags for SetupDiGetDeviceRegistryProperty().
    /// </summary>
    public enum DevRegProperty : uint {
        SPDRP_DEVICEDESC                    = 0x00000000, // DeviceDesc (R/W)
        SPDRP_HARDWAREID                    = 0x00000001, // HardwareID (R/W)
        SPDRP_COMPATIBLEIDS                 = 0x00000002, // CompatibleIDs (R/W)
        SPDRP_UNUSED0                       = 0x00000003, // unused
        SPDRP_SERVICE                       = 0x00000004, // Service (R/W)
        SPDRP_UNUSED1                       = 0x00000005, // unused
        SPDRP_UNUSED2                       = 0x00000006, // unused
        SPDRP_CLASS                         = 0x00000007, // Class (R--tied to ClassGUID)
        SPDRP_CLASSGUID                     = 0x00000008, // ClassGUID (R/W)
        SPDRP_DRIVER                        = 0x00000009, // Driver (R/W)
        SPDRP_CONFIGFLAGS                   = 0x0000000A, // ConfigFlags (R/W)
        SPDRP_MFG                           = 0x0000000B, // Mfg (R/W)
        SPDRP_FRIENDLYNAME                  = 0x0000000C, // FriendlyName (R/W)
        SPDRP_LOCATION_INFORMATION          = 0x0000000D, // LocationInformation (R/W)
        SPDRP_PHYSICAL_DEVICE_OBJECT_NAME   = 0x0000000E, // PhysicalDeviceObjectName (R)
        SPDRP_CAPABILITIES                  = 0x0000000F, // Capabilities (R)
        SPDRP_UI_NUMBER                     = 0x00000010, // UiNumber (R)
        SPDRP_UPPERFILTERS                  = 0x00000011, // UpperFilters (R/W)
        SPDRP_LOWERFILTERS                  = 0x00000012, // LowerFilters (R/W)
        SPDRP_BUSTYPEGUID                   = 0x00000013, // BusTypeGUID (R)
        SPDRP_LEGACYBUSTYPE                 = 0x00000014, // LegacyBusType (R)
        SPDRP_BUSNUMBER                     = 0x00000015, // BusNumber (R)
        SPDRP_ENUMERATOR_NAME               = 0x00000016, // Enumerator Name (R)
        SPDRP_SECURITY                      = 0x00000017, // Security (R/W, binary form)
        SPDRP_SECURITY_SDS                  = 0x00000018, // Security (W, SDS form)
        SPDRP_DEVTYPE                       = 0x00000019, // Device Type (R/W)
        SPDRP_EXCLUSIVE                     = 0x0000001A, // Device is exclusive-access (R/W)
        SPDRP_CHARACTERISTICS               = 0x0000001B, // Device Characteristics (R/W)
        SPDRP_ADDRESS                       = 0x0000001C, // Device Address (R)
        SPDRP_UI_NUMBER_DESC_FORMAT         = 0X0000001D, // UiNumberDescFormat (R/W)
        SPDRP_DEVICE_POWER_DATA             = 0x0000001E, // Device Power Data (R)
        SPDRP_REMOVAL_POLICY                = 0x0000001F, // Removal Policy (R)
        SPDRP_REMOVAL_POLICY_HW_DEFAULT     = 0x00000020, // Hardware Removal Policy (R)
        SPDRP_REMOVAL_POLICY_OVERRIDE       = 0x00000021, // Removal Policy Override (RW)
        SPDRP_INSTALL_STATE                 = 0x00000022, // Device Install State (R)
        SPDRP_LOCATION_PATHS                = 0x00000023, // Device Location Paths (R)
        SPDRP_BASE_CONTAINERID              = 0x00000024  // Base ContainerID (R)
    }

    public enum INTERFACE_TYPE : int {
        InterfaceTypeUndefined,
        Internal,
        Isa,
        Eisa,
        MicroChannel,
        TurboChannel,
        PCIBus,
        VMEBus,
        NuBus,
        PCMCIABus,
        CBus,
        MPIBus,
        MPSABus,
        ProcessorInternal,
        InternalPowerBus,
        PNPISABus,
        PNPBus,
        Vmcs,
        ACPIBus,
        MaximumInterfaceType
    }

    [Flags]
    public enum CM_DEVCAP : int {
        CM_DEVCAP_LOCKSUPPORTED     = (0x00000001),
        CM_DEVCAP_EJECTSUPPORTED    = (0x00000002),
        CM_DEVCAP_REMOVABLE         = (0x00000004),
        CM_DEVCAP_DOCKDEVICE        = (0x00000008),
        CM_DEVCAP_UNIQUEID          = (0x00000010),
        CM_DEVCAP_SILENTINSTALL     = (0x00000020),
        CM_DEVCAP_RAWDEVICEOK       = (0x00000040),
        CM_DEVCAP_SURPRISEREMOVALOK = (0x00000080),
        CM_DEVCAP_HARDWAREDISABLED  = (0x00000100),
        CM_DEVCAP_NONDYNAMIC        = (0x00000200),
        CM_DEVCAP_SECUREDEVICE      = (0x00000400),
    }

    public static class SetupApi {

        public static readonly DEVPROPKEY DEVPKEY_Device_DriverProblemDesc = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 11);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, ulong MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid ClassGuid,
            [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
            IntPtr hwndParent,
            DIGCF Flags
        );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(
            IntPtr ClassGuid,
            [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
            IntPtr hwndParent,
            DIGCF Flags
        );
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           ref UInt32 requiredSize,
           ref SP_DEVINFO_DATA deviceInfoData
        );


        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           ref UInt32 requiredSize,
           IntPtr deviceInfoData
        );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           IntPtr deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           ref UInt32 requiredSize,
           IntPtr deviceInfoData
        );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
            IntPtr hDevInfo,
            IntPtr devInfo,
            ref Guid interfaceClassGuid,
            UInt32 memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
            IntPtr hDevInfo,
            ref SP_DEVINFO_DATA devInfo,
            ref Guid interfaceClassGuid,
            UInt32 memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            DevRegProperty property,
            out UInt32 propertyRegDataType,
            IntPtr propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize
        );
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            DevRegProperty property,
            out UInt32 propertyRegDataType,
            StringBuilder propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize
        );
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            DevRegProperty property,
            out UInt32 propertyRegDataType,
            out Int32 propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize
        );
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            DEVPROPKEY property,
            out UInt32 propertyRegDataType,
            IntPtr propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize
        );
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            DEVPROPKEY property,
            out UInt32 propertyRegDataType,
            StringBuilder propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize
        );
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            DevRegProperty property,
            out UInt32 propertyRegDataType,
            byte[] propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize
        );

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInstanceId(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            StringBuilder DeviceInstanceId,
            int DeviceInstanceIdSize,
            out int RequiredSize
        );
    }
}
