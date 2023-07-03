using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class TextureReadableCheck : AssetCheck<Texture2D,TextureImporter>
    {
        public TextureReadableCheck() : base(
            "Texture Read/Write",
            "Textures shouldn't be readable, except in some rare scenarios.",
            new HashSet<string>() {"texture", "build", "size", "memory", "mobile"}, ImportanceType.Advised,
            Priority.Low)
        {
        }

        protected override bool AssetHasIssue(TextureImporter assetImporter, out IIssue issue)
        {
            if (assetImporter.isReadable)
            {
                issue = new TextureReadableIssue(assetImporter);
                return true;
            }
            issue = null;
            return false;
        }
    }
}