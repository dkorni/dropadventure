using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class TextureSizeIssue : AssetIssue<Texture2D, TextureImporter>
    {
        public TextureSizeIssue(TextureImporter textureImporter) : base(textureImporter)
        {
            Name = $"Texture {textureImporter.assetPath} has wrong size.";
            Description = $"Texture {textureImporter.assetPath} has a size > {TextureSizeCheck.MaxAllowedSize}, this is not needed for mobile games.";
            AutomationType = AutomationType.Automatic;
        }

        protected override void FixImporterSettings(TextureImporter importer)
        {
            var androidTextureImporter = importer.GetPlatformTextureSettings("Android");
            var iosTextureImporter = importer.GetPlatformTextureSettings("iPhone");
            importer.maxTextureSize = TextureSizeCheck.MaxAllowedSize;
            androidTextureImporter.maxTextureSize = TextureSizeCheck.MaxAllowedSize;
            iosTextureImporter.maxTextureSize = TextureSizeCheck.MaxAllowedSize;
            importer.SetPlatformTextureSettings(androidTextureImporter);
            importer.SetPlatformTextureSettings(iosTextureImporter);
        }
    }
}