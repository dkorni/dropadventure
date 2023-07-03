using System;
using System.Collections.Generic;
using System.IO;
using HomaGames.HomaBelly.Installer;
using HomaGames.HomaBelly.Installer.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class PluginManifestDeserializer : IModelDeserializer<PluginManifest>
    {
        public const string MANIFEST_FILE_BASE_PATH =
            "/Homa Games/Homa Belly/Plugin Manifest/Editor/Plugin System/PluginManifest.json";
        public static readonly string LOCAL_PLUGIN_MANIFEST_FILE = Application.dataPath + MANIFEST_FILE_BASE_PATH;
        public static readonly string MANIFEST_FILE_PROJECT_PATH = "Assets" + MANIFEST_FILE_BASE_PATH;

        public PluginManifest Deserialize(string json)
        {
            // Basic info
            if (!string.IsNullOrEmpty(json) && InstallerJson.Deserialize(json) is Dictionary<string, object> dictionary)
            {
                List<PackageComponent> availablePackages = new List<PackageComponent>();
                // Packages
                if (dictionary["res"] is Dictionary<string, object> packagesDictionary)
                {
                    // Core packages
                    availablePackages.AddRange(ExtractPackagesFromDictionary(packagesDictionary, "ao_core_packages",
                        EditorPackageType.CORE_PACKAGE));

                    // Mediation layers
                    availablePackages.AddRange(ExtractPackagesFromDictionary(packagesDictionary, "ao_mediation_layers",
                        EditorPackageType.MEDIATION_LAYER));

                    // Attribution platforms
                    availablePackages.AddRange(ExtractPackagesFromDictionary(packagesDictionary,
                        "ao_attribution_platforms", EditorPackageType.ATTRIBUTION_PLATFORM));

                    // Analytics systems
                    availablePackages.AddRange(ExtractPackagesFromDictionary(packagesDictionary, "ao_analytics_systems",
                        EditorPackageType.ANALYTICS_SYSTEM));

                    // Ad networks
                    availablePackages.AddRange(ExtractPackagesFromDictionary(packagesDictionary, "ao_ad_networks",
                        EditorPackageType.AD_NETWORK));

                    // Others
                    availablePackages.AddRange(ExtractPackagesFromDictionary(packagesDictionary, "ao_others",
                        EditorPackageType.OTHER));


                    AvailablePackages packages = new AvailablePackages(
                        Convert.ToInt32(packagesDictionary["i_manifest_id"]),
                        Convert.ToString(packagesDictionary["s_manifest_name"]),
                        packagesDictionary.ContainsKey("s_manifest_version_id")
                            ? Convert.ToString(packagesDictionary["s_manifest_version_id"])
                            : "", (string) packagesDictionary["s_android_bundle_id"],
                        (string) packagesDictionary["s_ios_bundle_id"], availablePackages);

                    PluginManifest manifest = new PluginManifest((string) dictionary["ti"], packages, json);

                    // Store mandatory values for ApiQueries
                    EditorAnalyticsProxy.SetTokenIdentifier(manifest.AppToken);
                    EditorAnalyticsProxy.SetManifestVersionId(manifest.Packages?.ManifestVersionId);

                    return manifest;
                }
            }


            return PluginManifest.Default;
        }

        private List<PackageComponent> ExtractPackagesFromDictionary(Dictionary<string, object> config,
            string packageTypeKey, EditorPackageType packageType)
        {
            List<PackageComponent> extractedPackageComponents = new List<PackageComponent>();
            if (config.ContainsKey(packageTypeKey) && config[packageTypeKey] is List<object> package)
            {
                for (int i = 0; i < package.Count; i++)
                {
                    Dictionary<string, object> dict = package[i] as Dictionary<string, object>;
                    extractedPackageComponents.Add(PackageComponent.FromDictionary(dict, packageType));
                }
            }

            return extractedPackageComponents;
        }

        [CanBeNull] private static PluginManifest _currentlyInstalled;

        /// <summary>
        /// Gets the currently installed manifest.
        /// </summary>
        public PluginManifest LoadCurrentlyInstalled()
        {
            try
            {
                if (_currentlyInstalled != null)
                    return _currentlyInstalled;

                string legacyManifestPath = "Library/Homa Belly/plugin_manifest.json";
                string manifestJson = null;
                if (File.Exists(LOCAL_PLUGIN_MANIFEST_FILE))
                {
                    manifestJson = File.ReadAllText(LOCAL_PLUGIN_MANIFEST_FILE);
                }
                // Check if legacy JSON file exists
                else if (File.Exists(legacyManifestPath))
                {
                    // Dump to new file and remove old one
                    manifestJson = File.ReadAllText(legacyManifestPath);
                    File.WriteAllText(LOCAL_PLUGIN_MANIFEST_FILE, manifestJson);
                    File.Delete(legacyManifestPath);
                }

                if (!string.IsNullOrEmpty(manifestJson))
                    _currentlyInstalled = Deserialize(manifestJson);

                return _currentlyInstalled;
            }
            catch (Exception e)
            {
                HomaBellyEditorLog.Warning($"[Plugin Manifest] Could not load plugin_manifest.json. Reason: {e}");
            }

            return null;
        }

        #region Private helpers

        public static void SaveAsCurrentlyInstalled(PluginManifest pluginManifest)
        {
            try
            {
                if (pluginManifest == null)
                {
                    File.Delete(LOCAL_PLUGIN_MANIFEST_FILE);
                }
                else
                {
                    EditorFileUtilities.CreateIntermediateDirectoriesIfNecessary(LOCAL_PLUGIN_MANIFEST_FILE);
                    File.WriteAllText(LOCAL_PLUGIN_MANIFEST_FILE, pluginManifest.RawJson);
                }
                _currentlyInstalled = pluginManifest;
            }
            catch (Exception e)
            {
                HomaBellyEditorLog.Warning(
                    $"[Plugin Manifest] Could not save plugin_manifest.json. Reason: {e.Message}");
            }
        }

        #endregion
    }
}