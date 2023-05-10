using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCPackageDetection : AssetPostprocessor {



            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
                if (importedAssets.Length > 0) {
                    CheckForIOS14Support(importedAssets);
                }
            }

            private static void CheckForIOS14Support(string[] importedAssets) {
#if !YC_IOS14_SUPPORT
                foreach (string importedAsset in importedAssets) {
                    if (importedAsset.Contains("GameUtils")) {
                        YCEditorCoroutine.StartCoroutine(InstallPackage("com.unity.ads.ios-support", "1.2.0", () => {
                            AddDefineSymbolsForGroups("YC_IOS14_SUPPORT");
                        }));
                        break;
                    }
                }
#endif
            }

            private static IEnumerator InstallPackage(string packageName, string version = "", Action onFinished = null) {
                var pack = Client.List();
                while (!pack.IsCompleted) yield return null;

                bool isInstalled = pack.Result.FirstOrDefault(q => q.name == packageName) != null;
                UnityEditor.PackageManager.Requests.AddRequest packAdd = null;
                if (isInstalled == false) {
                    if (string.IsNullOrEmpty(version) == false) {
                        packageName += "@" + version;
                    }
                    packAdd = Client.Add(packageName);
                }

                while (packAdd != null && !packAdd.IsCompleted) yield return null;
                onFinished?.Invoke();
            }

            private static void AddDefineSymbolsForGroups(string def) {
                AddDefineSymbol(def, BuildTargetGroup.Standalone);
                AddDefineSymbol(def, BuildTargetGroup.iOS);
                AddDefineSymbol(def, BuildTargetGroup.Android);
                AssetDatabase.SaveAssets();
            }

            private static void AddDefineSymbol(string def, BuildTargetGroup group) {
                string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                if (current.Contains(def) == false) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, current + ";" + def);
                }
            }
        }
    }
}