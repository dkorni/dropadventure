using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MissingScriptsCheck : BaseCheck
    {
        private string[] _allPrefabsGuids;

        public MissingScriptsCheck() : base(
            "Missing Scripts",
            "Checking if there's any missing scripts in your project.", new HashSet<string>() {"asset", "scripts"},
            ImportanceType.Advised, Priority.Medium)
        {
        }

        protected override Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();
            _allPrefabsGuids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var prefab in _allPrefabsGuids)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(prefab);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                int missingScripts =
                    GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
                if (missingScripts > 0)
                {
                    result.Issues.Add(new MissingScriptIssue(prefabPath, missingScripts,
                        gameObject.name));
                }
            }

            return Task.FromResult(result);
        }
    }
}