using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class BlendShapeCheck : AssetCheck<Mesh, ModelImporter>
    {
        public BlendShapeCheck() : base("Blend Shape",
            "Unity performance cheat sheets recommends disabling this option if it isn't used.\n" +
            "Disable rigs and BlendShapes: If your mesh does not need skeletal or blendshape animation, disable these options wherever possible.",
            new HashSet<string>() {"mesh", "memory"}, ImportanceType.Advised, Priority.Medium)
        {
        }

        protected override bool AssetHasIssue(ModelImporter assetImporter, out IIssue issue)
        {
            if (assetImporter.importBlendShapes)
            {
                issue = new BlendShapeIssue(assetImporter);
                return true;
            }

            issue = null;
            return false;
        }
    }
}