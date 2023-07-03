using System;
using System.Collections.Generic;
using HomaGames.HomaBelly.Installer;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class PackageComponent
    {
        public PackageComponent(string id, string version, string url, string changelogUrl, string docUrl,
            EditorPackageType packageType,
            List<string> files, Dictionary<string, string> data)
        {
            Id = id;
            Version = version;
            Url = url;
            PackageType = packageType;
            Files = files;
            Data = data;
        }

        //[JsonProperty("s_package_key")]
        public string Id { get; }

        //[JsonProperty("s_version_number")]
        public string Version { get; }

        //[JsonProperty("s_url")]
        public string Url { get; }

        public string ChangelogUrl { get; } = "";

        public string DocumentationUrl { get; } = "";

        //[JsonProperty("as_files")]
        public List<string> Files { get; }

        //[JsonProperty("o_data")]
        public Dictionary<string, string> Data { get; }
        public EditorPackageType PackageType { get; }

        public string GetName()
        {
            return $"{Id}-{Version}";
        }

        public override string ToString()
        {
            string dataToString = Data != null ? $"\nData: {string.Join(System.Environment.NewLine, Data)}" : "";
            return $"{Id}:{Version}{dataToString}";
        }

        public static PackageComponent FromDictionary(Dictionary<string, object> dictionary,
            EditorPackageType packageType)
        {
            // Files list
            List<string> filePaths = new List<string>();
            List<object> files = dictionary["as_files"] as List<object>;
            for (int i = 0; files != null && i < files.Count; i++)
            {
                filePaths.Add(files[i] as string);
            }

            Dictionary<string, string> data = new Dictionary<string, string>();
            // Optional - data
            if (dictionary.ContainsKey("o_data") && dictionary["o_data"] is Dictionary<string, object> dataDict)
            {
                foreach (KeyValuePair<string, object> entry in dataDict)
                {
                    // Ignore null values
                    if (entry.Value != null)
                    {
                        data.Add(entry.Key, entry.Value.ToString());
                    }
                }
            }

            return new PackageComponent(Convert.ToString(dictionary["s_package_key"]),
                Convert.ToString(dictionary["s_version_number"]),
                Convert.ToString(dictionary["s_url"]),
                dictionary.TryGetValue("s_changelog_url", out object changelog)
                    ? Convert.ToString(changelog)
                    : "",
                dictionary.TryGetValue("s_documentation_url", out object documentation)
                    ? Convert.ToString(documentation)
                    : "",
                packageType,
                filePaths,
                data
            );
        }

        public string GetMainPackageLocalFilePath()
        {
            return $"{Application.dataPath}/../Library/Homa Belly/{Url.Substring(Url.LastIndexOf("/") + 1)}";
        }
    }
}