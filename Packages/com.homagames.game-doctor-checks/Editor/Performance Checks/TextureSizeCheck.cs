using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class TextureSizeCheck : AssetCheck<Texture2D,TextureImporter>
    {
        public const int MaxAllowedSize = 512;

        public TextureSizeCheck() : base(
            "Texture Size",
            "Textures shouldn't exceed 512 size on mobile devices, except in some rare scenarios.",
            new HashSet<string>() {"texture", "build", "size", "memory", "mobile"}, ImportanceType.Advised,
            Priority.Low)
        {
        }

        private bool IsValidTextureSettings(TextureImporterPlatformSettings platformSettings)
        {
            return platformSettings.maxTextureSize <= MaxAllowedSize;
        }

        protected override bool AssetHasIssue(TextureImporter assetImporter, out IIssue issue)
        {
            var androidTextureImporter = assetImporter.GetPlatformTextureSettings("Android");
            var iosTextureImporter = assetImporter.GetPlatformTextureSettings("iPhone");
            if (!IsValidTextureSettings(androidTextureImporter) || !IsValidTextureSettings(iosTextureImporter))
            {
                issue = new TextureSizeIssue(assetImporter);
                return true;
            }
            issue = null;
            return false;
        }
    }
}