using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MeshReadWriteCheck : AssetCheck<Mesh, ModelImporter>
    {
        public MeshReadWriteCheck() : base("Mesh Read/Write enabled",
            "Usually, almost all meshes in your game don't need read/write enabled. You only need to enable it when you need to read the mesh at runtime. You can read the specific scenarios" +
            "<a href=\"https://docs.unity3d.com/ScriptReference/Mesh-isReadable.html\">here.</a>.",
            new HashSet<string>() {"mesh", "build", "size", "memory"}, ImportanceType.Advised, Priority.High)
        {
        }

        protected override bool AssetHasIssue(ModelImporter assetImporter, out IIssue issue)
        {
            if (assetImporter.isReadable)
            {
                issue = new MeshReadWriteIssue(assetImporter);
                return true;
            }

            issue = null;
            return false;
        }
    }
}