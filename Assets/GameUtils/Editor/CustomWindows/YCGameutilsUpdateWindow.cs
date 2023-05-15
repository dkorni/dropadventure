using System.IO;
using UnityEditor;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCGameutilsUpdateWindow : YCCustomWindow {

            private static string GU_PATH = "Assets/GameUtils";

            private YCUpdatesHandler.GUUpdateData data;
            private bool deleteFolder;

            public void Init(YCUpdatesHandler.GUUpdateData updateData) {
                this.data = updateData;

                this.deleteFolder = true;
                this.SetSize(400, 125);
                this.SetPosition(WindowPosition.MiddleCenter);
            }

            private void OnGUI() {
                string ok = "";
                if (this.data.isUpToDate) {
                    this.AddLabel("GameUtils is already up to date.", Color.green, TextAnchor.MiddleCenter);
                    ok = "Reimport";
                } else {
                    this.AddLabel("GameUtils needs to be updated.", Color.red, TextAnchor.MiddleCenter);
                    ok = "Update";
                }
                this.AddEmptyLine();
                this.AddLabel("Do you want to import the version " + this.data.version + "?");
                this.deleteFolder = this.AddToggle("Delete the Gameutils folder before importing (recommended)", this.deleteFolder);
                this.AddEmptyLine();
                this.AddCancelOk("Cancel", ok, () => this.Close(), () => {
                    if (this.deleteFolder && Directory.Exists(GU_PATH)) {
                        Directory.Delete(GU_PATH, true);
                        File.Delete(GU_PATH + ".meta");
                        AssetDatabase.Refresh();
                    }
                    YCUpdatesHandler.ImportGameutilsPackage(this.data);
                    this.Close();
                });
            }
        }
    }
}
