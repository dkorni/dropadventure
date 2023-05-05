using UnityEngine;
using UnityEditor;
using com.zibra.liquid.Solver;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System;

namespace com.zibra.liquid.Editor
{
    internal class ZibraLiquidUpdateChecker : EditorWindow
    {
        public static GUIContent WindowTitle => new GUIContent("Zibra Liquids Check Version");

        const string URL = "https://generation.zibra.ai/api/pluginVersion?effect=liquids&engine=unity&";

        private const string UPDATE_CHECK_PREFS_KEY = "ZibraLiquidsAutomaticallyCheckForUpdates";
        private const string UPDATE_CHECKED_SESSION_STATE_KEY = "ZibraLiquidsUpdateChecked";

        private static ZibraLiquidUpdateChecker InstanceInternal;
        private static ZibraLiquidUpdateChecker Instance
        {
            get
            {
                if (InstanceInternal == null)
                {
                    InstanceInternal = CreateInstance<ZibraLiquidUpdateChecker>();
                }
                return InstanceInternal;
            }
        }

        private static UnityWebRequestAsyncOperation Request;
        private static string PluginSKU = "";
        private static Label LabelMessage;
        private static Label LabelPluginSKU;
        private static Button BtnPluginSKU;
        private static Toggle Checkbox;

        private bool OnlyShowOutdated = false;
        private bool IsLatestVersion = false;


        [InitializeOnLoadMethod]
        internal static void InitializeOnLoad()
        {
            // Don't automatically open any windows in batch mode
            if (Application.isBatchMode)
            {
                return;
            }

            // Check whether user disabled auto update checking
            if (!EditorPrefs.GetBool(UPDATE_CHECK_PREFS_KEY, true))
            {
                return;
            }

            // Check whether auto update checking ran this editor session
            if (!SessionState.GetBool(UPDATE_CHECKED_SESSION_STATE_KEY, false))
            {
                SessionState.SetBool(UPDATE_CHECKED_SESSION_STATE_KEY, true);
                EditorApplication.update += ShowWindowDelayed;
            }
        }

        internal static void ShowWindowDelayed()
        {
            ShowWindow(true);
            EditorApplication.update -= ShowWindowDelayed;
        }

        [MenuItem("Zibra AI/Zibra Liquids/Check for Update", false, 1)]
        public static void ShowWindowMenu()
        {
            ShowWindow(false);
        }

        public static void ShowWindow(bool onlyShowOutdated)
        {
            Instance.OnlyShowOutdated = onlyShowOutdated;

            if (!onlyShowOutdated)
            {
                Instance.Show();
            }
        }

        private void OnEnable()
        {
            titleContent = WindowTitle;

            var root = rootVisualElement;

            int width = 480;
            int height = 360;

            minSize = maxSize = new Vector2(width, height);

            var uxmlAssetPath = AssetDatabase.GUIDToAssetPath("f1c391edc80cd254a81b3eea9e36b979");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlAssetPath);
            visualTree.CloneTree(root);

            var commonUSSAssetPath = AssetDatabase.GUIDToAssetPath("20c4b12a1544dac44b6c04777afe69db");
            var commonStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(commonUSSAssetPath);
            root.styleSheets.Add(commonStyleSheet);

#if ZIBRA_LIQUID_PRO_VERSION
            var versionSpecificUSSAssetPath = AssetDatabase.GUIDToAssetPath("6cc12d310d0c4244f91750a8f28911fb");
#elif ZIBRA_LIQUID_PAID_VERSION
            var versionSpecificUSSAssetPath = AssetDatabase.GUIDToAssetPath("f7fdff9b2ff25c242be662608c500479");
#else
            var versionSpecificUSSAssetPath = AssetDatabase.GUIDToAssetPath("2b6de5ec0ff70dc45a33e036cf0d599f");
#endif

            var versionSpecificStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(versionSpecificUSSAssetPath);
            root.styleSheets.Add(versionSpecificStyleSheet);

            BtnPluginSKU = root.Q<Button>("PluginSKU");
            BtnPluginSKU.visible = false;
            LabelMessage = root.Q<Label>("Version");
            LabelPluginSKU = root.Q<Label>("PluginSKU");
            Checkbox = root.Q<Toggle>("Check");
            Checkbox.value = EditorPrefs.GetBool(UPDATE_CHECK_PREFS_KEY, true);
            Checkbox.RegisterValueChangedCallback(evt => EditorPrefs.SetBool(UPDATE_CHECK_PREFS_KEY, evt.newValue));

#if ZIBRA_LIQUID_PRO_VERSION
            PluginSKU = "sku=pro";
#elif ZIBRA_LIQUID_PAID_VERSION
            PluginSKU = "sku=paid";
#else
            PluginSKU = "sku=free";
#endif
            RequestLatestVersion();
        }

        private void UpdatePluginPageClick()
        {
            EditorApplication.ExecuteMenuItem("Window/Package Manager");
        }

