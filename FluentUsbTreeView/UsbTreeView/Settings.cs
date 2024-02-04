using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            // @TODO: Load settings
        }

        public bool AutoRefresh { get; set; } = true;
        public bool EndpointDescriptors { get; set; } = true;
        public bool ScanAllStringDescriptors { get; set; } = false;
        public bool DescriptorHexDumps { get; set; } = false;
        public bool DriveNumbersInTree { get; set; } = false;
        public bool EndpointsInTree { get; set; } = false;

        // Automatic expansion
        public bool ExpandForEmptyPorts { get; set; } = false;
        public bool ExpandForEmptyHubs { get; set; } = false;
        public bool ExpandForNewDevices { get; set; } = true;
        public bool JumpToNewDevices { get; set; } = false;
        public bool JumpToRemovedDevices { get; set; } = false;
    }
}
