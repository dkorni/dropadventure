using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MeshSizeIssue : AssetIssue<Mesh,ModelImporter>
    {
        public MeshSizeIssue(ModelImporter assetImporter) : base(assetImporter)
        {
            Mesh m = AssetDatabase.LoadAssetAtPath<Mesh>(assetImporter.assetPath);
            Name = $"Mesh {assetImporter.assetPath} is too big.";
            Description = $"You should consider reducing the size of {assetImporter.assetPath}.\n" +
                          $"It has {m.vertexCount} vertices and is considered too much for mobile platforms.\n\n" +
                          $"You can still ignore this issue if you cannot reduce this mesh size.\n\n" +
                          $"<a href=\"https://docs.unity3d.com/Manual/mesh-compression.html\">Unity Documentation</a>";
            AutomationType = AutomationType.Interactive;
        }

        protected override void FixImporterSettings(ModelImporter importer)
        {
            
        }
    }
}