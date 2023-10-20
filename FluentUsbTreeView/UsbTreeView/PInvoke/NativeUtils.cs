using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.PInvoke {
    public static class NativeUtils {
        public static IntPtr ToPtr(uint val) {
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(uint));

            byte[] byteVal = BitConverter.GetBytes(val);
            Marshal.Copy(byteVal, 0, ptr, byteVal.Length);
            return ptr;
        }
        public static IntPtr ToPtr(int val) {
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(int));

            byte[] byteVal = BitConverter.GetBytes(val);
            Marshal.Copy(byteVal, 0, ptr, byteVal.Length);
            return ptr;
        }
    }
}
