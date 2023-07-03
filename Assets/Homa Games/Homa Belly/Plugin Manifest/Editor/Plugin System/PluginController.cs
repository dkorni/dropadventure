using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Installer.Utilities;
using HomaGames.HomaBelly.Utilities;
using UnityEditor;

namespace HomaGames.HomaBelly
{
    public static class PluginController
    {
        private static InstallerHttpCaller<PluginManifest> installerHttpCaller =
            new InstallerHttpCaller<PluginManifest>();

        private static PluginManifestDeserializer pluginManifestDeserializer = new PluginManifestDeserializer();


        #region Public methods

        public static async Task<PluginManifest> RequestPluginManifest(string appToken)
        {
            return await installerHttpCaller.Get(
                    string.Format(HomaBellyInstallerConstants.API_APP_BASE_URL, appToken), pluginManifestDeserializer);
        }

        public static async Task InstallPluginManifest(PluginManifest newPluginManifest, Action<float, string> onProgress = null)
        {
            await InstallPluginManifest(newPluginManifest, CancellationToken.None, onProgress);
        }

        public static async Task InstallPluginManifest(PluginManifest newPluginManifest,
            CancellationToken cancellationToken, Action<float, string> onProgress = null)
        {
            EditorAnalyticsProxy.TrackEditorAnalyticsEvent("manifest_installation_started");

            if (newPluginManifest != null)
            {
                onProgress?.Invoke(0, "");

                PluginManifest.TryGetCurrentlyInstalled(out PluginManifest currentManifest);
                var pluginManifestDiff =
                    new PluginManifestDiff(currentManifest, newPluginManifest);

                int diffItemDone = 0;
                // New packages need to be downloaded and installed so the count for 2 items
                int totalItems = pluginManifestDiff.NewPackageVersions.Count * 2
                                 + pluginManifestDiff.RemovedPackages.Count;

                HomaBellyEditorLog.Debug($"{newPluginManifest}");

                // Lock reload assemblies while installing packages
                EditorApplication.LockReloadAssemblies();

                // Uninstall old packages
                foreach (var removed in pluginManifestDiff.RemovedPackages)
                {
                    PackageUninstaller.UninstallPackage(removed.Value);
                    diffItemDone++;
                    onProgress?.Invoke((float) diffItemDone / totalItems, $"Deleting {removed.Value.Id} v{removed.Value.Version}");
                    await Task.Delay(100, cancellationToken);
                }

                // Download packages from new manifest
                foreach (var newPackage in pluginManifestDiff.NewPackageVersions)
                {
                    await PackageDownloader.DownloadPackage(newPackage.Value);
                    cancellationToken.ThrowIfCancellationRequested();
                    diffItemDone++;
                    onProgress?.Invoke((float) diffItemDone / totalItems,
                        $"Downloading {newPackage.Value.Id} v{newPackage.Value.Version}");
                }

                // Install packages from new manifest
                foreach (var newPackage in pluginManifestDiff.NewPackageVersions)
                {
                    await PackageInstaller.InstallPackage(newPackage.Value);
                    await Task.Delay(100, cancellationToken);
                    diffItemDone++;
                    onProgress?.Invoke((float) diffItemDone / totalItems,
                        $"Installing {newPackage.Value.Id} v{newPackage.Value.Version}");
                }

                // Try reinstalling packages with missing files
                foreach (var failedPackage in newPluginManifest.Packages.GetAllPackages()
                             .FindAll(p => !PackageCommon.IsPackageInstalled(p)))
                {
                    await PackageInstaller.InstallPackage(failedPackage);
                    await Task.Delay(100, cancellationToken);
                    onProgress?.Invoke((float) diffItemDone / totalItems,
                        $"Try Reinstalling {failedPackage.Id} v{failedPackage.Version}");
                }

                onProgress?.Invoke(1, "");
                // Unlock reload assemblies and refresh AssetDatabase
                EditorApplication.UnlockReloadAssemblies();

                // Save the new manifest as the currently installed manifest
                newPluginManifest.SaveAsCurrentlyInstalled();
            }

            AssetDatabase.Refresh();
        }

        public static void UninstallAllPackages()
        {
            if (PluginManifest.TryGetCurrentlyInstalled(out PluginManifest pluginManifest))
            {
                PluginManifestDeserializer.SaveAsCurrentlyInstalled(null);
                PackageUninstaller.UninstallAllPackages(pluginManifest);
            }
        }

        #endregion
    }
}