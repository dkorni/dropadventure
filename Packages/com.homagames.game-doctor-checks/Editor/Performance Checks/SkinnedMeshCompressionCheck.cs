using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class SkinnedMeshCompressionCheck : AssetCheck<Mesh, ModelImporter>
    {
        public SkinnedMeshCompressionCheck() : base("Skinned Meshes Compression",
            "Skinned meshes should have compression enabled to reduce file size.\n" +
            "Despite this can increase loading times, usually we don't have too much skinned meshes in the scene. If you have a lot of skinned meshes, and you want to reduce loading times to the minimum, you should consider ignoring this issue.\n\n" +
            "<a href=\"https://docs.unity3d.com/Manual/mesh-compression.html\">Unity Documentation</a>",
            new HashSet<string>() {"mesh", "build", "size"}, ImportanceType.Advised, Priority.Medium)
        {
        }

        protected override bool AssetHasIssue(ModelImporter assetImporter, out IIssue issue)
        {
            if (assetImporter.animationType != ModelImporterAnimationType.None &&
                assetImporter.meshCompression == ModelImporterMeshCompression.Off)
            {
                issue = new SkinnedMeshCompressionIssue(assetImporter);
                return true;
            }

            issue = null;
            return false;
        }
    }
}