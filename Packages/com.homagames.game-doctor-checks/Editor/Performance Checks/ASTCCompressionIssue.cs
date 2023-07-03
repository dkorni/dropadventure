using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class ASTCCompressionIssue : AssetIssue<Texture2D,TextureImporter>
    {
        public ASTCCompressionIssue(TextureImporter textureImporter) : base(textureImporter)
        {
            Name = $"Texture {textureImporter.assetPath} is not ASTC";
            Description = $"Texture {textureImporter.assetPath} should use ASTC compression.";
            AutomationType = AutomationType.Automatic;
        }

        protected override void FixImporterSettings(TextureImporter importer)
        {
            var androidTextureImporter = importer.GetPlatformTextureSettings("Android");
            var iosTextureImporter = importer.GetPlatformTextureSettings("iPhone");
            androidTextureImporter.format = TextureImporterFormat.ASTC_8x8;
            iosTextureImporter.format = TextureImporterFormat.ASTC_8x8;
            androidTextureImporter.overridden = true;
            iosTextureImporter.overridden = true;
            importer.SetPlatformTextureSettings(androidTextureImporter);
            importer.SetPlatformTextureSettings(iosTextureImporter);
        }
    }
}