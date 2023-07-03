using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MeshReadWriteIssue : AssetIssue<Mesh, ModelImporter>
    {
        public MeshReadWriteIssue(ModelImporter assetImporter) : base(assetImporter)
        {
            Name = $"{assetImporter.assetPath} shouldn't be writable";
            Description =
                $"The asset {assetImporter.assetPath} should not be writable except in some really specific scenarios.";
            AutomationType = AutomationType.Automatic;
        }

        protected override void FixImporterSettings(ModelImporter importer)
        {
            importer.isReadable = false;
        }
    }
}