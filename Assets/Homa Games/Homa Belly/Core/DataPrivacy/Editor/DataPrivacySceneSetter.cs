using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HomaGames.HomaBelly.DataPrivacy
{
    public class DataPrivacySceneSetter : IActiveBuildTargetChanged
    {
        private const string INIT_SCENE_PATH = "Assets/Homa Games/Homa Belly/Core/DataPrivacy/Runtime/Scenes/Homa Games DataPrivacy Init Scene.unity";
        private const string GDPR_SCENE_PATH = "Assets/Homa Games/Homa Belly/Core/DataPrivacy/Runtime/Scenes/Homa Games DataPrivacy GDPR Scene.unity";
        private const string IDFA_SCENE_PATH = "Assets/Homa Games/Homa Belly/Core/DataPrivacy/Runtime/Scenes/Homa Games DataPrivacy IDFA Scene.unity";

        private static readonly string[] ScenePaths = {INIT_SCENE_PATH, GDPR_SCENE_PATH, IDFA_SCENE_PATH};
        
        public int callbackOrder => 0;
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Configure();
        }


        [InitializeOnLoadMethod]
        private static void OnProjectLoaded()
        {
            BuildPlayerHandlerWrapper.AddBuildFilter(OnBuild);
            
            if (Application.isBatchMode) return;

            Configure();
        }

        private static bool OnBuild(BuildReport options)
        {
            Configure();
            return true;
        }

        private static void Configure()
        {
            DataPrivacy.Settings settings = DataPrivacy.Settings.EditorCreateOrLoadDataPrivacySettings();

            UpdateSettings(settings);

            EditorBuildSettingsScene[] initialBuildSettingsScenes = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] buildSettingsScenes = AddAllDataPrivacyScenes(initialBuildSettingsScenes);

            try
            {
                buildSettingsScenes.First(scene => scene.path == INIT_SCENE_PATH).enabled = true;
                buildSettingsScenes.First(scene => scene.path == GDPR_SCENE_PATH).enabled =
                    IsGdprSceneEnabled(settings);
                buildSettingsScenes.First(scene => scene.path == IDFA_SCENE_PATH).enabled =
                    IsIdfaSceneEnabled(settings);
            }
            catch (InvalidOperationException)
            {
                Debug.LogError("Error when setting up DataPrivacy scenes.");
            }

            if (!initialBuildSettingsScenes.SequenceEqual(buildSettingsScenes, new EditorBuildSettingsSceneComparer()))
                EditorBuildSettings.scenes = buildSettingsScenes;
        }

        private static void UpdateSettings(DataPrivacy.Settings settings)
        {
            string[] dataKeys =
            {
                "b_enable_gdpr_android",
                "b_enable_gdpr_ios",
                "b_enable_idfa",
                "b_enable_idfa_prepopup"
            };

            Action<bool>[] dataSetters =
            {
                b => settings.GdprEnabledForAndroid = b,
                b => settings.GdprEnabledForIos = b,
                b => settings.ShowIdfa = b,
                b => settings.ShowIdfaPrePopup = b,
            };

            for (int i = 0; i < dataKeys.Length; i++)
            {
                if (HomaBellyManifestConfiguration.TryGetBool(out var enabled, HomaBellyEditorConstants.PACKAGE_ID,
                        dataKeys[i]))
                {
                    dataSetters[i].Invoke(enabled);
                }
            }
        }

        private static bool IsGdprSceneEnabled(DataPrivacy.Settings settings)
        {
#if !UNITY_ANDROID && !UNITY_IOS
            return false;
#else
            return !settings.ForceDisableGdpr && settings.GdprEnabled;
#endif
        }

        private static bool IsIdfaSceneEnabled(DataPrivacy.Settings settings)
        {
#if !UNITY_IOS
            return false;
#else
            return settings.ShowIdfa;
#endif
        }

        private static EditorBuildSettingsScene[] AddAllDataPrivacyScenes(EditorBuildSettingsScene[] initialSceneList)
        {
            IEnumerable<EditorBuildSettingsScene> nonDataPrivacyScenes =
                initialSceneList.Where(scene => !ScenePaths.Contains(scene.path));

            IEnumerable<EditorBuildSettingsScene> DataPrivacyScenes =
                ScenePaths.Select(path => new EditorBuildSettingsScene(path, true));

            return DataPrivacyScenes.Concat(nonDataPrivacyScenes).ToArray();
        }

        private static bool StartsWith(EditorBuildSettingsScene[] sceneList, string[] scenePaths)
        {
            if (sceneList.Length < scenePaths.Length)
                return false;
            
            for (int i = 0; i < scenePaths.Length; i++)
            {
                if (sceneList[i].path != scenePaths[i])
                    return false;
            }

            return true;
        }

        private class EditorBuildSettingsSceneComparer : EqualityComparer<EditorBuildSettingsScene>
        {
            public override bool Equals(EditorBuildSettingsScene settingsScene1, EditorBuildSettingsScene settingsScene2)
            {
                if (settingsScene1 == null)
                    return settingsScene2 == null;

                if (settingsScene2 == null)
                    return false;
                
                return settingsScene1.path == settingsScene2.path
                       && settingsScene1.enabled == settingsScene2.enabled;
            }

            public override int GetHashCode(EditorBuildSettingsScene settingsScene)
            {
                return settingsScene.path.GetHashCode() ^ settingsScene.enabled.GetHashCode();
            }
        }
    }
}
