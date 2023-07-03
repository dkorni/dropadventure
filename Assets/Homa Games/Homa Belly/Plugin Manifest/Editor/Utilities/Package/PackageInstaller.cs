using System.Reflection; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly.Utilities
{
    /// <summary>
    /// Utility class to install `unitypackage` files into current project
    /// </summary>
    public static class PackageInstaller
    {
        private static TaskCompletionSource<bool> _packageImportFinished = new TaskCompletionSource<bool>();
        /// <summary>
        /// Asynchronously installs the package represented by the given PackageComponent.
        /// </summary>
        /// <param name="packageComponent">The PackageComponent to be installed</param>
        /// <returns>Void Task</returns>
        public static async Task InstallPackage(PackageComponent packageComponent)
        {
            HomaBellyEditorLog.Debug($"Installing package: {packageComponent.GetName()}");
            string packagePath = packageComponent.GetMainPackageLocalFilePath();
            _packageImportFinished = new TaskCompletionSource<bool>();
            try
            {
                if (File.Exists(packagePath))
                {
                    // Import all package without showing editor import window
                    RegisterAssetDatabaseCallbacks();
                    if (Application.isBatchMode)
                        typeof(AssetDatabase)
                            .GetMethod("ImportPackageImmediately", BindingFlags.Static | BindingFlags.NonPublic)
                            .Invoke(null, new object[] {packagePath});
                    else
                        AssetDatabase.ImportPackage(packagePath, false);
                    await _packageImportFinished.Task;
                    HomaBellyEditorLog.Debug($"{packageComponent.GetName()} installed");
                }
                else
                {
                    HomaBellyEditorLog.Error(
                        $"Could not install package {packageComponent.GetName()}. File {packagePath} not found.");
                }
            }
            catch (Exception e)
            {
                HomaBellyEditorLog.Error(
                    $"Could not install package {packageComponent.GetName()}. Reason: {e.Message}");
            }
        }

        private static void OnImportPackageFailed(string packageName, string errorMessage)
        {
            HomaBellyEditorLog.Warning($"Could not install package {packageName}. Error: {errorMessage}");
            _packageImportFinished.TrySetResult(true);
            DeregisterAssetDatabaseCallbacks();
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            _packageImportFinished.TrySetResult(true);
            DeregisterAssetDatabaseCallbacks();
        }

        private static void RegisterAssetDatabaseCallbacks()
        {
            AssetDatabase.importPackageCancelled += OnImportPackageCompleted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        private static void DeregisterAssetDatabaseCallbacks()
        {
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= OnImportPackageCompleted;
        }
    }
}
