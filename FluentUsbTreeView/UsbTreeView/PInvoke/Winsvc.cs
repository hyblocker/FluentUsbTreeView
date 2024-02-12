using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.PInvoke {
    public static class Winsvc {

        public const ulong DELETE                           = (0x00010000L);
        public const ulong READ_CONTROL                     = (0x00020000L);
        public const ulong WRITE_DAC                        = (0x00040000L);
        public const ulong WRITE_OWNER                      = (0x00080000L);
        public const ulong SYNCHRONIZE                      = (0x00100000L);

        public const ulong STANDARD_RIGHTS_REQUIRED         = (0x000F0000L);

        public const ulong STANDARD_RIGHTS_READ             = (READ_CONTROL);
        public const ulong STANDARD_RIGHTS_WRITE            = (READ_CONTROL);
        public const ulong STANDARD_RIGHTS_EXECUTE          = (READ_CONTROL);

        public const ulong STANDARD_RIGHTS_ALL              = (0x001F0000L);

        public const ulong SPECIFIC_RIGHTS_ALL              = (0x0000FFFFL);


        [Flags]
        public enum ENUM_SERVICE_TYPE : uint {
            SERVICE_DRIVER              = 0x0000000B,
            SERVICE_KERNEL_DRIVER       = 0x00000001,
            SERVICE_WIN32               = 0x00000030,
            SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
            SERVICE_ADAPTER             = 0x00000004,
            SERVICE_FILE_SYSTEM_DRIVER  = 0x00000002,
            SERVICE_RECOGNIZER_DRIVER   = 0x00000008,
            SERVICE_WIN32_OWN_PROCESS   = 0x00000010,
            SERVICE_USER_OWN_PROCESS    = 0x00000050,
            SERVICE_USER_SHARE_PROCESS  = 0x00000060,
        }

        public enum SERVICE_ERROR : uint {
            SERVICE_ERROR_CRITICAL  = 3U,
            SERVICE_ERROR_IGNORE    = 0U,
            SERVICE_ERROR_NORMAL    = 1U,
            SERVICE_ERROR_SEVERE    = 2U,
        }

        public enum SERVICE_START_TYPE : uint {
            SERVICE_AUTO_START      = 2U,
            SERVICE_BOOT_START      = 0U,
            SERVICE_DEMAND_START    = 3U,
            SERVICE_DISABLED        = 4U,
            SERVICE_SYSTEM_START    = 1U,
        }

        [Flags]
        public enum SERVICE_ACCESS_RIGHTS {
            SERVICE_QUERY_CONFIG           = 0x0001,
            SERVICE_CHANGE_CONFIG          = 0x0002,
            SERVICE_QUERY_STATUS           = 0x0004,
            SERVICE_ENUMERATE_DEPENDENTS   = 0x0008,
            SERVICE_START                  = 0x0010,
            SERVICE_STOP                   = 0x0020,
            SERVICE_PAUSE_CONTINUE         = 0x0040,
            SERVICE_INTERROGATE            = 0x0080,
            SERVICE_USER_DEFINED_CONTROL   = 0x0100,

            SERVICE_ALL_ACCESS             = 0xF01FF,
        }

        [Flags]
        public enum SC_MANAGER_ACCESS_RIGHTS {
            SC_MANAGER_CONNECT             = 0x0001,
            SC_MANAGER_CREATE_SERVICE      = 0x0002,
            SC_MANAGER_ENUMERATE_SERVICE   = 0x0004,
            SC_MANAGER_LOCK                = 0x0008,
            SC_MANAGER_QUERY_LOCK_STATUS   = 0x0010,
            SC_MANAGER_MODIFY_BOOT_CONFIG  = 0x0020,

            SC_MANAGER_ALL_ACCESS          = 0xF003F
        }


        [StructLayout(LayoutKind.Sequential)]
        public unsafe class QUERY_SERVICE_CONFIG {
            public int dwServiceType;
            public int dwStartType;
            public int dwErrorControl;
            public char *lpBinaryPathName;
            public char *lpLoadOrderGroup;
            public int dwTagId;
            public char *lpDependencies;
            public char *lpServiceStartName;
            public char *lpDisplayName;
        }

        public const int SC_MANAGER_ALL_ACCESS = 0x000F003F;
        public const int SERVICE_QUERY_CONFIG = 0x00000001;

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(IntPtr lpMachineName, IntPtr lpDatabaseName, SC_MANAGER_ACCESS_RIGHTS dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, SERVICE_ACCESS_RIGHTS dwDesiredAccess);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "QueryServiceConfigW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool QueryServiceConfig(IntPtr hService, IntPtr lpServiceConfig, uint cbBufSize, uint* pcbBytesNeeded);

        [DllImport("advapi32.dll", ExactSpelling = true, EntryPoint = "QueryServiceConfigW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool QueryServiceConfig(IntPtr hService, [Optional] QUERY_SERVICE_CONFIG* lpServiceConfig, uint cbBufSize, uint* pcbBytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);
    }
}
