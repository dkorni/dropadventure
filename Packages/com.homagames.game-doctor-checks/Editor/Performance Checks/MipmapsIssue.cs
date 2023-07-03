using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MipmapsIssue : AssetIssue<Texture2D,TextureImporter>
    {
        public MipmapsIssue(TextureImporter textureImporter) : base(textureImporter)
        {
            Name = $"Texture {textureImporter.assetPath} shouldn't use mipmaps.";
            Description = $"Texture {textureImporter.assetPath} is using mipmaps but this is usually not needed for mobile games.";
            AutomationType = AutomationType.Automatic;
        }

        protected override void FixImporterSettings(TextureImporter importer)
        {
            importer.mipmapEnabled = false;
        }
    }
}