using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FluentUsbTreeView.UsbTreeView {
    public class Settings {

        private const string APPLICATION_FOLDER_NAME = "FluentUsbTreeView";
        private const string SETTINGS_FILE = "config.json";
        public const int INVALID_POSITION = 0xFFFFFFF;

        private static Settings s_settings;
        public static Settings Instance {
            get {
                if ( s_settings == null) {
                    s_settings = new Settings();
                }
                return s_settings;
            }
        }

        private string m_settingsPath = "%FUTV_DIR%/config.json";
        private string m_appDirectory = "%FUTV_DIR%";

        [JsonIgnore]
        public string SettingsPath { get { return m_settingsPath; } }
        [JsonIgnore]
        public string ApplicationDirectory { get { return m_appDirectory; } }

        private Settings() {
            m_appDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APPLICATION_FOLDER_NAME));
            m_settingsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APPLICATION_FOLDER_NAME, SETTINGS_FILE));
        }

        public void WriteSettings() {
            // Writes the settings to a file
            string serializedSettings = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsPath, serializedSettings);
            Logger.Info($"Wrote settings file to \"{SettingsPath}\"!");
        }

        public void LoadSettings() {
            // Ensure app directory exists 
            if ( !Directory.Exists(ApplicationDirectory) ) {
                Directory.CreateDirectory(ApplicationDirectory);
            }

            if ( !File.Exists(SettingsPath) ) {
                WriteSettings();
                return;
            }

            string rawSettings = File.ReadAllText(SettingsPath);
            var settingsDeserialized = JsonConvert.DeserializeObject<Settings>(rawSettings);
            if (settingsDeserialized != null ) {
                Logger.Info("Loaded settings file!");
                s_settings = settingsDeserialized;
            }
        }

        public int PositionX { get; set; } = INVALID_POSITION;
        public int PositionY { get; set; } = INVALID_POSITION;
        public int SizeX { get; set; } = INVALID_POSITION;
        public int SizeY { get; set; } = INVALID_POSITION;
        public int SplitterPosition { get; set; } = INVALID_POSITION;

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
