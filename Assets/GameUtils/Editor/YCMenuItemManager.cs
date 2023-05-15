using UnityEngine;
using UnityEditor;

namespace YsoCorp {
    namespace GameUtils {

        public class YCMenuItemManager {

            static string WEBSITE_URL = "https://gameutils.ysocorp.com/";
            static string FIREBASE_URL = "https://gameutils-unity.ysocorp.com/public/unity/FirebaseAnalytics_9_3_0.unitypackage";
            static string AMAZON_URL = "https://gameutils-unity.ysocorp.com/public/unity/Amazon_APS.unitypackage";

            static YCConfig YCCONFIGDATA;


            [InitializeOnLoadMethod]
            private static void Init() {
                GetYCConfigData();
            }

            [MenuItem("GameUtils/Open game's page", false, 1001)]
            static void MenuOpenGamePage() {
                GetYCConfigData();
                if (YCCONFIGDATA.gameYcId != "") {
                    Application.OpenURL(WEBSITE_URL + YCCONFIGDATA.gameYcId + "/settings");
                } else {
                    EditorUtility.DisplayDialog("Game's web page", "Please enter your Game Yc ID in the YCConfigData.", "Ok");
                }
            }

            [MenuItem("GameUtils/Import Package/Firebase", false, 1002)]
            static void ImportFirebase() {
                YCPackageManager.DownloadAndImportPackage(FIREBASE_URL, "FirebaseAnalytics", true);
            }

            [MenuItem("GameUtils/Import Package/Amazon", false, 1003)]
            static void ImportAmazon() {
                YCPackageManager.DownloadAndImportPackage(AMAZON_URL, "Amazon_APS", true);
            }

            [MenuItem("GameUtils/Update GameUtils", false, 2001)]
            static void MenuUpdateGameutils() {
                YCUpdatesHandler.UpdateGameutils();
            }

            [MenuItem("GameUtils/Upgrade Applovin MAX", false, 2002)]
            static void MenuUpgradeMax() {
                YCUpdatesHandler.UpgradeMax();
            }

            [MenuItem("GameUtils/Import Config", false, 2003)]
            static void MenuImportConfig() {
                GetYCConfigData();
                EditorUtility.SetDirty(YCCONFIGDATA);
                YCCONFIGDATA.EditorImportConfig();
            }

            [MenuItem("GameUtils/Tools/Display Debug Window", false, 3001)]
            static void MenuToolDebugWindow() {
                YCDebugWindow window = EditorWindow.GetWindow<YCDebugWindow>(false, "GameUtils Debug Window");
                YCDebugWindow.Init();
                window.SetMinSize(570, 290);
                window.Show();
            }

            [MenuItem("GameUtils/Tools/Replace GameObjects", false, 3002)]
            static void MenuToolReplaceBy() {
                YCReplaceObjectsWindow window = EditorWindow.GetWindow<YCReplaceObjectsWindow>(false, "Replace GameObjects");
                window.Init();
                window.Show();
            }

            private static void GetYCConfigData() {
                if (YCCONFIGDATA == null) {
                    YCCONFIGDATA = Resources.Load<YCConfig>("YCConfigData");
                }
            }
        }
    }
}