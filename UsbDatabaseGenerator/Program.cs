using System.Text;

namespace UsbDatabaseGenerator {
    internal class Program {

        const string USB_FORUM_KNOWN_VENDORS = "https://cms.usb.org/usbif.json";
        const string LINUX_KERNEL_KNOWN_USB_DEVICES = "http://www.linux-usb.org/usb.ids";
        const string LINUX_KERNEL_KNOWN_PCIE_DEVICES = "https://pci-ids.ucw.cz/v2.2/pci.ids";

        /// <summary>
        /// Program whose sole purpose is basically code-gen;
        /// Takes the json from the 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            Dictionary<ushort, string> usbVendorNames = new Dictionary<ushort, string>();
            Dictionary<uint, string> usbProductNames = new Dictionary<uint, string>();
            Dictionary<ushort, string> pcieVendorNames = new Dictionary<ushort, string>();
            Dictionary<uint, string> pcieProductNames = new Dictionary<uint, string>();

            Console.WriteLine("Fetching latest USB Forum database...");
            string usbForumRaw = DownloadString(USB_FORUM_KNOWN_VENDORS);
            File.WriteAllText("usb_forum.json", usbForumRaw);
            Console.WriteLine("Parsing USB Forum database...");
            UsbForumVidParser.ParseUsbForumVids(usbForumRaw, usbVendorNames);

            Console.WriteLine("Fetching latest known USB devices from Linux USB IDs database...");
            string linuxKernelKnownDevsRaw = DownloadString(LINUX_KERNEL_KNOWN_USB_DEVICES);
            File.WriteAllText("linux_usb.ids", linuxKernelKnownDevsRaw);
            Console.WriteLine("Parsing Linux kernel Ids...");
            LinuxKernelParser.ParseLinuxKernelUsbIds(linuxKernelKnownDevsRaw, usbVendorNames, usbProductNames);

#if false
            Console.WriteLine("Fetching latest known PCIe devices from PCIe IDs database...");
            string pcieDbKnownPcieVidsRaw = DownloadString(LINUX_KERNEL_KNOWN_PCIE_DEVICES);
            File.WriteAllText("pcie.ids", pcieDbKnownPcieVidsRaw);
            Console.WriteLine("Parsing PCIe IDs...");
            LinuxKernelParser.ParseLinuxKernelUsbIds(pcieDbKnownPcieVidsRaw, pcieVendorNames, pcieProductNames);
#endif

            // Above is useful but also somewhat useless for us...
            // As ultimately we don't care above most PCIe vendors, only the few that make USB controllers
            // Hence hardcode the few that do:
            // This also let's us offer prettier names for the PCIe vendors database!
            // TLDR: Get the PCIe database, comb through every product going under Host Controller, manually add the string here
            pcieVendorNames.Add(0x021B, "Compaq");
            pcieVendorNames.Add(0x0E11, "Compaq");
            pcieVendorNames.Add(0x1032, "Compaq");
            pcieVendorNames.Add(0x10DA, "Compaq");
            pcieVendorNames.Add(0x1002, "AMD/ATI");
            pcieVendorNames.Add(0x1022, "AMD");
            pcieVendorNames.Add(0x1025, "Acer");
            pcieVendorNames.Add(0x1033, "NEC");
            pcieVendorNames.Add(0x11C3, "NEC");
            pcieVendorNames.Add(0x1BCF, "NEC");
            pcieVendorNames.Add(0xA200, "NEC");
            pcieVendorNames.Add(0x104C, "Texas Instruments");
            pcieVendorNames.Add(0x104D, "Sony");
            pcieVendorNames.Add(0xA304, "Sony");
            pcieVendorNames.Add(0x1095, "Silicon Image");
            pcieVendorNames.Add(0x10B9, "ULi Electronics");
            pcieVendorNames.Add(0x10DE, "NVIDIA");
            pcieVendorNames.Add(0x10EC, "Realtek");
            pcieVendorNames.Add(0x1106, "VIA");
            pcieVendorNames.Add(0x1412, "VIA");
            pcieVendorNames.Add(0x0925, "VIA");
            pcieVendorNames.Add(0x1131, "Philips");
            pcieVendorNames.Add(0x14B1, "Philips");
            pcieVendorNames.Add(0x152F, "Philips");
            pcieVendorNames.Add(0x102F, "Toshiba");
            pcieVendorNames.Add(0x1179, "Toshiba");
            pcieVendorNames.Add(0x11E7, "Toshiba");
            pcieVendorNames.Add(0x13D7, "Toshiba");
            pcieVendorNames.Add(0x1180, "Ricoh");
            pcieVendorNames.Add(0x1217, "O2 Micro");
            pcieVendorNames.Add(0x1000, "Broadcom");
            pcieVendorNames.Add(0x1166, "Broadcom");
            pcieVendorNames.Add(0x14E4, "Broadcom");
            pcieVendorNames.Add(0x166D, "Broadcom");
            pcieVendorNames.Add(0x182F, "Broadcom");
            pcieVendorNames.Add(0xFEDA, "Broadcom");
            pcieVendorNames.Add(0x1538, "ARALION");
            pcieVendorNames.Add(0x1679, "Tokyo Electron");
            pcieVendorNames.Add(0x17A0, "Genesys Logic");
            pcieVendorNames.Add(0x1912, "Renesas");
            pcieVendorNames.Add(0x1947, "C-guys");
            pcieVendorNames.Add(0x197B, "JMicron");
            pcieVendorNames.Add(0x19E5, "Huawei");
            pcieVendorNames.Add(0x1B21, "ASMedia");
            pcieVendorNames.Add(0x1AF4, "Red Hat");
            pcieVendorNames.Add(0x1B36, "Red Hat");
            pcieVendorNames.Add(0x6900, "Red Hat");
            pcieVendorNames.Add(0x1B6F, "Etron");
            pcieVendorNames.Add(0x1B73, "Fresco Logic");
            pcieVendorNames.Add(0x1D17, "Zhaoxin");
            pcieVendorNames.Add(0x1D94, "HyGon"); // Chengdu Haiguang IC Design is colloquially referred to as HyGon
            pcieVendorNames.Add(0x8086, "Intel");
            pcieVendorNames.Add(0x9710, "MosChip");
            pcieVendorNames.Add(0xAAAA, "Adnaco");


