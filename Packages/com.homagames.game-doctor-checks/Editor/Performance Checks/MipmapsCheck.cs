using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MipmapsCheck : AssetCheck<Texture2D,TextureImporter>
    {
        public MipmapsCheck() : base(
        "Mipmaps usage",
        "Textures shouldn't use mipmaps on mobile devices, except in some rare scenarios.",
        new HashSet<string>() {"texture", "build", "size", "memory", "mobile"}, ImportanceType.Advised,
        Priority.Low)
        {
        }

        protected override bool AssetHasIssue(TextureImporter assetImporter, out IIssue issue)
        {
            if (assetImporter.mipmapEnabled)
            {
                issue = new MipmapsIssue(assetImporter);
                return true;
            }

            issue = null;
            return false;
        }
    }
}