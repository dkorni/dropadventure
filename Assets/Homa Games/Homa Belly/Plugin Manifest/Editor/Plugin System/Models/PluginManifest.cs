using System;
using System.Collections.Generic;

namespace HomaGames.HomaBelly
{
    public class PluginManifest
    {
        public static PluginManifest Default
            => new PluginManifest("",
                new AvailablePackages(default, default, default, "", "", new List<PackageComponent>()), "");

        public PluginManifest(string appToken, AvailablePackages packages, string json)
        {
            RawJson = json;
            AppToken = appToken;
            Packages = packages;
        }

        public string RawJson { get; }

        public AvailablePackages Packages { get; }
        public string AppToken { get; }

        private static PluginManifestDeserializer pluginManifestDeserializer = new PluginManifestDeserializer();

        public static bool TryGetCurrentlyInstalled(out PluginManifest installedManifest)
        {
            installedManifest = pluginManifestDeserializer.LoadCurrentlyInstalled();
            return installedManifest != null;
        }
        
        /// <summary>
        /// Legacy. <see cref="SaveAsCurrentlyInstalled"/> is preferred.
        /// </summary>
        public static PluginManifest LoadFromLocalFile()
        {
            TryGetCurrentlyInstalled(out PluginManifest pluginManifest);
            return pluginManifest;
        }

        public void SaveAsCurrentlyInstalled()
        {
            PluginManifestDeserializer.SaveAsCurrentlyInstalled(this);
        }

        #region Overrides

        public override string ToString()
        {
            string result = "Plugin Manifest:\n";

            result += Packages != null ? Packages.ToString() : "";

            return result;
        }

        public override bool Equals(object obj)
        {
            // Check for null and compare run-time types.
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }
            PluginManifest other = (PluginManifest) obj;
            return this.Packages.ManifestVersionId == other.Packages.ManifestVersionId;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion
    }
}