            // Final file write
            Console.WriteLine("Generating file...");
            StringBuilder usbVendorGenerator = new StringBuilder();
            StringBuilder usbProductGenerator = new StringBuilder();
            StringBuilder pcieVendorGenerator = new StringBuilder();
            StringBuilder pcieProductGenerator = new StringBuilder();

            // Compose USB vendor names
            foreach ( KeyValuePair<ushort, string> vendorPair in usbVendorNames ) {
                usbVendorGenerator.Append($"            s_knownUsbVendorNames.Add(0x{vendorPair.Key.ToString("X4")}, \"{vendorPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\r\n");
            }

#if false
            // Compose USB product names
            foreach ( KeyValuePair<uint, string> productPair in usbProductNames ) {
                usbProductGenerator.Append($"            s_knownUsbProductNames.Add(0x{productPair.Key.ToString("X8")}, \"{productPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\r\n");
            }
#endif

            // Compose PCIe vendor names
            foreach ( KeyValuePair<ushort, string> vendorPair in pcieVendorNames ) {
                pcieVendorGenerator.Append($"            s_knownPcieVendorNames.Add(0x{vendorPair.Key.ToString("X4")}, \"{vendorPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\r\n");
            }

            // Compose PCIe vendor names
            foreach ( KeyValuePair<uint, string> productPair in pcieProductNames ) {
                pcieProductGenerator.Append($"            s_knownPcieProductNames.Add(0x{productPair.Key.ToString("X8")}, \"{productPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\r\n");
            }

            const string fileData = @"using System.Collections.Generic;

namespace FluentUsbTreeView.UsbTreeView {
    public static partial class UsbDatabase {
        static UsbDatabase () {
            s_knownUsbVendorNames = new Dictionary<ushort, string>();
            s_knownUsbProductNames = new Dictionary<uint, string>();
            s_knownPcieVendorNames = new Dictionary<ushort, string>();
            s_knownPcieProductNames = new Dictionary<uint, string>();

            // USB Vendor names
            #region Known USB Vendor Names
%USBVIDS%            #endregion

            // USB Product names
            #region Known USB Product Names
%USBPIDS%            #endregion

            // PCIe Vendor names
            #region Known PCIe Vendor Names
%PCIEVIDS%            #endregion

            // PCIe PRODUCT names
            #region Known PCIe Product Names
%PCIEPIDS%            #endregion
        }
    }
}
";

            string solutionDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
            string fullPath = Path.GetFullPath(Path.Combine(solutionDir, "FluentUsbTreeView", "UsbTreeView", "UsbDatabase.Data.cs"));
            File.WriteAllText(fullPath, fileData
                .Replace("%USBVIDS%",   usbVendorGenerator.ToString())
                .Replace("%USBPIDS%",   usbProductGenerator.ToString())
                .Replace("%PCIEVIDS%",  pcieVendorGenerator.ToString())
                .Replace("%PCIEPIDS%",  pcieProductGenerator.ToString())
            );
            Console.WriteLine($"Generated successfully at {fullPath}!");
        }

        private static string DownloadString(string url) {
            try {
                string contents;
                using ( HttpClient wc = new() ) {
                    contents = wc.GetStringAsync(url).GetAwaiter().GetResult();
                }

                return contents;
            } catch ( Exception ex ) {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
                return "";
            }
        }
    }
}