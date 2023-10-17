using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.UsbTreeView {
    public class Settings {
        private static Settings s_settings;
        public static Settings Instance {
            get {
                if ( s_settings == null) {
                    s_settings = new Settings();
                }
                return s_settings;
            }
        }

        public string SettingsPath { get {  return "idk yet lol"; } }

        private Settings() {

        }
    }
}
