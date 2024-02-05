using System;
using System.Runtime.InteropServices;

namespace FluentUsbTreeView.PInvoke {

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct DEV_BROADCAST_DEVICEINTERFACE_SETUP {
        public int dbcc_size;
        public int dbcc_devicetype;
        public int dbcc_reserved;
        public Guid dbcc_classguid;
        public IntPtr dbcc_name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct DEV_BROADCAST_DEVICEINTERFACE {
        public int dbcc_size;
        public int dbcc_devicetype;
        public int dbcc_reserved;
        public Guid dbcc_classguid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=255)]
        public string dbcc_name;
    }

    public static class User32 {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        public static extern bool UnregisterDeviceNotification(IntPtr handle);
    }
}
