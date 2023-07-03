using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HomaGames.HomaBelly.Installer
{
    public class PackageListComponent
    {
        //Templates
        private VisualTreeAsset packageRowTemplate;
        
        // Cache
        private VisualElement packageListContainer;
        private Texture2D validIcon;
        private Texture2D errorIcon;
        
        public PackageListComponent(VisualElement root)
        {
            packageRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{HomaBellyEditorWindow.RESOURCES_PATH}/PackagesRow.uxml");
            validIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{HomaBellyEditorWindow.RESOURCES_PATH}/check_circle_outlined.png");
            errorIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{HomaBellyEditorWindow.RESOURCES_PATH}/error_log.png");
            packageListContainer = root;
        }
        
        public void Update(PluginManifest current, PluginManifest latest)
        {
            PluginManifestDiff diff = new PluginManifestDiff(current, latest);
            for (int i = packageListContainer.childCount - 1; i >= 0; i--)
            {
                packageListContainer[i].RemoveFromHierarchy();
            }

            var packages = new List<PackageComponent>();
            if (current != null)
                packages = current.Packages.GetAllPackages();
            else if (latest != null)
                packages = latest.Packages.GetAllPackages();

            foreach (var packageComponent in packages)
            {
                var row = packageRowTemplate.Instantiate();
                var packageName = row.Query<Label>("PackageName").First();
                var packageVersion = row.Query<Label>("PackageVersion").First();
                var suffix = row.Query<Label>("NewVersionAvailable").First();
                var changelog = row.Query<VisualElement>("Changelog").First();
                var docs = row.Query<VisualElement>("Documentation").First();
                var icon = row.Query<VisualElement>("StateIcon").First();
                packageName.text = packageComponent.Id;
                packageVersion.text = packageComponent.Version;
                var tooltip = "";
                if (diff.NewPackages.ContainsKey(packageComponent.Id))
                    tooltip = "New!";
                else if (!PackageCommon.IsPackageInstalled(packageComponent))
                {
                    var redColor = new Color(182 / 255f, 60 / 255f, 20 / 255f);
                    suffix.style.color = redColor;
                    packageVersion.style.color = redColor;
                    tooltip = "Files missing!";
                    icon.style.backgroundImage = errorIcon;
                }
                else
                {
                    if (diff.IsPackageInDevelopment(packageComponent.Id))
                    {
                        tooltip = "Custom";
                    }
                    else if (diff.TryGetNewLatestVersion(packageComponent.Id, out string v))
                        tooltip = $"{v} available!";
                }

                suffix.text = tooltip;
                if (!string.IsNullOrEmpty(packageComponent.ChangelogUrl))
                {
                    changelog.RegisterCallback<ClickEvent>(_ => Application.OpenURL(packageComponent.ChangelogUrl));
                }
                else
                {
                    changelog.RemoveFromHierarchy();
                }

                if (!string.IsNullOrEmpty(packageComponent.DocumentationUrl))
                {
                    docs.RegisterCallback<ClickEvent>(_ => Application.OpenURL(packageComponent.DocumentationUrl));
                }
                else
                {
                    docs.RemoveFromHierarchy();
                }

                packageListContainer.Add(row);
            }
        }
    }
}