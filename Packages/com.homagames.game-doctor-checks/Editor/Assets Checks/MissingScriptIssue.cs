using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MissingScriptIssue : BaseIssue
    {
        private readonly string _assetPath;

        public MissingScriptIssue(string assetPath, int count, string shortName)
        {
            _assetPath = assetPath;
            Name = "Missing script for asset " + shortName;
            Description = $"There are {count} missing scripts for asset at path {assetPath}";
            AutomationType = AutomationType.Automatic;
        }

        public override void Draw()
        {
            base.Draw();
            if (GUILayout.Button("Locate"))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(_assetPath));
            }
        }

        protected override Task<bool> InternalFix()
        {
            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(_assetPath);
            if (!gameObject)
                return Task.FromResult(false);

            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            PrefabUtility.SavePrefabAsset(gameObject);

            return Task.FromResult(true);
        }
    }
}