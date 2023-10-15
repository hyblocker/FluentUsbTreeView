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

            Console.WriteLine("Fetching latest known PCIe devices from PCIe IDs database...");
            string pcieDbKnownPcieVidsRaw = DownloadString(LINUX_KERNEL_KNOWN_PCIE_DEVICES);
            File.WriteAllText("pcie.ids", pcieDbKnownPcieVidsRaw);
            Console.WriteLine("Parsing PCIe IDs...");
            LinuxKernelParser.ParseLinuxKernelUsbIds(pcieDbKnownPcieVidsRaw, pcieVendorNames, pcieProductNames);

            // Final file write
            Console.WriteLine("Generating file...");
            StringBuilder usbVendorGenerator = new StringBuilder();
            StringBuilder usbProductGenerator = new StringBuilder();
            StringBuilder pcieVendorGenerator = new StringBuilder();
            StringBuilder pcieProductGenerator = new StringBuilder();

            // Compose USB vendor names
            foreach ( KeyValuePair<ushort, string> vendorPair in usbVendorNames ) {
                usbVendorGenerator.Append($"            s_knownUsbVendorNames.Add(0x{vendorPair.Key.ToString("X4")}, \"{vendorPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\n");
            }

            // Compose USB product names
            foreach ( KeyValuePair<uint, string> productPair in usbProductNames ) {
                usbProductGenerator.Append($"            s_knownUsbProductNames.Add(0x{productPair.Key.ToString("X8")}, \"{productPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\n");
            }

            // Compose USB vendor names
            foreach ( KeyValuePair<ushort, string> vendorPair in pcieVendorNames ) {
                pcieVendorGenerator.Append($"            s_knownPcieVendorNames.Add(0x{vendorPair.Key.ToString("X4")}, \"{vendorPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\n");
            }

            // Compose PCIe vendor names
            foreach ( KeyValuePair<uint, string> productPair in pcieProductNames ) {
                pcieProductGenerator.Append($"            s_knownPcieProductNames.Add(0x{productPair.Key.ToString("X8")}, \"{productPair.Value.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"")}\");\n");
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

            string fullPath = Path.GetFullPath(@"test.cs");
            File.WriteAllText(fullPath, fileData
                .Replace("%USBVIDS%",   usbVendorGenerator.ToString())
                .Replace("%USBPIDS%",   usbProductGenerator.ToString())
                .Replace("%PCIEVIDS%",  pcieVendorGenerator.ToString())
                .Replace("%PCIEPIDS%",  pcieProductGenerator.ToString())
            );
            Console.WriteLine($"Generated successfully at {fullPath}!");
        }

        private static string DownloadString(string url) {
            string contents;
            using ( HttpClient wc = new() ) {
                contents = wc.GetStringAsync(url).GetAwaiter().GetResult();
            }

            return contents;
        }
    }
}