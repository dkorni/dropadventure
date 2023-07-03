using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly.Installer
{
    public class FileRegistryChecker : AssetPostprocessor
    {
        public static string RegistryFileName => "HomaFileRegistry";
        public static string RegistryFileNameWithExtension => $"{RegistryFileName}.txt";
    
#if !HOMA_BELLY_DEV_ENV
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            CheckRegistries(importedAssets
                .Where(asset => Path.GetFileName(asset) == RegistryFileNameWithExtension));
        }
#endif

        [MenuItem("Window/Homa Games/Homa Belly/Check For Leftover Files", false, 40)]
        public static void CheckAllRegistries()
        {
            if (! CheckRegistries(FileRegistryUtils.FindAllRegistriesInProject()))
            {
                Debug.Log("[Homa Belly] No leftover file found!");
            }
        }

        public static bool CheckRegistries(IEnumerable<string> registries)
        {
            List<string> filePathsToDelete = new List<string>();
            
            foreach (var registry in registries)
            {
                FileFromRegistry[] expectedFiles = FileRegistryUtils.GetRegistryData(registry).ToArray();

                var actualFileGuids = FileRegistryUtils.GetGuidsFor(registry);

                foreach (var guid in actualFileGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (string.IsNullOrEmpty(path)) continue;
                    
                    if (expectedFiles.All(file => file.Guid != guid))
                        filePathsToDelete.Add(path);
                }

            }
            
            if (filePathsToDelete.Count > 0)
            {
                Debug.Log($"Leftover files to delete:\n{string.Join("\n", filePathsToDelete)}");
                
                if (Application.isBatchMode || EditorUtility.DisplayDialog(
                        "Old files remaining",
                        "After the import, some old files have been found and need to be deleted.",
                        "Delete them", "Leave them"))
                {
                    foreach (var filePathToDelete in filePathsToDelete)
                    {
                        AssetDatabase.DeleteAsset(filePathToDelete);
                    }
                }

                return true;
            }

            return false;
        }
    }
}