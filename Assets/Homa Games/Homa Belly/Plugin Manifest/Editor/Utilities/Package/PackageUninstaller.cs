using System;
using System.Collections.Generic;
using UnityEditor;

namespace HomaGames.HomaBelly.Utilities
{
    /// <summary>
    /// Utility class to uninstall any package not present
    /// in the new plugin manifest but installed in previous integrations.
    /// </summary>
    public static class PackageUninstaller
    {
        public static void UninstallAllPackages(PluginManifest manifest)
        {
            if (manifest != null && manifest.Packages != null)
            {
                List<PackageComponent> packages = manifest.Packages.GetAllPackages();
                for (int i = 0; i < packages.Count; i++)
                {
                    UninstallPackage(packages[i]);
                }
            }
        }

        public static void UninstallPackage(PackageComponent packageComponent)
        {
            HomaBellyEditorLog.Debug($"Uninstalling package: {packageComponent.GetName()}");
            if (packageComponent.Files != null)
            {
                foreach (string filePath in packageComponent.Files)
                {
                    DeleteAsset(filePath);
                }
            }
        }

        private static void DeleteAsset(string assetPath)
        {
            string assetWithoutPrefix = PackageCommon.GetAssetWithoutPrefix(assetPath);
            HomaBellyEditorLog.Debug($"Deleting file/directory at {assetWithoutPrefix}");
            try
            {
                // Delete file and .meta
                bool result = FileUtil.DeleteFileOrDirectory(assetWithoutPrefix);
                result &= FileUtil.DeleteFileOrDirectory(assetWithoutPrefix + ".meta");
                if (result)
                {
                    HomaBellyEditorLog.Debug($"{assetWithoutPrefix} deleted");
                }
                else
                {
                    HomaBellyEditorLog.Warning($"Could not delete {assetWithoutPrefix}");
                }
            }
            catch (Exception e)
            {
                HomaBellyEditorLog.Warning($"Could not delete file/directory. Reason: {e.Message}");
            }
        }
    }
}
