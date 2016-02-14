using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TilesDavis.Core
{
    public class IgnoreEntry
    {
        public IgnoreEntry(string name = null, string path = null)
        {
            Name = name;
            Path = path;
            NamePattern = ConvertToRegEx(name);
            PathPattern = ConvertToRegEx(path);
        }

        private string ConvertToRegEx(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? null
                : "^" + Regex.Escape(text).Replace(@"\*", ".*") + "$";
        }

        public string Name { get; private set; }

        public string Path { get; private set; }

        [JsonIgnore]
        public string NamePattern { get; private set; }

        [JsonIgnore]
        public string PathPattern { get; private set; }

        private static readonly JsonSerializerSettings serializerSettings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public static IgnoreEntry ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<IgnoreEntry>(json, serializerSettings);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, serializerSettings);
        }

        public bool IsMatch(string filename)
        {
            var matches = new List<bool>();
            if (Path != null)
            {
                matches.Add(Regex.IsMatch(filename, PathPattern));
            }
            if (Name != null)
            {
                matches.Add(Regex.IsMatch(filename, NamePattern));
            }

            return matches.Any() && matches.TrueForAll(m => m);
        }
    }
}