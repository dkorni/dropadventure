using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class TextureReadableIssue : AssetIssue<Texture2D,TextureImporter>
    {
        public TextureReadableIssue(TextureImporter textureImporter) : base(textureImporter)
        {
            Name = $"Texture {textureImporter.assetPath} is readable.";
            Description = $"Texture {textureImporter.assetPath} shouldn't be readable. Textures with Read/Write option enabled are taking almost twice the size in runtime memory.";
            AutomationType = AutomationType.Automatic;
        }

        protected override void FixImporterSettings(TextureImporter importer)
        {
            importer.isReadable = false;
        }
    }
}