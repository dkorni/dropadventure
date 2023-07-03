using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Installer.Utilities;
using UnityEngine;

namespace HomaGames.HomaBelly.Utilities
{
    /// <summary>
    /// Utility class to download packages to a library folder
    /// </summary>
    public static class PackageDownloader
    {
        private static InstallerHttpCaller<string> httpCaller = new InstallerHttpCaller<string>();
        
        /// <summary>
        /// Downloads the specified package
        /// </summary>
        /// <param name="packages"></param>
        /// <returns></returns>
        public static async Task DownloadPackage(PackageComponent package,bool forceDownload = false)
        {
            await DownloadPackage(package.Url, package.GetMainPackageLocalFilePath(),forceDownload);
        }

        /// <summary>
        /// Clear all the locally downloaded packages for a manifest.
        /// </summary>
        public static void ClearPackageCache(PluginManifest pluginManifest)
        {
            foreach (var package in pluginManifest.Packages.GetAllPackages())
            {
                ClearPackageCache(package);
            }
        }
        
        /// <summary>
        /// Clear the locally downloaded package.
        /// </summary>
        public static void ClearPackageCache(PackageComponent package)
        {
            var path = package.GetMainPackageLocalFilePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Asynchronously downloads the available package in <paramref name="packageUri"/>, saving
        /// it in <see cref="TMP_PACKAGE_DOWNLOAD_DIR"/> folder
        /// </summary>
        /// <param name="packageUri">The uri hosting the package</param>
        /// <param name="forceDownload">(Optional) True to force download</param>
        /// <returns>A Task with the saved file path</returns>
        private static async Task<string> DownloadPackage(string packageUri, string outputFilePath, bool forceDownload = false)
        {
            if (!File.Exists(outputFilePath) || forceDownload)
            {
                EditorFileUtilities.CreateIntermediateDirectoriesIfNecessary(outputFilePath);
                HomaBellyEditorLog.Debug($"Downloading package: {packageUri}");
                try
                {
                    // Trigger package download
                    string packagePath = await httpCaller.DownloadFile(packageUri, outputFilePath);
                    HomaBellyEditorLog.Debug($"Package available at: {packagePath}");
                    return packagePath;
                }
                catch (Exception e)
                {
                    EditorAnalyticsProxy.TrackEditorAnalyticsEvent("error_downloading_package", e.Message);
                    HomaBellyEditorLog.Warning($"Could not download package at: {packageUri}. Error: {e.Message}");
                    return "";
                }
            }
            else
            {
                // Package already downloaded, notify callback
                HomaBellyEditorLog.Debug($"Package already downloaded. Available at: {outputFilePath}");
                return outputFilePath;
            }
        }
    }
}
