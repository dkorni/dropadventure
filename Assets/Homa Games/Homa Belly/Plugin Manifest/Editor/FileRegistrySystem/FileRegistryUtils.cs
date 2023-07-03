using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly.Installer
{
    public static class FileRegistryUtils
    {
        public static void AssertIsFileRegistry(string registryPath)
        {
            if ( Path.GetFileName(registryPath) != FileRegistryChecker.RegistryFileNameWithExtension)
                throw new ArgumentException(
                    $"registry file \"{registryPath}\" must be a \"{FileRegistryChecker.RegistryFileNameWithExtension}\" file.");
        }

        public static string GetRegistryDirectory(string registryPath)
        {
            AssertIsFileRegistry(registryPath);
            
            var registryDirectory = Path.GetDirectoryName(registryPath);
            if (registryDirectory == null)
                throw new ArgumentException($"invalid registry path: \"{registryPath}\"");

            return registryDirectory;
        }

        public static IEnumerable<FileFromRegistry> GetRegistryData(string registryPath)
        {
            foreach (var registryLine in File.ReadAllLines(registryPath))
            {
                yield return new FileFromRegistry(registryLine);
            }
        }

        public static string[] GetGuidsFor(string registryPath)
        {
            Func<string, bool> assetPathFilter = CreateAssetPathFilterFor(registryPath);
            return AssetDatabase.FindAssets(string.Empty, new[] {GetRegistryDirectory(registryPath)})
                .Where(guid => assetPathFilter(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
        
        public static string GenerateRegistryText(string registryPath)
        {
            string registryDirectory = GetRegistryDirectory(registryPath);
            
            string[] assetGuidList = AssetDatabase.FindAssets(string.Empty, new[] {registryDirectory});

            Func<string, bool> assetPathFilter = CreateAssetPathFilterFor(registryPath);
            IEnumerable<FileFromRegistry> registryFiles = assetGuidList
                .Select(guid => new FileFromRegistry(guid))
                .Where(file => assetPathFilter(AssetDatabase.GUIDToAssetPath(file.Guid)));

            return string.Join("\n", registryFiles.Select(file => file.ToRegistryLine()));
        }

        public static IEnumerable<string> FindAllRegistriesInProject()
        {
            var registryGuids = AssetDatabase.FindAssets($"{FileRegistryChecker.RegistryFileName} t:{nameof(TextAsset)}");

            foreach (var registryGuid in registryGuids)
            {
                yield return AssetDatabase.GUIDToAssetPath(registryGuid);
            }
        }

        private static Func<string, bool> CreateAssetPathFilterFor(string registryPath)
        {
            string registryDirectory = GetRegistryDirectory(registryPath);
            return path => 
                path != registryPath
                && path != PluginManifestDeserializer.MANIFEST_FILE_PROJECT_PATH
                && PathStartsWith(path, registryDirectory)
                && !Directory.Exists(path);
        }


        public static bool PathStartsWith(string mainPath, string startPath)
        {
            return StandardizePath(mainPath).StartsWith(StandardizePath(startPath));
        }
        
        private static string StandardizePath(string path)
            => path.Replace('\\', '/');
    }
}