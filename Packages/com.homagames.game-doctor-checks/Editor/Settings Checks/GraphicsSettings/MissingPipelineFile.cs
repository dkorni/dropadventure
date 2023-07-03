using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

namespace HomaGames.GameDoctor.Checks
{
    public class MissingPipelineFile : StepBasedIssue
    {
        public MissingPipelineFile()
        {
            Name = "Missing Pipeline File";
            Description =
                "You have the URP pipeline imported but you are missing a URP pipeline asset in your graphics settings";
            AutomationType = AutomationType.Interactive;
            Priority = Priority.High;
            stepsList = new List<Step>()
            {
                new OpenPlayerSettingsStep("Graphics"),
                new Step(() => GraphicsSettings.renderPipelineAsset != null, "Create and assign pipeline settings.",
                    (issue, step) =>
                    {
                        EditorGUILayout.LabelField(
                            "Have a look at Unity documentation on how to create a pipeline settings and how to assign :",
                            EditorStyles.wordWrappedLabel);
                        if (GUILayout.Button("here"))
                        {
                            Application.OpenURL(
                                "https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/configuring-universalrp-for-use.html");
                        }
                    })
            };
        }

        public async Task<bool> HasIssue()
        {
            var packages = Client.List();
            while (!packages.IsCompleted)
                await Task.Delay(200);
            bool srp = false;
            foreach (var package in packages.Result)
            {
                if (package.name == "com.unity.render-pipelines.core")
                {
                    srp = true;
                    break;
                }
            }

            return srp && GraphicsSettings.renderPipelineAsset == null;
        }
    }
}