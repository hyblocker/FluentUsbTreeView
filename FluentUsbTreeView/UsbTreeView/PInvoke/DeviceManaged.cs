using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace FluentUsbTreeView.PInvoke {
    public static class DeviceManaged {

        /// <summary>
        /// Called whenever a new device is added
        /// </summary>
        public static Action<string> OnDeviceAdded;
        /// <summary>
        /// Called whenever a new device is removed
        /// </summary>
        public static Action<string> OnDeviceRemoved;
        /// <summary>
        /// Called whenever device nodes have changed
        /// </summary>
        public static Action OnDeviceNodesChanged;

        // Prevent race conditions
        private static volatile bool s_isUnregisteringDevice = false;

        private static IntPtr s_hHwndSrc = IntPtr.Zero;

        private static IntPtr CreateMessageWindow() {
            // -1, -2 are invalid handles
            IntPtr hwndMsg = new IntPtr(-3);
            HwndSourceParameters srcParams = new HwndSourceParameters() { ParentWindow = hwndMsg };
            HwndSource src = new HwndSource(srcParams);
            src.AddHook(WndProc);
            return src.Handle;
        }

        public static void RegisterDeviceNotificationHandler() {
            if ( s_deviceNotificationHandle != IntPtr.Zero && s_deviceNotificationHandle != Kernel32.INVALID_HANDLE_VALUE ) {
                return;
            }

            s_hHwndSrc = CreateMessageWindow();

            var dbi = new DEV_BROADCAST_DEVICEINTERFACE_SETUP {
                dbcc_size           = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE_SETUP)),
                dbcc_devicetype     = DBT_DEVTYP_DEVICEINTERFACE,
                dbcc_reserved       = 0,
                dbcc_classguid      = WinApiGuids.GUID_DEVINTERFACE_USB_DEVICE,
                dbcc_name           = IntPtr.Zero
            };

            IntPtr buffer = Marshal.AllocHGlobal(dbi.dbcc_size);
            Marshal.StructureToPtr(dbi, buffer, true);

            s_deviceNotificationHandle = User32.RegisterDeviceNotification(s_hHwndSrc, buffer, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);

        }

        public static void UnregisterDeviceNotifications() {
            if ( s_isUnregisteringDevice == false ) {
                s_isUnregisteringDevice = true;

                if ( s_deviceNotificationHandle != null && ( s_deviceNotificationHandle != IntPtr.Zero && s_deviceNotificationHandle != Kernel32.INVALID_HANDLE_VALUE ) ) {
                    User32.UnregisterDeviceNotification(s_deviceNotificationHandle);
                }

                s_deviceNotificationHandle = Kernel32.INVALID_HANDLE_VALUE;
                s_isUnregisteringDevice = false;
            }
        }

        private static IntPtr s_deviceNotificationHandle = Kernel32.INVALID_HANDLE_VALUE;

        private const int DBT_DEVICEARRIVAL         = 0x8000; // New device        
        private const int DBT_DEVICEREMOVECOMPLETE  = 0x8004; // Removed
        private const int DBT_DEVNODES_CHANGED      = 0x0007; // Removed
        private const int WM_DEVICECHANGE           = 0x0219; // Change event
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

        private static IntPtr WndProc(IntPtr hwnd, int uMsg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if ( uMsg == WM_DEVICECHANGE ) {
                switch ( ( int ) wParam ) {
                    case DBT_DEVICEREMOVECOMPLETE:
                        if ( OnDeviceRemoved != null ) {
                            DEV_BROADCAST_DEVICEINTERFACE lParamData = (DEV_BROADCAST_DEVICEINTERFACE) Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
                            // string nameDecoded = Marshal.PtrToStringAuto(lParamData.Name);
                            // OnDeviceRemoved(nameDecoded);
                            OnDeviceRemoved(lParamData.dbcc_name);
                        }
                        break;
                    case DBT_DEVICEARRIVAL:
                        if ( OnDeviceAdded != null ) {
                            DEV_BROADCAST_DEVICEINTERFACE lParamData = (DEV_BROADCAST_DEVICEINTERFACE) Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
                            // string nameDecoded = Marshal.PtrToStringAuto(lParamData.Name);
                            // OnDeviceAdded(nameDecoded);
                            OnDeviceAdded(lParamData.dbcc_name);
                        }
                        break;
                    case DBT_DEVNODES_CHANGED:
                        if ( OnDeviceNodesChanged != null )
                            OnDeviceNodesChanged();

                        break;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }
    }
}
