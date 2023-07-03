using System.IO;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public static class ConfigurationBuilder
    {
        [InitializeOnLoadMethod]
        public static void Build()
        {
            Build(false);
        }

        public static void Build(bool forceBuilding)
        {
            if (Application.isBatchMode && !forceBuilding) return;
            
            // building the config while the asset database is not loaded completely yet may 
            // wipe the values
            if (! CanManifestConfigurationBeImportedCorrectly())
                return;
            
            HomaBellyManifestConfiguration homaBellyManifestConfiguration = GetOrCreateAsset();
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                FileUtil.GetProjectRelativePath(PluginManifestDeserializer.LOCAL_PLUGIN_MANIFEST_FILE));
            var currentPluginManifestHash = textAsset ? textAsset.text.GetHashCode() : 0;
            if (currentPluginManifestHash == homaBellyManifestConfiguration.PluginManifestHash) return;
            var manifest = PluginManifest.LoadFromLocalFile();
            if (manifest == null) return;
            
            homaBellyManifestConfiguration.SetEntry(manifest.AppToken, HomaBellyManifestConfiguration.MANIFEST_TOKEN_KEY);
            homaBellyManifestConfiguration.SetEntry(manifest.Packages?.ManifestVersionId, HomaBellyManifestConfiguration.MANIFEST_VERSION_ID_KEY);
            foreach (var component in manifest.Packages.GetAllPackages())
            {
                // Not all components may have Data, so ignore empty/null ones
                if (component.Data != null && component.Data.Count > 0)
                {
                    foreach (var data in component.Data)
                    {
                        homaBellyManifestConfiguration.SetEntry(data.Value, component.Id, data.Key);
                    }
                }
            }

            EditorUtility.SetDirty(homaBellyManifestConfiguration);
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
            homaBellyManifestConfiguration.PluginManifestHash = currentPluginManifestHash;
        }

        /// <summary>
        /// Checks if there is no discrepancy between the content of the AssetDatabase and the project for
        /// the file at <see cref="HomaBellyManifestConfiguration.CONFIG_FILE_PROJECT_PATH"/>.
        /// </summary>
        /// <returns></returns>
        private static bool CanManifestConfigurationBeImportedCorrectly()
        {
            bool assetExists = AssetDatabase.LoadAssetAtPath<HomaBellyManifestConfiguration>(
                HomaBellyManifestConfiguration.CONFIG_FILE_PROJECT_PATH);
            bool fileExists = File.Exists(HomaBellyManifestConfiguration.CONFIG_FILE_PROJECT_PATH);
            return assetExists == fileExists;
        }

        private static HomaBellyManifestConfiguration GetOrCreateAsset()
        {
            var asset = AssetDatabase.LoadAssetAtPath<HomaBellyManifestConfiguration>(
                HomaBellyManifestConfiguration.CONFIG_FILE_PROJECT_PATH);
            if (asset)
                return asset;
            FileUtilities.CreateIntermediateDirectoriesIfNecessary(HomaBellyManifestConfiguration.CONFIG_FILE_PROJECT_PATH);
            var newAsset = ScriptableObject.CreateInstance<HomaBellyManifestConfiguration>();
            AssetDatabase.CreateAsset(newAsset, HomaBellyManifestConfiguration.CONFIG_FILE_PROJECT_PATH);
            return newAsset;
        }
    }
}