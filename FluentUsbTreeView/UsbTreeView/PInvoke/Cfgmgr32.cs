﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.PInvoke {
    public static class CfgMgr32 {
        public enum CR_RESULT : uint {
            CR_SUCCESS                  = (0x00000000),
            CR_DEFAULT                  = (0x00000001),
            CR_OUT_OF_MEMORY            = (0x00000002),
            CR_INVALID_POINTER          = (0x00000003),
            CR_INVALID_FLAG             = (0x00000004),
            CR_INVALID_DEVNODE          = (0x00000005),
            CR_INVALID_DEVINST          = CR_INVALID_DEVNODE,
            CR_INVALID_RES_DES          = (0x00000006),
            CR_INVALID_LOG_CONF         = (0x00000007),
            CR_INVALID_ARBITRATOR       = (0x00000008),
            CR_INVALID_NODELIST         = (0x00000009),
            CR_DEVNODE_HAS_REQS         = (0x0000000A),
            CR_DEVINST_HAS_REQS         = CR_DEVNODE_HAS_REQS,
            CR_INVALID_RESOURCEID       = (0x0000000B),
            /// <summary>WIN 95 ONLY</summary>
            CR_DLVXD_NOT_FOUND          = (0x0000000C),
            CR_NO_SUCH_DEVNODE          = (0x0000000D),
            CR_NO_SUCH_DEVINST          = CR_NO_SUCH_DEVNODE,
            CR_NO_MORE_LOG_CONF         = (0x0000000E),
            CR_NO_MORE_RES_DES          = (0x0000000F),
            CR_ALREADY_SUCH_DEVNODE     = (0x00000010),
            CR_ALREADY_SUCH_DEVINST     = CR_ALREADY_SUCH_DEVNODE,
            CR_INVALID_RANGE_LIST       = (0x00000011),
            CR_INVALID_RANGE            = (0x00000012),
            CR_FAILURE                  = (0x00000013),
            CR_NO_SUCH_LOGICAL_DEV      = (0x00000014),
            CR_CREATE_BLOCKED           = (0x00000015),
            /// <summary>WIN 95 ONLY</summary>
            CR_NOT_SYSTEM_VM            = (0x00000016),
            CR_REMOVE_VETOED            = (0x00000017),
            CR_APM_VETOED               = (0x00000018),
            CR_INVALID_LOAD_TYPE        = (0x00000019),
            CR_BUFFER_SMALL             = (0x0000001A),
            CR_NO_ARBITRATOR            = (0x0000001B),
            CR_NO_REGISTRY_HANDLE       = (0x0000001C),
            CR_REGISTRY_ERROR           = (0x0000001D),
            CR_INVALID_DEVICE_ID        = (0x0000001E),
            CR_INVALID_DATA             = (0x0000001F),
            CR_INVALID_API              = (0x00000020),
            CR_DEVLOADER_NOT_READY      = (0x00000021),
            CR_NEED_RESTART             = (0x00000022),
            CR_NO_MORE_HW_PROFILES      = (0x00000023),
            CR_DEVICE_NOT_THERE         = (0x00000024),
            CR_NO_SUCH_VALUE            = (0x00000025),
            CR_WRONG_TYPE               = (0x00000026),
            CR_INVALID_PRIORITY         = (0x00000027),
            CR_NOT_DISABLEABLE          = (0x00000028),
            CR_FREE_RESOURCES           = (0x00000029),
            CR_QUERY_VETOED             = (0x0000002A),
            CR_CANT_SHARE_IRQ           = (0x0000002B),
            CR_NO_DEPENDENT             = (0x0000002C),
            CR_SAME_RESOURCES           = (0x0000002D),
            CR_NO_SUCH_REGISTRY_KEY     = (0x0000002E),
            /// <summary>NT ONLY</summary>
            CR_INVALID_MACHINENAME      = (0x0000002F),
            /// <summary>NT ONLY</summary>
            CR_REMOTE_COMM_FAILURE      = (0x00000030),
            /// <summary>NT ONLY</summary>
            CR_MACHINE_UNAVAILABLE      = (0x00000031),
            /// <summary>NT ONLY</summary>
            CR_NO_CM_SERVICES           = (0x00000032),
            /// <summary>NT ONLY</summary>
            CR_ACCESS_DENIED            = (0x00000033),
            CR_CALL_NOT_IMPLEMENTED     = (0x00000034),
            CR_INVALID_PROPERTY         = (0x00000035),
            CR_DEVICE_INTERFACE_ACTIVE  = (0x00000036),
            CR_NO_SUCH_DEVICE_INTERFACE = (0x00000037),
            CR_INVALID_REFERENCE_STRING = (0x00000038),
            CR_INVALID_CONFLICT_LIST    = (0x00000039),
            CR_INVALID_INDEX            = (0x0000003A),
            CR_INVALID_STRUCTURE_SIZE   = (0x0000003B),
            NUM_CR_RESULTS              = (0x0000003C),
        }

        /// <summary>DevInst problem values, returned by call to CM_Get_DevInst_Status</summary>
        public enum CM_PROB : uint {
            /// <summary>no config for device</summary>
            CM_PROB_NOT_CONFIGURED             = (0x00000001),
            /// <summary>service load failed</summary>
            CM_PROB_DEVLOADER_FAILED           = (0x00000002),
            /// <summary>out of memory</summary>
            CM_PROB_OUT_OF_MEMORY              = (0x00000003),
            CM_PROB_ENTRY_IS_WRONG_TYPE        = (0x00000004),
            CM_PROB_LACKED_ARBITRATOR          = (0x00000005),
            /// <summary>boot config conflict</summary>
            CM_PROB_BOOT_CONFIG_CONFLICT       = (0x00000006),
            CM_PROB_FAILED_FILTER              = (0x00000007),
            /// <summary>Devloader not found</summary>
            CM_PROB_DEVLOADER_NOT_FOUND        = (0x00000008),
            /// <summary>Invalid ID</summary>
            CM_PROB_INVALID_DATA               = (0x00000009),
            CM_PROB_FAILED_START               = (0x0000000A),
            CM_PROB_LIAR                       = (0x0000000B),
            /// <summary>config conflict</summary>
            CM_PROB_NORMAL_CONFLICT            = (0x0000000C),
            CM_PROB_NOT_VERIFIED               = (0x0000000D),
            /// <summary>requires restart</summary>
            CM_PROB_NEED_RESTART               = (0x0000000E),
            CM_PROB_REENUMERATION              = (0x0000000F),
            CM_PROB_PARTIAL_LOG_CONF           = (0x00000010),
            CM_PROB_UNKNOWN_RESOURCE           = (0x00000011),
            /// <summary>unknown res type</summary>
            CM_PROB_REINSTALL                  = (0x00000012),
            CM_PROB_REGISTRY                   = (0x00000013),
            /// <summary>WINDOWS 95 ONLY</summary>
            CM_PROB_VXDLDR                     = (0x00000014),
            /// <summary>devinst will remove</summary>
            CM_PROB_WILL_BE_REMOVED            = (0x00000015),
            /// <summary>devinst is disabled</summary>
            CM_PROB_DISABLED                   = (0x00000016),
            // Devloader not ready
            CM_PROB_DEVLOADER_NOT_READY        = (0x00000017),
            /// <summary>device doesn't exist</summary>
            CM_PROB_DEVICE_NOT_THERE           = (0x00000018),
            CM_PROB_MOVED                      = (0x00000019),
            CM_PROB_TOO_EARLY                  = (0x0000001A),
            /// <summary>no valid log config</summary>
            CM_PROB_NO_VALID_LOG_CONF          = (0x0000001B),
            // install failed
            CM_PROB_FAILED_INSTALL             = (0x0000001C),
            /// <summary>device disabled</summary>
            CM_PROB_HARDWARE_DISABLED          = (0x0000001D),
            /// <summary>can't share IRQ</summary>
            CM_PROB_CANT_SHARE_IRQ             = (0x0000001E),
            /// <summary>driver failed add</summary>
            CM_PROB_FAILED_ADD                 = (0x0000001F),
            /// <summary>service's Start = 4</summary>
            CM_PROB_DISABLED_SERVICE           = (0x00000020),
            /// <summary>resource translation failed</summary>
            CM_PROB_TRANSLATION_FAILED         = (0x00000021),
            /// <summary>no soft config</summary>
            CM_PROB_NO_SOFTCONFIG              = (0x00000022),
            /// <summary>device missing in BIOS table</summary>
            CM_PROB_BIOS_TABLE                 = (0x00000023),
            /// <summary>IRQ translator failed</summary>
            CM_PROB_IRQ_TRANSLATION_FAILED     = (0x00000024),
            /// <summary>DriverEntry() failed.</summary>
            CM_PROB_FAILED_DRIVER_ENTRY        = (0x00000025),
            /// <summary>Driver should have unloaded.</summary>
            CM_PROB_DRIVER_FAILED_PRIOR_UNLOAD = (0x00000026),
            /// <summary>Driver load unsuccessful.</summary>
            CM_PROB_DRIVER_FAILED_LOAD         = (0x00000027),
            /// <summary>Error accessing driver's service key</summary>
            CM_PROB_DRIVER_SERVICE_KEY_INVALID = (0x00000028),
            /// <summary>Loaded legacy service created no devices</summary>
            CM_PROB_LEGACY_SERVICE_NO_DEVICES  = (0x00000029),
            /// <summary>Two devices were discovered with the same name</summary>
            CM_PROB_DUPLICATE_DEVICE           = (0x0000002A),
            /// <summary>The drivers set the device state to failed</summary>
            CM_PROB_FAILED_POST_START          = (0x0000002B),
            /// <summary>This device was failed post start via usermode</summary>
            CM_PROB_HALTED                     = (0x0000002C),
            /// <summary>The devinst currently exists only in the registry</summary>
            CM_PROB_PHANTOM                    = (0x0000002D),
            /// <summary>The system is shutting down</summary>
            CM_PROB_SYSTEM_SHUTDOWN            = (0x0000002E),
            /// <summary>The device is offline awaiting removal</summary>
            CM_PROB_HELD_FOR_EJECT             = (0x0000002F),
            /// <summary>One or more drivers is blocked from loading</summary>
            CM_PROB_DRIVER_BLOCKED             = (0x00000030),
            /// <summary>System hive has grown too large</summary>
            CM_PROB_REGISTRY_TOO_LARGE         = (0x00000031),
            /// <summary>Failed to apply one or more registry properties  </summary>
            CM_PROB_SETPROPERTIES_FAILED       = (0x00000032),
            /// <summary>Device is stalled waiting on a dependency to start</summary>
            CM_PROB_WAITING_ON_DEPENDENCY      = (0x00000033),
            /// <summary>Failed load driver due to unsigned image.</summary>
            CM_PROB_UNSIGNED_DRIVER            = (0x00000034),
            /// <summary>Device is being used by kernel debugger</summary>
            CM_PROB_USED_BY_DEBUGGER           = (0x00000035),
            /// <summary>Device is being reset</summary>
            CM_PROB_DEVICE_RESET               = (0x00000036),
            /// <summary>Device is blocked while console is locked</summary>
            CM_PROB_CONSOLE_LOCKED             = (0x00000037),
            /// <summary>Device needs extended class configuration to start</summary>
            CM_PROB_NEED_CLASS_CONFIG          = (0x00000038),
        }

        /// <summary>Device Instance status flags, returned by call to CM_Get_DevInst_Status</summary>
        [Flags]
        public enum DN_Status : uint {
            /// <summary>Was enumerated by ROOT</summary>
            DN_ROOT_ENUMERATED = (0x00000001),
            /// <summary>Has Register_Device_Driver</summary>
            DN_DRIVER_LOADED   = (0x00000002),
            /// <summary>Has Register_Enumerator</summary>
            DN_ENUM_LOADED     = (0x00000004),
            /// <summary>Is currently configured</summary>
            DN_STARTED         = (0x00000008),
            /// <summary>Manually installed</summary>
            DN_MANUAL          = (0x00000010),
            /// <summary>May need reenumeration</summary>
            DN_NEED_TO_ENUM    = (0x00000020),
            /// <summary>Has received a config</summary>
            DN_NOT_FIRST_TIME  = (0x00000040),
            /// <summary>Enum generates hardware ID</summary>
            DN_HARDWARE_ENUM   = (0x00000080),
            /// <summary>Lied about can reconfig once</summary>
            DN_LIAR            = (0x00000100),
            /// <summary>Not CM_Create_DevInst lately</summary>
            DN_HAS_MARK        = (0x00000200),
            /// <summary>Need device installer</summary>
            DN_HAS_PROBLEM     = (0x00000400),
            /// <summary>Is filtered</summary>
            DN_FILTERED        = (0x00000800),
            /// <summary>Has been moved</summary>
            DN_MOVED           = (0x00001000),
            /// <summary>Can be disabled</summary>
            DN_DISABLEABLE     = (0x00002000),
            /// <summary>Can be removed</summary>
            DN_REMOVABLE       = (0x00004000),
            /// <summary>Has a private problem</summary>
            DN_PRIVATE_PROBLEM = (0x00008000),
            /// <summary>Multi function parent</summary>
            DN_MF_PARENT       = (0x00010000),
            /// <summary>Multi function child</summary>
            DN_MF_CHILD        = (0x00020000),
            /// <summary>DevInst is being removed</summary>
            DN_WILL_BE_REMOVED = (0x00040000),

            //
            // Windows 4 OPK2 Flags
            //
            /// <summary>S: Has received a config enumerate</summary>
            DN_NOT_FIRST_TIMEE  = 0x00080000,
            /// <summary>S: When child is stopped, free resources</summary>
            DN_STOP_FREE_RES    = 0x00100000,
            /// <summary>S: Don't skip during rebalance</summary>
            DN_REBAL_CANDIDATE  = 0x00200000,
            /// <summary>S: This devnode's log_confs do not have same resources</summary>
            DN_BAD_PARTIAL      = 0x00400000,
            /// <summary>S: This devnode's is an NT enumerator</summary>
            DN_NT_ENUMERATOR    = 0x00800000,
            /// <summary>S: This devnode's is an NT driver</summary>
            DN_NT_DRIVER        = 0x01000000,
            //
            // Windows 4.1 Flags
            //
            /// <summary>S: Devnode need lock resume processing</summary>
            DN_NEEDS_LOCKING    = 0x02000000,
            /// <summary>S: Devnode can be the wakeup device</summary>
            DN_ARM_WAKEUP       = 0x04000000,
            /// <summary>S: APM aware enumerator</summary>
            DN_APM_ENUMERATOR   = 0x08000000,
            /// <summary>S: APM aware driver</summary>
            DN_APM_DRIVER       = 0x10000000,
            /// <summary>S: Silent install</summary>
            DN_SILENT_INSTALL   = 0x20000000,
            /// <summary>S: No show in device manager</summary>
            DN_NO_SHOW_IN_DM    = 0x40000000,
            /// <summary>S: Had a problem during preassignment of boot log conf</summary>
            DN_BOOT_LOG_PROB    = 0x80000000,

            //
            // Windows NT Flags
            //
            // These are overloaded on top of unused Win 9X flags
            //

            // WIN 2K OR NEWER

            /// <summary>System needs to be restarted for this Devnode to work properly</summary>
            DN_NEED_RESTART                 = DN_LIAR,


            // WIN XP OR NEWER

            // One or more drivers are blocked from loading for this Devnode
            DN_DRIVER_BLOCKED               = DN_NOT_FIRST_TIME,
            // This device is using a legacy driver
            DN_LEGACY_DRIVER                = DN_MOVED,
            // One or more children have invalid ID(s)
            DN_CHILD_WITH_INVALID_ID        = DN_HAS_MARK,


            // WIN 8 OR NEWER

            /// <summary>The function driver for a device reported that the device is not connected.  Typically this means a wireless device is out of range.</summary>
            DN_DEVICE_DISCONNECTED = DN_NEEDS_LOCKING,

            // WIN 10 OR NEWER

            /// <summary>Device is part of a set of related devices collectively pending query-removal</summary>
            DN_QUERY_REMOVE_PENDING         = DN_MF_PARENT,
            /// <summary>Device is actively engaged in a query-remove IRP</summary>
            DN_QUERY_REMOVE_ACTIVE          = DN_MF_CHILD,

            DN_CHANGEABLE_FLAGS = (DN_NOT_FIRST_TIME +
                DN_HARDWARE_ENUM +
                DN_HAS_MARK +
                DN_DISABLEABLE +
                DN_REMOVABLE +
                DN_MF_CHILD +
                DN_MF_PARENT +
                DN_NOT_FIRST_TIMEE +
                DN_STOP_FREE_RES +
                DN_REBAL_CANDIDATE +
                DN_NT_ENUMERATOR +
                DN_NT_DRIVER +
                DN_SILENT_INSTALL +
                DN_NO_SHOW_IN_DM)
        }

        /// <summary>Identifies the device property to be obtained from the registry.</summary>
        public enum CM_DRP {
            /// <summary>DeviceDesc REG_SZ property (RW)</summary>
            CM_DRP_DEVICEDESC = 0x00000001,
            /// <summary>HardwareID REG_MULTI_SZ property (RW)</summary>
            CM_DRP_HARDWAREID = 0x00000002,
            /// <summary>CompatibleIDs REG_MULTI_SZ property (RW)</summary>
            CM_DRP_COMPATIBLEIDS = 0x00000003,
            /// <summary>unused</summary>
            CM_DRP_UNUSED0 = 0x00000004,
            /// <summary>Service REG_SZ property (RW)</summary>
            CM_DRP_SERVICE = 0x00000005,
            /// <summary>unused</summary>
            CM_DRP_UNUSED1 = 0x00000006,
            /// <summary>unused</summary>
            CM_DRP_UNUSED2 = 0x00000007,
            /// <summary>Class REG_SZ property (RW)</summary>
            CM_DRP_CLASS = 0x00000008,
            /// <summary>ClassGUID REG_SZ property (RW)</summary>
            CM_DRP_CLASSGUID = 0x00000009,
            /// <summary>Driver REG_SZ property (RW)</summary>
            CM_DRP_DRIVER = 0x0000000A,
            /// <summary>ConfigFlags REG_DWORD property (RW)</summary>
            CM_DRP_CONFIGFLAGS = 0x0000000B,
            /// <summary>Mfg REG_SZ property (RW)</summary>
            CM_DRP_MFG = 0x0000000C,
            /// <summary>FriendlyName REG_SZ property (RW)</summary>
            CM_DRP_FRIENDLYNAME = 0x0000000D,
            /// <summary>LocationInformation REG_SZ property (RW)</summary>
            CM_DRP_LOCATION_INFORMATION = 0x0000000E,
            /// <summary>PhysicalDeviceObjectName REG_SZ property (R)</summary>
            CM_DRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000F,
            /// <summary>Capabilities REG_DWORD property (R)</summary>
            CM_DRP_CAPABILITIES = 0x00000010,
            /// <summary>UiNumber REG_DWORD property (R)</summary>
            CM_DRP_UI_NUMBER = 0x00000011,
            /// <summary>UpperFilters REG_MULTI_SZ property (RW)</summary>
            CM_DRP_UPPERFILTERS = 0x00000012,
            /// <summary>LowerFilters REG_MULTI_SZ property (RW)</summary>
            CM_DRP_LOWERFILTERS = 0x00000013,
            /// <summary>Bus Type Guid, GUID, (R)</summary>
            CM_DRP_BUSTYPEGUID = 0x00000014,
            /// <summary>Legacy bus type, INTERFACE_TYPE, (R)</summary>
            CM_DRP_LEGACYBUSTYPE = 0x00000015,
            /// <summary>Bus Number, DWORD, (R)</summary>
            CM_DRP_BUSNUMBER = 0x00000016,
            /// <summary>Enumerator Name REG_SZ property (R)</summary>
            CM_DRP_ENUMERATOR_NAME = 0x00000017,
            /// <summary>Security - Device override (RW)</summary>
            CM_DRP_SECURITY = 0x00000018,
            /// <summary>Security - Device override (RW)</summary>
            CM_DRP_SECURITY_SDS = 0x00000019,
            /// <summary>Device Type - Device override (RW)</summary>
            CM_DRP_DEVTYPE = 0x0000001A,
            /// <summary>Exclusivity - Device override (RW)</summary>
            CM_DRP_EXCLUSIVE = 0x0000001B,
            /// <summary>Characteristics - Device Override (RW)</summary>
            CM_DRP_CHARACTERISTICS = 0x0000001C,
            /// <summary>Device Address (R)</summary>
            CM_DRP_ADDRESS = 0x0000001D,
            /// <summary>UINumberDescFormat REG_SZ property (RW)</summary>
            CM_DRP_UI_NUMBER_DESC_FORMAT = 0x0000001E,
            /// <summary>CM_POWER_DATA REG_BINARY property (R)</summary>
            CM_DRP_DEVICE_POWER_DATA = 0x0000001F,
            /// <summary>CM_DEVICE_REMOVAL_POLICY REG_DWORD (R)</summary>
            CM_DRP_REMOVAL_POLICY = 0x00000020,
            /// <summary>CM_DRP_REMOVAL_POLICY_HW_DEFAULT REG_DWORD (R)</summary>
            CM_DRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000021,
            /// <summary>CM_DRP_REMOVAL_POLICY_OVERRIDE REG_DWORD (RW)</summary>
            CM_DRP_REMOVAL_POLICY_OVERRIDE = 0x00000022,
            /// <summary>CM_DRP_INSTALL_STATE REG_DWORD (R)</summary>
            CM_DRP_INSTALL_STATE = 0x00000023,
            /// <summary>CM_DRP_LOCATION_PATHS REG_MULTI_SZ (R)</summary>
            CM_DRP_LOCATION_PATHS = 0x00000024,
            /// <summary>Base ContainerID REG_SZ property (R)</summary>
            CM_DRP_BASE_CONTAINERID = 0x00000025,
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

        [Flags]
        public enum CM_LOCATE_DEVNODE : uint {
            /// <summary>The function retrieves the device instance handle for the specified device only if the device is currently configured in the device tree.</summary>
            CM_LOCATE_DEVNODE_NORMAL = 0x00000000,
            /// <summary>The function retrieves a device instance handle for the specified device if the device is currently configured in the device tree or the device is a nonpresent device that is not currently configured in the device tree.</summary>
            CM_LOCATE_DEVNODE_PHANTOM = 0x00000001,
            /// <summary>The function retrieves a device instance handle for the specified device if the device is currently configured in the device tree or in the process of being removed from the device tree. If the device is in the process of being removed, the function cancels the removal of the device.</summary>
            CM_LOCATE_DEVNODE_CANCELREMOVE = 0x00000002,
            /// <summary>Not used.</summary>
            CM_LOCATE_DEVNODE_NOVALIDATION = 0x00000004,
        }

        public enum REG_VALUE_TYPE : uint {
            /// <summary>No defined value type.</summary>
            REG_NONE = 0,

            /// <summary>Binary data in any form.</summary>
            REG_BINARY = 3,

            /// <summary>A 32-bit number.</summary>
            REG_DWORD = 4,

            /// <summary>
            /// A 32-bit number in little-endian format.
            /// <para>Windows is designed to run on little-endian computer architectures.</para>
            /// <para>Therefore, this value is defined as REG_DWORD in the Windows header files.</para>
            /// </summary>
            REG_DWORD_LITTLE_ENDIAN = 4,

            /// <summary>
            /// A 32-bit number in big-endian format.
            /// <para>Some UNIX systems support big-endian architectures.</para>
            /// </summary>
            REG_DWORD_BIG_ENDIAN = 5,

            /// <summary>
            /// A null-terminated string that contains unexpanded references to environment variables (for example, "%PATH%"). It will be a
            /// Unicode or ANSI string depending on whether you use the Unicode or ANSI functions. To expand the environment variable
            /// references, use the ExpandEnvironmentStrings function.
            /// </summary>
            REG_EXPAND_SZ = 2,

            /// <summary>
            /// A null-terminated Unicode string that contains the target path of a symbolic link that was created by calling the RegCreateKeyEx
            /// function with REG_OPTION_CREATE_LINK.
            /// </summary>
            REG_LINK = 6,

            /// <summary>
            /// A sequence of null-terminated strings, terminated by an empty string (\0).
            /// <para>The following is an example:</para>
            /// <para>String1\0String2\0String3\0LastString\0\0</para>
            /// <para>
            /// The first \0 terminates the first string, the second to the last \0 terminates the last string, and the final \0 terminates the
            /// sequence.Note that the final terminator must be factored into the length of the string.
            /// </para>
            /// </summary>
            REG_MULTI_SZ = 7,

            /// <summary>A 64-bit number.</summary>
            REG_QWORD = 11,

            /// <summary>
            /// A 64-bit number in little-endian format.
            /// <para>
            /// Windows is designed to run on little-endian computer architectures. Therefore, this value is defined as REG_QWORD in the Windows
            /// header files.
            /// </para>
            /// </summary>
            REG_QWORD_LITTLE_ENDIAN = 11,

            /// <summary>
            /// A null-terminated string. This will be either a Unicode or an ANSI string, depending on whether you use the Unicode or ANSI functions.
            /// </summary>
            REG_SZ = 1,

            /// <summary>Resource list in the resource map.</summary>
            REG_RESOURCE_LIST = 8,

            /// <summary>Resource list in the hardware description.</summary>
            REG_FULL_RESOURCE_DESCRIPTOR = 9,

            /// <summary>Resource requirement list.</summary>
            REG_RESOURCE_REQUIREMENTS_LIST = 10,
        }

        public const int REGSTR_VAL_MAX_HCID_LEN = 1024;

        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern CR_RESULT CM_Get_DevNode_Status(out DN_Status status, out CM_PROB probNum, UInt32 devInst, int flags);

        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern CR_RESULT CM_Get_Parent(out uint pdnDevInst, UInt32 dnDevInst, int ulFlags);
        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern CR_RESULT CM_Locate_DevNode(out uint pdnDevInst, [In, Optional, MarshalAs(UnmanagedType.LPTStr)] string pDeviceID, CM_LOCATE_DEVNODE ulFlags = CM_LOCATE_DEVNODE.CM_LOCATE_DEVNODE_NORMAL);

        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern CR_RESULT CM_Get_Device_ID_Size(out int pulLen, UInt32 dnDevInst, int flags = 0);

        [DllImport("cfgmgr32.dll", SetLastError = true)]
        public static extern CR_RESULT CM_Get_Device_ID(uint dnDevInst, StringBuilder Buffer, int BufferLen, int ulFlags = 0);

        [DllImport("cfgmgr32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern CR_RESULT CM_Get_DevNode_Registry_Property(uint dnDevInst, CM_DRP ulProperty, out REG_VALUE_TYPE pulRegDataType, [Out, Optional] IntPtr Buffer, ref uint pulLength, uint ulFlags = 0);
        [DllImport("cfgmgr32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern CR_RESULT CM_Get_DevNode_Registry_Property(uint dnDevInst, CM_DRP ulProperty, out REG_VALUE_TYPE pulRegDataType, [Out, Optional] StringBuilder Buffer, ref uint pulLength, uint ulFlags = 0);
        [DllImport("cfgmgr32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern CR_RESULT CM_Get_DevNode_Registry_Property(uint dnDevInst, CM_DRP ulProperty, out REG_VALUE_TYPE pulRegDataType, [Out, Optional] byte[] Buffer, ref uint pulLength, uint ulFlags = 0);
        [DllImport("cfgmgr32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern CR_RESULT CM_Get_DevNode_Registry_Property(uint dnDevInst, CM_DRP ulProperty, out REG_VALUE_TYPE pulRegDataType, [Out, Optional] Int32 Buffer, ref uint pulLength, uint ulFlags = 0);
        [DllImport("cfgmgr32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern CR_RESULT CM_Get_DevNode_Property(uint dnDevInst, in DEVPROPKEY PropertyKey, out DEVPROPTYPE PropertyType, [Out, Optional] IntPtr PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags = 0);
        [DllImport("cfgmgr32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        public static extern CR_RESULT CM_Get_DevNode_Property(uint dnDevInst, in DEVPROPKEY PropertyKey, out DEVPROPTYPE PropertyType, [Out, Optional] StringBuilder PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags = 0);
    }
}
