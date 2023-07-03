using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Installer.Utilities;
using HomaGames.HomaBelly.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HomaGames.HomaBelly.Installer
{
    public class HomaBellyEditorWindow : EditorWindow
    {
        public const string RESOURCES_PATH = "Assets/Homa Games/Homa Belly/Plugin Manifest/Editor/Style";

        // Templates
        private VisualTreeAsset errorLineTemplate;
        private VisualTreeAsset loadingBarTemplate;
        private VisualTreeAsset headerPopupTemplate;

        // Cache
        private Label installerTitle;
        private VisualElement center;
        private VisualElement mainWindow;
        private TextField tokenField;
        private VisualElement errorContainer;
        private VisualElement loadingBarInstance;
        private VisualElement loadingBarProgress;
        private Label loadingBarText;
        private VisualElement headerPopupInstance;
        private VisualElement footer;
        private Label androidId;
        private Label iosId;
        private Button logsButton;

        private const string LastUsedTokenPrefsKey = "homa_installer_last_used_token";
        private string LastUsedToken
        {
            get => PlayerPrefs.GetString(LastUsedTokenPrefsKey);
            set => PlayerPrefs.SetString(LastUsedTokenPrefsKey, value);
        }

        private LogComponent logComponent;
        private PackageListComponent packageListComponent;

        private CancellationTokenSource installCancelSource;

        // State
        private string defaultTitle = "Welcome to the Homa Belly SDK Installer!";

        [MenuItem("Window/Homa Games/Homa Belly/Setup", false, 1)]
        internal static void CreateSettingsAndFocus()
        {
            EditorAnalyticsProxy.TrackEditorAnalyticsEvent("open_homa_belly_setup_from_menu");
            var window = GetWindow(typeof(HomaBellyEditorWindow), false, "Homa Belly", true);
            window.minSize = new Vector2(400, 400);
        }

        [MenuItem("Window/Homa Games/Homa Belly/Delete All Packages", false, 12)]
        internal static void DeleteAllPackages()
        {
            // Determine if PluginManifest is available, which means some packages are installed
            if (PluginManifest.TryGetCurrentlyInstalled(out PluginManifest manifest))
            {
                bool ensureDeletion = EditorUtility.DisplayDialog("Homa Belly",
                    "You are about to delete all installed packages from Homa Belly.\n\nAre you sure?", "Yes", "No");
                if (ensureDeletion)
                {
                    EditorAnalyticsProxy.TrackEditorAnalyticsEvent("delete_all_packages");
                    PackageDownloader.ClearPackageCache(manifest);
                    PluginController.UninstallAllPackages();
                    // Delete manifest
                    File.Delete(PluginManifestDeserializer.LOCAL_PLUGIN_MANIFEST_FILE);
                    File.Delete(PluginManifestDeserializer.LOCAL_PLUGIN_MANIFEST_FILE + ".meta");
                    // Refresh Assets Database
                    AssetDatabase.Refresh();
                    // Open window
                    HomaBellyEditorLog.Debug("All packages deleted");
                    if (GetWindow(typeof(HomaBellyEditorWindow), false, "Homa Belly") is HomaBellyEditorWindow window)
                    {
#pragma warning disable CS4014
                        window.UpdatePackageListWithLatestManifest();
#pragma warning restore CS4014
                    }
                }
            }
            else
            {
                HomaBellyEditorLog.Warning("Homa Belly Manifest not found");
            }
        }

        [MenuItem("Window/Homa Games/Homa Belly/Full Implementation Guide", false, 23)]
        internal static void OpenHomaBellyDocumentation()
        {
            EditorAnalyticsProxy.TrackEditorAnalyticsEvent("open_homa_belly_implementation_guide");
            Application.OpenURL(
                "https://www.notion.so/homagames/Full-implementation-guide-13eb58192648433bb78707be3537e521");
        }

        [MenuItem("Window/Homa Games/Homa Belly/Troubleshooting", false, 23)]
        internal static void OpenHomaBellyTroubleshooting()
        {
            EditorAnalyticsProxy.TrackEditorAnalyticsEvent("open_homa_belly_troubleshooting");
            Application.OpenURL(
                "https://www.notion.so/homagames/Troubleshooting-Known-Issues-98747034856b41c79ae3caed102dd12f");
        }

        void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            installCancelSource = new CancellationTokenSource();
        }

        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnBeforeAssemblyReload()
        {
            HomaBellyEditorLog.BeforeAssemblyReload();
        }

        private void OnAfterAssemblyReload()
        {
            HomaBellyEditorLog.AfterAssemblyReload();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            var baseTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{RESOURCES_PATH}/HomaBellyInstaller.uxml");
            errorLineTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{HomaBellyEditorWindow.RESOURCES_PATH}/ErrorLine.uxml");
            loadingBarTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{RESOURCES_PATH}/LoadingBar.uxml");
            headerPopupTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{RESOURCES_PATH}/HeaderPopup.uxml");
            loadingBarInstance = loadingBarTemplate.Instantiate();
            headerPopupInstance = headerPopupTemplate.Instantiate();
            headerPopupInstance.Query<Button>("InstallerDownloadButton").First()
                .RegisterCallback<ClickEvent>(OnDownloadNewVersionClicked);
            loadingBarProgress = loadingBarInstance.Query<VisualElement>("LoadingBarProgress").First();
            loadingBarText = loadingBarInstance.Query<Label>("LoadingBarText").First();
            VisualElement labelFromUxml = baseTree.Instantiate();
            labelFromUxml.style.flexGrow = 1;
            root.Add(labelFromUxml);
            installerTitle = root.Query<Label>("Title").First();
            root.Query<Button>("Settings").First().RegisterCallback<ClickEvent>(OnSettingsClicked);
            logsButton = root.Query<Button>("Logs").First();
            logsButton.RegisterCallback<ClickEvent>(OnLogsClicked);
            var refreshButton = root.Query<Button>("RefreshButton").First();
#pragma warning disable CS4014
            refreshButton.RegisterCallback<ClickEvent>(OnRefreshButtonClicked);
#pragma warning restore CS4014
            tokenField = root.Query<TextField>("TokenInput").First();
            tokenField.RegisterCallback<ChangeEvent<string>>(e => LastUsedToken = e.newValue);
            center = root.Query<VisualElement>("Center").First();
            mainWindow = root.Query<VisualElement>("MainWindow").First();
            packageListComponent = new PackageListComponent(root.Query<VisualElement>("PackagesRoot").First());
            errorContainer = root.Query<VisualElement>("ErrorContainer").First();
            footer = root.Query<VisualElement>("Footer").First();
            androidId = root.Query<Label>("AndroidId").First();
            iosId = root.Query<Label>("IOSId").First();
            logComponent = new LogComponent(center, mainWindow);
            logComponent.OnToggled += (enabled) => { logsButton.ToggleInClassList("simple-toggle"); };
            SetLoadingProgressVisibility(false);

#if !(UNITY_ANDROID || UNITY_IOS)
            SetErrorMessage(
                "Homa Belly is only supported for iOS and Android targets. Please change the target before you continue.");
            tokenField.SetEnabled(false);
            refreshButton.SetEnabled(false);
#else
            SetErrorMessage("");
#endif
            if (PluginManifest.TryGetCurrentlyInstalled(out PluginManifest manifest))
            {
                packageListComponent.Update(manifest,null);
                UpdateBundleIds(manifest.Packages.AndroidBundleId, manifest.Packages.IOSBundleId, true);
                tokenField.value = manifest.AppToken;
            }
            else
            {
                UpdateBundleIds("", "", false);
                tokenField.value = LastUsedToken;
            }
        }

        private async void OnBecameVisible()
        {
            await UpdatePackageListWithLatestManifest();
        }

        private void Update()
        {
            // Tricks to populate logs in the UI in main thread
            // Cannot touch the UI in another thread but logs can happen in other threads
            logComponent?.UpdateLogsOnMainThread();
        }

        private void OnDestroy()
        {
            installCancelSource.Cancel();
        }

        void OnSettingsClicked(ClickEvent evt)
        {
            SettingsReflection.OpenSettings();
        }

        void OnLogsClicked(ClickEvent evt)
        {
            logComponent.IsOpen = !logComponent.IsOpen;
        }

        void OnDownloadNewVersionClicked(ClickEvent evt)
        {
            Application.OpenURL("https://lab.homagames.com/damysus/info");
        }

        void OnRefreshButtonClicked(ClickEvent evt)
        {
#pragma warning disable CS4014
            StartInstall();
#pragma warning restore CS4014
        }

        public async Task StartInstall()
        {
            try
            {
                SetErrorMessage("");
                var newestManifest = await PluginController.RequestPluginManifest(tokenField.value);
                PluginManifest.TryGetCurrentlyInstalled(out PluginManifest currentManifest);
                packageListComponent.Update(currentManifest, newestManifest);
                SetLoadingProgressVisibility(true);
                await PluginController.InstallPluginManifest(newestManifest, installCancelSource.Token,
                    SetLoadingProgress);
                PluginManifest.TryGetCurrentlyInstalled(out currentManifest);
                packageListComponent.Update(currentManifest, null);
                SetLoadingProgressVisibility(false);
                UpdateBundleIds(newestManifest.Packages.AndroidBundleId, newestManifest.Packages.IOSBundleId, true);
            }
            catch (InstallerHttpCallerException e)
            {
                switch (e.Status)
                {
                    case InstallerHttpCallerException.ErrorStatus.UNKNOWN:
                        SetErrorMessage($"Unknown error {e.StatusString}");
                        break;
                    case InstallerHttpCallerException.ErrorStatus.TOKEN_ID_MISSING:
                    case InstallerHttpCallerException.ErrorStatus.MANIFEST_NO_MATCH:
                        SetErrorMessage("Wrong Token ID, please make sure you enter a valid token.");
                        break;
                    case InstallerHttpCallerException.ErrorStatus.MANIFEST_SDK_VERSION_NOT_ALLOWED:
                        SetErrorMessage("Installer version is too old. Please update the installer.");
                        SetHeaderPopupVisibility(true);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // the window was closed
            }
            catch (Exception other)
            {
                Debug.LogError($"Unknown exception when trying to install : {other}");
            }
            finally
            {
                // Make sure we don't lock anything in case of failure
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        async Task UpdatePackageListWithLatestManifest()
        {
            if (PluginManifest.TryGetCurrentlyInstalled(out PluginManifest manifest))
            {
                try
                {
                    PluginManifest newest = await PluginController.RequestPluginManifest(manifest.AppToken);
                    packageListComponent.Update(manifest, newest);
                }
                catch (InstallerHttpCallerException e)
                {
                    switch (e.Status)
                    {
                        case InstallerHttpCallerException.ErrorStatus.UNKNOWN:
                            SetErrorMessage($"Unknown error {e.StatusString}");
                            break;
                        case InstallerHttpCallerException.ErrorStatus.TOKEN_ID_MISSING:
                            SetErrorMessage("Wrong Token ID, please make sure you enter a valid token.");
                            break;
                        case InstallerHttpCallerException.ErrorStatus.MANIFEST_SDK_VERSION_NOT_ALLOWED:
                            SetErrorMessage("Installer version is too old. Please update the installer.");
                            SetHeaderPopupVisibility(true);
                            break;
                    }
                }
                catch (Exception other)
                {
                    Debug.LogError($"Unknown exception when fetching latest configuration : {other}");
                }
            }
        }

        void SetLoadingProgress(float progress, string message)
        {
            if (progress == 0)
                installerTitle.text = defaultTitle;
            if (progress > 0)
                installerTitle.text = "Installing...";
            if (progress >= 0.999f)
                installerTitle.text = "Installation Finished.";
            loadingBarProgress.style.width = Length.Percent(progress * 100);
            loadingBarText.text = message;
            loadingBarInstance.MarkDirtyRepaint();
        }

        void SetLoadingProgressVisibility(bool visible)
        {
            if (visible)
            {
                mainWindow.Insert(1, loadingBarInstance);
            }
            else
            {
                loadingBarInstance.RemoveFromHierarchy();
            }
        }

        void SetErrorMessage(string content)
        {
            for (int i = errorContainer.childCount - 1; i >= 0; i--)
            {
                errorContainer[i].RemoveFromHierarchy();
            }

            if (string.IsNullOrEmpty(content))
                return;
            var line = errorLineTemplate.Instantiate();
            var text = line.Query<Label>("ErrorLabel").First();
            text.text = content;
            errorContainer.Add(line);
        }

        void UpdateBundleIds(string android, string ios, bool enabled)
        {
            if (enabled)
            {
                if (footer.parent == null)
                    rootVisualElement.Add(footer);
                if (androidId != null)
                    androidId.text = android;
                if (iosId != null)
                    iosId.text = ios;
            }
            else
            {
                footer.RemoveFromHierarchy();
            }
        }

        void SetHeaderPopupVisibility(bool visible)
        {
            if (visible)
            {
                rootVisualElement.Insert(0, headerPopupInstance);
            }
            else
            {
                headerPopupInstance.RemoveFromHierarchy();
            }
        }
    }
}