        private void RequestLatestVersion()
        {
            Request = UnityWebRequest.Get(URL + PluginSKU).SendWebRequest();
            LabelMessage.text = "Please wait.";
            Request.completed += UpdateCheckVersionRequest;
        }
        
        private void UpdateCheckVersionRequest(AsyncOperation obj)
        {
            if (Request == null || !Request.isDone)
            {
                return;
            }
#if UNITY_2020_2_OR_NEWER
            if (Request.webRequest.result != UnityWebRequest.Result.Success)
#else
            if (request.webRequest.isHttpError || request.webRequest.isNetworkError)
#endif
            {
                string errorMessage = $"Update check failed: {Request.webRequest.error}";
                Debug.LogError(errorMessage);
                LabelMessage.text = errorMessage;
                if (OnlyShowOutdated)
                {
                    DestroyImmediate(Instance);
                }
                return;
            }
            if (Request.webRequest.responseCode != 200)
            {
                string errorMessage = $"Update check failed: {Request.webRequest.responseCode} - {Request.webRequest.downloadHandler.text}";
                Debug.LogError(errorMessage);
                LabelMessage.text = errorMessage;
                if (OnlyShowOutdated)
                {
                    DestroyImmediate(Instance);
                }
                return;
            }

            CheckVersion();
        }

        public void CheckVersion()
        {
            var pluginVersion = JsonUtility.FromJson<ZibraLiquidVersion>(Request.webRequest.downloadHandler.text);
            IsLatestVersion = CheckIsLatestVersion(pluginVersion.version, ZibraLiquid.PluginVersion);
#pragma warning disable 0162
            if (ZibraLiquid.IsPreReleaseVersion)
            {
                LabelMessage.text = $"You have a Pre-release version of the Zibra Liquids: {ZibraLiquid.PluginVersion}";
                if (OnlyShowOutdated)
                {
                    DestroyImmediate(Instance);
                }
            }
            else if (IsLatestVersion)
            {
                LabelMessage.text = $"You have the latest version of the Zibra Liquids: {ZibraLiquid.PluginVersion}";
                if (OnlyShowOutdated)
                {
                    DestroyImmediate(Instance);
                }
            }
            else
            {
                LabelMessage.text = $"New Zibra Liquids version available. Please consider updating. Latest version: \"{pluginVersion.version}\". Current version: \"{ZibraLiquid.PluginVersion}\"";
#if ZIBRA_LIQUID_PRO_VERSION
                LabelPluginSKU.text = "Update Zibra Liquids Pro";
#elif ZIBRA_LIQUID_PAID_VERSION
                LabelPluginSKU.text = "Update Zibra Liquids";
#else
                LabelPluginSKU.text = "Update Zibra Liquids Free";
#endif
                BtnPluginSKU.visible = true;
                BtnPluginSKU.clicked += UpdatePluginPageClick;
                if (OnlyShowOutdated)
                {
                    Show();
                }
            }
#pragma warning restore 0162

            LabelMessage.style.marginLeft = 50;
            LabelMessage.style.marginRight = 50;
            LabelMessage.style.whiteSpace = WhiteSpace.Normal;
            LabelMessage.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private bool CheckIsLatestVersion(string pluginVersion, string localPluginVersion)
        {
            char[] separator = { '.' };
            int[] currentPluginVersion = { };
            int[] serverPluginVersion = { };

            try
            {
                string[] currentPluginVersionStrArr = localPluginVersion.Split(separator);
                currentPluginVersion = new int[currentPluginVersionStrArr.Length];
                for (int i = 0; i < currentPluginVersionStrArr.Length; i++)
                {
                    currentPluginVersion[i] = int.Parse(currentPluginVersionStrArr[i]);
                }

                string[] serverPluginVersionStrArr = pluginVersion.Split(separator);
                serverPluginVersion = new int[serverPluginVersionStrArr.Length];
                for (int i = 0; i < serverPluginVersionStrArr.Length; i++)
                {
                    serverPluginVersion[i] = int.Parse(serverPluginVersionStrArr[i]);
                }
            }
            catch (Exception)
            {
                return true;
            }

            if (currentPluginVersion.Length < serverPluginVersion.Length)
            {
                int[] tempArr = currentPluginVersion;
                currentPluginVersion = new int[serverPluginVersion.Length];
                Array.Copy(tempArr, currentPluginVersion, tempArr.Length);
            }
            else if (currentPluginVersion.Length > serverPluginVersion.Length)
            {
                int[] tempArr = serverPluginVersion;
                serverPluginVersion = new int[currentPluginVersion.Length];
                Array.Copy(tempArr, serverPluginVersion, tempArr.Length);
            }

            for (int i = 0; i < Math.Min(currentPluginVersion.Length, serverPluginVersion.Length); i++)
            {
                if (currentPluginVersion[i] < serverPluginVersion[i])
                {
                    return false;
                }
                else if (currentPluginVersion[i] > serverPluginVersion[i])
                {
                    return true;
                }
            }
            return true;
        }

        private struct ZibraLiquidVersion
        {
            public string version;
        }
    }
}
