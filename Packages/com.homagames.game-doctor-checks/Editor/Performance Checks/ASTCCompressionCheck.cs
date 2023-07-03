using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class AstcCompressionCheck : AssetCheck<Texture2D, TextureImporter>
    {
        public AstcCompressionCheck() : base(
            "ASTC Compression",
            "Making sure all textures on Android and iOS are using ASTC compression.\n" +
            "ASTC compression is the best compromise for all scenarios and is the recommended default textures on mobile platforms.\n\n" +
            "<a href=\"https://docs.unity3d.com/Manual/class-TextureImporterOverride.html\">Unity Documentation</a>",
            new HashSet<string>() {"texture", "build", "size", "memory"}, ImportanceType.Advised, Priority.Low)
        {
        }

        private bool IsValidTextureSettings(TextureImporterPlatformSettings platformSettings)
        {
            return IsValidCompressionFormat(platformSettings.format);
        }

        private bool IsValidCompressionFormat(TextureImporterFormat textureImporterFormat)
        {
            return textureImporterFormat == TextureImporterFormat.ASTC_4x4
                   || textureImporterFormat == TextureImporterFormat.ASTC_5x5
                   || textureImporterFormat == TextureImporterFormat.ASTC_6x6
                   || textureImporterFormat == TextureImporterFormat.ASTC_8x8
                   || textureImporterFormat == TextureImporterFormat.ASTC_10x10
                   || textureImporterFormat == TextureImporterFormat.ASTC_12x12
                   || textureImporterFormat == TextureImporterFormat.ASTC_HDR_4x4
                   || textureImporterFormat == TextureImporterFormat.ASTC_HDR_5x5
                   || textureImporterFormat == TextureImporterFormat.ASTC_HDR_6x6
                   || textureImporterFormat == TextureImporterFormat.ASTC_HDR_8x8
                   || textureImporterFormat == TextureImporterFormat.ASTC_HDR_10x10
                   || textureImporterFormat == TextureImporterFormat.ASTC_HDR_12x12;
        }

        protected override bool AssetHasIssue(TextureImporter assetImporter, out IIssue issue)
        {
            var androidTextureImporter = assetImporter.GetPlatformTextureSettings("Android");
            var iosTextureImporter = assetImporter.GetPlatformTextureSettings("iPhone");
            if (assetImporter.textureType != TextureImporterType.SingleChannel &&
                (!IsValidTextureSettings(androidTextureImporter) || !IsValidTextureSettings(iosTextureImporter)))
            {
                issue = new ASTCCompressionIssue(assetImporter);
                return true;
            }

            issue = null;
            return false;
        }
    }
}
