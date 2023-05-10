using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace YsoCorp {
    namespace GameUtils {

        public class YCPackageManager {

            public static IEnumerator DownloadPackage(string url, string fileName, Action<bool, string> onDownload = null) {
                if (fileName.EndsWith(".unitypackage") == false) {
                    fileName += ".unitypackage";
                }
                var path = Path.Combine(Application.temporaryCachePath, fileName);
                var downloadHandler = new DownloadHandlerFile(path);

                UnityWebRequest webRequest = new UnityWebRequest(url) {
                    method = UnityWebRequest.kHttpVerbGET,
                    downloadHandler = downloadHandler
                };

                var operation = webRequest.SendWebRequest();
                Debug.Log("Downloading " + fileName);
                while (!operation.isDone) {
                    yield return new WaitForSeconds(0.1f);
                }

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
                {
                    Debug.LogError("The file " + fileName + " could not be downloaded.");
                    onDownload?.Invoke(false, path);
                } else {
                    onDownload?.Invoke(true, path);
                }
            }

            public static void DownloadAndImportPackage(string url, string fileName, bool interactive, Action<bool, string> onDownload = null) {
                onDownload = ((downloaded, path) =>  AssetDatabase.ImportPackage(path, interactive)) + onDownload ;
                YCEditorCoroutine.StartCoroutine(DownloadPackage(url, fileName, onDownload));
                    
            }
        }
    }
}