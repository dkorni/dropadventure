using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MeshSizeCheck : AssetCheck<Mesh, ModelImporter>
    {
        public const int MAX_ALLOWED_VERTICES = 65535;

        public MeshSizeCheck() : base("Mesh Size",
            "Models with a lot of vertices and a lot of information per vertex are expensive to render, and they can take a big chunk of memory.\n" +
            "You can reduce the size of those meshes, tweaking the import settings:\n" +
            "If you are certain the mesh’s material will not need normals or tangents, uncheck these options for extra savings.\n\n" +
            "If this isn't enough, you should consider cutting down models in your DCC package of choice.\n" +
            "Delete unseen polygons from the camera’s point of view. Use textures and normal maps for fine detail instead of high-density meshes.",
            new HashSet<string>() {"mesh", "memory", "fps", "build", "size"}, ImportanceType.Advised, Priority.High)
        {
            
        }

        protected override bool AssetHasIssue(ModelImporter assetImporter, out IIssue issue)
        {
            Mesh m = AssetDatabase.LoadAssetAtPath<Mesh>(assetImporter.assetPath);
            if (m && m.vertexCount > MAX_ALLOWED_VERTICES)
            {
                issue = new MeshSizeIssue(assetImporter);
                return true;
            }

            issue = null;
            return false;
        }
    }
}