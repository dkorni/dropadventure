using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class BundleNameIssue : StepBasedIssue
    {
        public BundleNameIssue()
        {
            stepsList = new List<Step>()
            {
                BundleStep
            };

            Name = "Change your identifiers";
            Description = "You android Package Name should be the same as your iOS Bundle Identifier.";
            Priority = Priority.Low;
            AutomationType = AutomationType.Interactive;
            InteractiveWindowSize = new Vector2(450,200);
        }

        private Step BundleStep => new Step(() => !HasIssue, "Change your identifiers.", (issue, step) =>
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,EditorGUILayout.TextField("iOS Bundle Identifier",
                PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS)));
            EditorGUILayout.LabelField("Should be the same as :");
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,EditorGUILayout.TextField("Android Package Name",
                PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)));
        });

        public bool HasIssue => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) !=
                                PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
    }
}