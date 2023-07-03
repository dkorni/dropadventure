using System;
using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class IconIssue : StepBasedIssue
    {
        public IconIssue()
        {
            Name = "No Default Icon";
            Description = "You should assign a default Icon for your game.";
            stepsList = new List<Step>()
            {
                new Step(() => !HasIssue, "Assign a texture icon.", DrawStep)
            };
            AutomationType = AutomationType.Interactive;
        }

        private void DrawStep(StepBasedIssue issue, Step step)
        {
            var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
            var newIcon = EditorGUILayout.ObjectField("Game Icon", icons[0], typeof(Texture2D), false) as Texture2D;
            icons[0] = newIcon;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);
        }

        public static bool HasIssue
        {
            get
            {
                var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
                return !(icons.Length > 0) || !icons[0];
            }
        }
    }
}