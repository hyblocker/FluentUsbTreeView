using Newtonsoft.Json;

namespace UsbDatabaseGenerator {

    public static class UsbForumVidParser {
        private struct UsbForumRoot {
            public string name { get; set; }
            public string field_vid { get; set; }

            public override string ToString() {
                return $"{field_vid} : {name}";
            }
        }

        public static void ParseUsbForumVids(string inJson, Dictionary<ushort, string> vendorNames) {

            List<UsbForumRoot> deserialisedUsbData = JsonConvert.DeserializeObject<List<UsbForumRoot>>(inJson);

            foreach (var pair in deserialisedUsbData) {

                if ( pair.field_vid == "1010101010" ) // TEST USB-IF Member
                    continue;
                // might have duplicates for some reason
                ushort vid_as_number = ushort.Parse(pair.field_vid);
                vendorNames.TryAdd(vid_as_number, pair.name);
            }
        }
    }
}
