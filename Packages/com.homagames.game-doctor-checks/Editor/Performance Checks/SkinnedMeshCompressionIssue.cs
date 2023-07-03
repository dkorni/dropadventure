using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class SkinnedMeshCompressionIssue : AssetIssue<Mesh,ModelImporter>
    {
        public SkinnedMeshCompressionIssue(ModelImporter assetImporter) : base(assetImporter)
        {
            Name = $"Wrong Compression for {assetImporter.assetPath}";
            Description = $"The compression for asset {assetImporter.assetPath} should be enabled.\n" +
                          $"<a href=\"https://docs.unity3d.com/Manual/mesh-compression.html\">Unity Documentation</a>";
            AutomationType = AutomationType.Automatic;
        }

        protected override void FixImporterSettings(ModelImporter importer)
        {
            importer.meshCompression = ModelImporterMeshCompression.Medium;
        }
    }
}