using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using HomaGames.GameDoctor.Utilities;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public abstract class AssetIssue<T,I> : BaseIssue where T : Object where I : AssetImporter
    {
        private readonly I _assetImporter;

        protected AssetIssue(I assetImporter)
        {
            _assetImporter = assetImporter;
        }

        protected override Task<bool> InternalFix()
        {
            FixImporterSettings(_assetImporter);

            GameDoctorFlow.RegisterForReimport(_assetImporter);

            return Task.FromResult(true);
        }

        public override void Draw()
        {
            base.Draw();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Locate",GUILayout.Width(75)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(_assetImporter.assetPath);
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
            EditorGUILayout.EndHorizontal();
        }

        protected abstract void FixImporterSettings(I importer);
    }
}