using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbDatabaseGenerator {
    public static class LinuxKernelParser {
        public static void ParseLinuxKernelUsbIds(string inTxt, Dictionary<ushort, string> vendorNames, Dictionary<uint, string> productNames) {

            // Don't parse past this string
            int maxIndex = inTxt.IndexOf("# List of known device classes, subclasses and");

            StringBuilder tempStringBuilder = new StringBuilder();
            bool isComment = false;
            ushort currentVendorId = 0xFFFF;
            for ( int i = 0; i < maxIndex; i++ ) {
                if ( !( char.IsWhiteSpace(inTxt[i]) || inTxt[i] == '\n' ) ) {
                    // Not space or EOL
                    // Check if it's a comment
                    if ( inTxt[i] == '#' ) {
                        isComment = true;
                        continue;
                    }
                }

                if ( inTxt[i] != '\n' ) {
                    // Build up buffer until EOL
                    if ( !isComment )
                        tempStringBuilder.Append(inTxt[i]);

                    // Since not EOL don't bother parsing yet
                    continue;
                }

                if ( isComment ) {
                    isComment = false;
                    continue;
                }

                // this is the line we have to work with
                string currentLine = tempStringBuilder.ToString();
                tempStringBuilder.Clear();

                // skip if whitespace
                if ( currentLine.Trim().Length == 0 )
                    continue;

                bool isVendor = false;
                bool isDevice = false;
                bool isSubdevice = false;
                bool isMalformed = false;

                // Get what we are parsing
                if ( currentLine[0] != '\t' ) {
                    isVendor = true;
                } else if ( currentLine[0] == '\t' && currentLine[1] != '\t' ) {
                    isDevice = true;
                } else if ( currentLine[0] == '\t' && currentLine[1] == '\t' ) {
                    isSubdevice = true;
                } else {
                    isMalformed = true;
                }

                // Handle vendors
                if (isVendor) {
                    string vendorPart = currentLine.Substring(0, 4);
                    ushort vendorIdParsed = Convert.ToUInt16(vendorPart, 16);
                    string vendorName = currentLine.Substring(5).Trim();

                    // Append to vendor ids list
                    if (!vendorNames.ContainsKey(vendorIdParsed) ) {
                        vendorNames.Add(vendorIdParsed, vendorName);
                    }

                    currentVendorId = vendorIdParsed;
                }

                // Handle devices
                if ( isDevice ) {
                    string productPart = currentLine.Substring(1, 4);
                    ushort productIdParsed = Convert.ToUInt16(productPart, 16);
                    string productName = currentLine.Substring(6).Trim();

                    // Pack VID and PID into a single int
                    uint packedVidPid = currentVendorId | ( uint ) ( productIdParsed << 16 );

                    // Append to vendor ids list
                    if ( !productNames.ContainsKey(packedVidPid) ) {
                        productNames.Add(packedVidPid, productName);
                    }
                }

                isComment = false;
            }
        }
    }
}
