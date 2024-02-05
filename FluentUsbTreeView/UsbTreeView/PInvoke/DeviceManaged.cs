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

        public static void RegisterDeviceNotificationHandler() {
            if ( s_deviceNotificationHandle != IntPtr.Zero && s_deviceNotificationHandle != Kernel32.INVALID_HANDLE_VALUE ) {
                return;
            }
            s_window = new NotificationListenWindow();
            s_window.Show();
            if ( Application.Current.MainWindow.GetType() == s_window.GetType() )
                Application.Current.MainWindow = null;
        }

        public static void UnregisterDeviceNotifications() {
            if ( s_deviceNotificationHandle != null && ( s_deviceNotificationHandle != IntPtr.Zero && s_deviceNotificationHandle != Kernel32.INVALID_HANDLE_VALUE ) )
                User32.UnregisterDeviceNotification(s_deviceNotificationHandle);
            try {
                if ( s_window != null ) {
                    s_window.Dispatcher.Invoke(() => {
                        s_window.Close();
                    });
                }
            } finally {
                s_deviceNotificationHandle = Kernel32.INVALID_HANDLE_VALUE;
            }
        }

        private static NotificationListenWindow s_window = null;
        private static IntPtr s_deviceNotificationHandle = Kernel32.INVALID_HANDLE_VALUE;

        private const int DBT_DEVICEARRIVAL         = 0x8000; // New device        
        private const int DBT_DEVICEREMOVECOMPLETE  = 0x8004; // Removed
        private const int DBT_DEVNODES_CHANGED      = 0x0007; // Removed
        private const int WM_DEVICECHANGE           = 0x0219; // Change event
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

        private class NotificationListenWindow : Window {

            [DllImport("user32.dll")]
            static extern IntPtr SetParent(IntPtr hwnd, IntPtr hwndNewParent);
            private const int HWND_MESSAGE = -3;

            private IntPtr windowHandle;
            private HwndSource m_hwnd;

            internal NotificationListenWindow() { }
            protected override void OnSourceInitialized(EventArgs e) {
                base.OnSourceInitialized(e);
                m_hwnd = PresentationSource.FromVisual(this) as HwndSource;

                if ( m_hwnd != null ) {
                    windowHandle = m_hwnd.Handle;
                    SetParent(windowHandle, ( IntPtr ) HWND_MESSAGE);
                    Visibility = Visibility.Hidden;
                    m_hwnd.AddHook(WndProc);

                    var dbi = new DEV_BROADCAST_DEVICEINTERFACE_SETUP {
                        dbcc_size           = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE_SETUP)),
                        dbcc_devicetype     = DBT_DEVTYP_DEVICEINTERFACE,
                        dbcc_reserved       = 0,
                        dbcc_classguid      = WinApiGuids.GUID_DEVINTERFACE_USB_DEVICE,
                        dbcc_name           = IntPtr.Zero
                    };

                    IntPtr buffer = Marshal.AllocHGlobal(dbi.dbcc_size);
                    Marshal.StructureToPtr(dbi, buffer, true);

                    s_deviceNotificationHandle = User32.RegisterDeviceNotification(windowHandle, buffer, DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
                }
            }

            protected override void OnClosed(EventArgs e) {
                Hide();
                m_hwnd.RemoveHook(WndProc);
                m_hwnd.Dispose();
            }

            private IntPtr WndProc(IntPtr hwnd, int uMsg, IntPtr wParam, IntPtr lParam, ref bool handled) {
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
}
