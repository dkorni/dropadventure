using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class MissingSceneIssue : StepBasedIssue
    {
        public MissingSceneIssue()
        {
            AutomationType = AutomationType.Interactive;
            Name = "No main scene";
            Description =
                "You don't have any main scene assigned in your build settings or all your scenes are disabled";
            stepsList = new List<Step>()
            {
                new Step(() => !HasIssue, "Assign a default scene", (issue, step) =>
                {
                    var currentBuildSceneAsset =
                        EditorBuildSettings.scenes.Length > 0 ? EditorBuildSettings.scenes[0] : null;
                    var currentScene = currentBuildSceneAsset != null
                        ? AssetDatabase.LoadAssetAtPath<SceneAsset>(currentBuildSceneAsset.path)
                        : null;
                    var scene = (SceneAsset) EditorGUILayout.ObjectField("Default scene", currentScene,
                        typeof(SceneAsset), false);
                    if (!scene) return;
                    var newEditorBuildSceneAsset =
                        new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(scene), true);
                    if (EditorBuildSettings.scenes.Length > 0)
                    {
                        var allScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                        allScenes[0] = newEditorBuildSceneAsset;
                        EditorBuildSettings.scenes = allScenes.ToArray();
                    }
                    else
                    {
                        EditorBuildSettings.scenes = new[] {newEditorBuildSceneAsset};
                    }
                })
            };
        }

        public bool HasIssue
        {
            get
            {
                return EditorBuildSettings.scenes.Length == 0 ||
                       EditorBuildSettings.scenes.All(scene => scene == null || !scene.enabled);
            }
        }
    }
}