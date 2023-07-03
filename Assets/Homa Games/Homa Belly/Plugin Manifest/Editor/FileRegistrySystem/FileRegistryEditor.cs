#if HOMA_BELLY_DEV_ENV
using System.IO;
using UnityEditor;

namespace HomaGames.HomaBelly.Installer
{
    public class FileRegistryEditor : AssetPostprocessor
    {
        [MenuItem("Assets/Create New File Registry")]
        public static void CreateList()
        {
            string directory = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (directory == "")
            {
                directory = "Assets";
            }
            else if (Path.GetExtension(directory) != "")
            {
                directory = Path.GetDirectoryName(directory);
            }
            
            CreateNewFileRegistryIn(directory);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            UpdateRegistries();
        }
        
        [MenuItem("Window/Homa Games/Homa Belly/Update Registries", false, 41)]
        [InitializeOnLoadMethod]
        public static void UpdateRegistries()
        {
            foreach (var registry in FileRegistryUtils.FindAllRegistriesInProject())
            {
                UpdateRegistry(registry);
            }
            AssetDatabase.Refresh();
        }
    
        private static void CreateNewFileRegistryIn(string registryDirectory)
        {
            var registryPath = Path.Combine(registryDirectory, FileRegistryChecker.RegistryFileNameWithExtension);
            UpdateRegistry(registryPath);
            AssetDatabase.Refresh();
        }

        private static void UpdateRegistry(string registryPath)
        {
            FileRegistryUtils.AssertIsFileRegistry(registryPath);

            string registryText = FileRegistryUtils.GenerateRegistryText(registryPath);
            
            if (!File.Exists(registryPath) || File.ReadAllText(registryPath) != registryText)
                File.WriteAllText(registryPath, registryText);
        }
    }
}

#endif
