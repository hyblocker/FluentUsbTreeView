using System;
using System.Collections.Generic;

namespace FluentUsbTreeView.UsbTreeView {
    public static partial class UsbDatabase {

        // Containers for vendor and product names
        private static Dictionary<ushort, string> s_knownUsbVendorNames;
        private static Dictionary<uint, string> s_knownUsbProductNames;
        private static Dictionary<ushort, string> s_knownPcieVendorNames;
        private static Dictionary<uint, string> s_knownPcieProductNames;


        /// <summary>
        /// Returns the USB vendor name from the USB database
        /// </summary>
        /// <param name="vendorId">The given USB device's vendor ID</param>
        /// <returns>The associated name or null if the VID is not in the database</returns>
        public static string GetUsbVendorName(ushort vendorId) {

            if ( s_knownUsbVendorNames.TryGetValue(vendorId, out string result) ) {
                return result;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns the USB product name from the USB database
        /// </summary>
        /// <param name="vendorId">The given USB device's vendor ID</param>
        /// <param name="productId">The given USB device's product ID</param>
        /// <returns>The associated name or null if the VID is not in the database</returns>
        public static string GetUsbProductName(ushort vendorId, ushort productId) {

            // Pack VID and PID into a single int
            uint packedUsbId = vendorId | ( uint ) ( productId << 16 );

            if ( s_knownUsbProductNames.TryGetValue(packedUsbId, out string result)) {
                return result;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns the PCIe vendor name from the PCIe database
        /// </summary>
        /// <param name="vendorId">The given PCIe device's vendor ID</param>
        /// <returns>The associated name or null if the VID is not in the database</returns>
        public static string GetPCIeVendorName(ushort vendorId) {

            if ( s_knownPcieVendorNames.TryGetValue(vendorId, out string result) ) {
                return result;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns the PCIe product name from the PCIe database
        /// </summary>
        /// <param name="vendorId">The given PCIe device's vendor ID</param>
        /// <param name="productId">The given PCIe device's product ID</param>
        /// <returns>The associated name or null if the VID is not in the database</returns>
        public static string GetPCIeProductName(ushort vendorId, ushort productId) {

            // Pack VID and PID into a single int
            uint packedUsbId = vendorId | ( uint ) ( productId << 16 );

            if ( s_knownPcieProductNames.TryGetValue(packedUsbId, out string result) ) {
                return result;
            } else {
                return null;
            }
        }
    }
}
