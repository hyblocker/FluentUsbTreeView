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
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
        // public string DriverKeyName;
        public IntPtr DriverKeyNamePtr;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_NODE_CONNECTION_DRIVERKEY_NAME_STRING {
        public uint ConnectionIndex;
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
        public string DriverKeyName;
        // public IntPtr DriverKeyNamePtr;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct USB_HUB_NAME {
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string HubName;
    }

    public static class Kernel32 {

        public const uint ERROR_SUCCESS = 0;
        public const uint ERROR_INVALID_DATA = 13;
        public const uint ERROR_INSUFFICIENT_BUFFER = 122;
        public const uint ERROR_NO_MORE_ITEMS = 259;
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_READ    = 0x00000001;
        public const int FILE_SHARE_WRITE   = 0x00000002;
        public const int FILE_SHARE_DELETE  = 0x00000004;
        public const int OPEN_EXISTING = 3;

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);


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

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.U8)]
        public static extern ulong GetTickCount64();

        [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sscanf(string buffer, string format, out uint arg0, out uint arg1, out uint arg3, out uint arg4);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static unsafe extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            void* lpInBuffer,
            int nInBufferSize,
            void* lpOutBuffer,
            int nOutBufferSize,
            int* lpBytesReturned,
            IntPtr lpOverlapped);

        public static bool DeviceIoControl<T>(
            IntPtr hDevice,
            uint dwIoControlCode,
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


        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]

        private static extern uint GetLastError();
        public static void EnableAnsiCmd() {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if ( !GetConsoleMode(iStdOut, out uint outConsoleMode) ) {
                Console.Error.WriteLine("Failed to get output console mode");
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if ( !SetConsoleMode(iStdOut, outConsoleMode) ) {
                Console.Error.WriteLine($"Failed to set output console mode, error code: {GetLastError()}");
                return;
            }
        }
    }
}
