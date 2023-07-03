using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public abstract class AssetCheck<U,I> : BaseCheck where U : Object where I : AssetImporter
    {
        public AssetCheck(string name, string description, HashSet<string> tags, ImportanceType importance,
            Priority priority) : base(
            name, description, tags, importance, priority)
        {
        }

        protected override Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();
            foreach (var textureImporter in AssetUtility.GetAllImporters<U,I>())
            {
                if (AssetHasIssue(textureImporter, out IIssue issue))
                    result.Issues.Add(issue);
            }

            return Task.FromResult(result);
        }

        protected abstract bool AssetHasIssue(I assetImporter,out IIssue issue);
    }
}