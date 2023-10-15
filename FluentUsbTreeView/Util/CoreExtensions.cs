using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView {
    public static class CoreExtensions {
        public static T Clone<T>(this T val) where T : struct => val;
    }
}
