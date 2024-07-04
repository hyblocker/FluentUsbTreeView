using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView {
    // Look this is here because people using other apps without an updater get stuck on ancient versions and that sucks!!
    public class Updater {

        const string UPDATER_URL = "https://raw.githubusercontent.com/hyblocker/FluentUsbTreeView/master/.github/version.json";

        const int BINARY_VERSION = 1;
        private int currentVersion = -1;

        private bool m_lastUpdateCheckSuccessful = false;

        public bool CheckForUpdates() {
            try {
                using ( WebClient wc = new WebClient() ) {
                    string jsonDownloaded = wc.DownloadString(UPDATER_URL);
                    var json = JsonConvert.DeserializeObject<dynamic>(jsonDownloaded);
                    m_lastUpdateCheckSuccessful = false;
                    if ( json != null ) {
                        // Get version field
                        if ( json["version_numeric"] != null ) {
                            if (!int.TryParse(json["version_numeric"].ToString(), out currentVersion)) {
                                // Unknown latest version
                                m_lastUpdateCheckSuccessful = false;
                            }
                        }
                    }
                }

                return m_lastUpdateCheckSuccessful;
            } catch ( Exception ex ) {
                Logger.Fatal(ex);
                return false;
            }
        }
    }
}
