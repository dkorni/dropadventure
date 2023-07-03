using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public static class AssetUtility
    { 
        public static IEnumerable<I> GetAllImporters<T, I>() where T : Object where I : AssetImporter
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] {"Assets"});
            foreach (var guid in assets)
            {
                var textureImporter = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as TextureImporter;
                if (textureImporter != null && textureImporter is I importer)
                    yield return importer;
            }
        }
    }
}