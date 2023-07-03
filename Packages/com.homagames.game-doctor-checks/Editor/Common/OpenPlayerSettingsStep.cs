using System;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class OpenPlayerSettingsStep : Step
    {
        private string _setting;

        public OpenPlayerSettingsStep(string setting = "Player")
        {
            _predicateFunc = IsPlayerSettingsOpen;
            _name = $"Go to {setting} Settings";
            _drawFunc = Draw;
            _setting = setting;
        }

        private void Draw(StepBasedIssue issue, Step step)
        {
            if (GUILayout.Button($"Open {_setting} settings"))
            {
                SettingsService.OpenProjectSettings($"Project/{_setting}");
            }
        }

        private bool IsPlayerSettingsOpen() => true;
    }
}