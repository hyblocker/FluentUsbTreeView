using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace FluentUsbTreeView.PInvoke {

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_HCD_DRIVERKEY_NAME {
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DriverKeyName;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_ROOT_HUB_NAME {
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string RootHubName;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_NODE_CONNECTION_DRIVERKEY_NAME {
        public uint ConnectionIndex;
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DriverKeyName;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_HUB_NAME {
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string HubName;
    }

    public static class Kernel32 {

        public const uint ERROR_SUCCESS = 0;
        public const uint ERROR_INSUFFICIENT_BUFFER = 122;
        public const uint ERROR_NO_MORE_ITEMS = 259;
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_WRITE = 2;
        public const int OPEN_EXISTING = 3;

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            uint lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sscanf(string buffer, string format, out uint arg0, out uint arg1, out uint arg3, out uint arg4);

        public const int IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME       = 0x220420;
        public const int IOCTL_GET_HCD_DRIVERKEY_NAME                       = 0x220424;
        public const int IOCTL_USB_GET_ROOT_HUB_NAME                        = 0x220408;
        public const int IOCTL_USB_GET_NODE_INFORMATION                     = 0x220408;
        public const int IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION      = 0x220410;
        public const int IOCTL_USB_GET_NODE_CONNECTION_NAME                 = 0x220414;
        public const int IOCTL_USB_USER_REQUEST                             = 0x220438;
        public const int IOCTL_USB_GET_HUB_CAPABILITIES_EX                  = 0x220450;
        public const int IOCTL_USB_GET_HUB_INFORMATION_EX                   = 0x220454;
        public const int IOCTL_USB_GET_PORT_CONNECTOR_PROPERTIES            = 0x220458;
        public const int IOCTL_USB_GET_NODE_CONNECTION_INFORMATION          = 0x22040C;
        public const int IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX       = 0x220448;
        public const int IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2    = 0x22045C;

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            int dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool DeviceIoControl(
            IntPtr hDevice,
            int dwIoControlCode,
            void* lpInBuffer,
            int nInBufferSize,
            void* lpOutBuffer,
            int nOutBufferSize,
            int* lpBytesReturned,
            IntPtr lpOverlapped);

        public static bool DeviceIoControl<T>(
            IntPtr hDevice,
            int dwIoControlCode,
            ref T lpInBuffer,
            int nInBufferSize,
            out T lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped) where T : struct {

            lpOutBuffer = new T();

            // memory safety yippee
            int size = Marshal.SizeOf( typeof ( T ) );
            IntPtr inBufferPtr = Marshal.AllocHGlobal( size );
            Marshal.StructureToPtr(lpInBuffer, inBufferPtr, false);
            IntPtr outBufferPtr = Marshal.AllocHGlobal( size );
            bool bResult = DeviceIoControl(hDevice, dwIoControlCode, inBufferPtr, nInBufferSize, outBufferPtr, nOutBufferSize, out lpBytesReturned, lpOverlapped);
            try {
                lpOutBuffer = ( T ) Marshal.PtrToStructure(outBufferPtr, lpOutBuffer.GetType());
            } finally {
                Marshal.FreeHGlobal(inBufferPtr);
                Marshal.FreeHGlobal(outBufferPtr);
            }

            return bResult;
        }
    }
}
