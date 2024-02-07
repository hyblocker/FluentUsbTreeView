using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace FluentUsbTreeView.PInvoke {
    public static class WindowsTools {

        [DllImport("devmgr.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void DeviceProperties_RunDLL(IntPtr hwndStub, IntPtr hAppInstance /* NULL */, string lpCmdLine, int nCmdShow);

        public static void OpenDeviceManagerAtDevice(string deviceId) {
            IntPtr windowHandle = new WindowInteropHelper(MainWindow.Instance).Handle;
            DeviceProperties_RunDLL(windowHandle, IntPtr.Zero, $"/DeviceID {deviceId}", 0);
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct LargeIntegerStruct {
            [FieldOffset(0)]
            public uint LowPart;
            [FieldOffset(4)]
            public int HighPart;
            [FieldOffset(0)]
            public long QuadPart;

            internal DateTime ToDateTime() {
                try {
                    return DateTime.FromFileTime(QuadPart);
                } catch ( ArgumentException ) {
                    return DateTime.MinValue;
                }
            }
        }
    }
}
