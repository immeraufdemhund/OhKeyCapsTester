using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using OhKeyCapsTester.Contracts;

namespace OhKeyCapsTester.Core
{
    internal class KeyboardReaderService : IKeyboardReaderService
    {
        public IEnumerable<KeyboardLayout> LoadKeyboardLayouts()
        {
            return Directory.EnumerateFiles("Keyboards", "info.json", SearchOption.AllDirectories)
                .Select(File.ReadAllText)
                .Select(JObject.Parse)
                .Select(ParseKeyboardLayout);
        }

        private static KeyboardLayout ParseKeyboardLayout(JObject j)
        {
            return new KeyboardLayout
            {
                KeyboardName = j["keyboard_name"].Value<string>(),
                Rows = j["rows"].Value<int>(),
                Cols = j["cols"].Value<int>(),
                Width = j["width"].Value<int>(),
                Height = j["height"].Value<int>(),
                Layouts = ParseKeyboardLayoutCollection(j["layouts"])
            };
        }

        private static KeyLayoutCollection ParseKeyboardLayoutCollection(JToken jToken)
        {
            return new KeyLayoutCollection
            {
                Layout = jToken.Select(ParseKeyLayout).ToArray()
            };
        }

        private static KeyLayout ParseKeyLayout(JToken jToken)
        {
            return new KeyLayout
            {
                Name = ((JProperty)jToken).Name,
                Layout = ParseKeyCoordinates((JArray)jToken.First["layout"]),
            };
        }

        private static KeyLayoutCoordinate[] ParseKeyCoordinates(JArray jArray)
        {
            return jArray.Select(x => new KeyLayoutCoordinate
            {
                Label = x["label"].Value<string>(),
                X = x["x"].Value<int>(),
                H = x["h"]?.Value<int>() ?? 1,
                Y = x["y"].Value<int>()
            }).ToArray();
        }
    }
}
