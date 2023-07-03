using System;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using HomaGames.GameDoctor.Ui;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly.GameDoctor
{
    public static class HomaValidationProfileRoutine
    {
        private const string LastTimePromptedPrefsKey = "homagames.gamedoctor.last_time_checked";
        private static readonly long PromptFrequencyTicks = new TimeSpan(72, 0, 0).Ticks;

        private static long LastTimePrompted
        {
            get => long.TryParse(EditorPrefs.GetString(LastTimePromptedPrefsKey, "0"), out var result) ? result : 0;
            set => EditorPrefs.SetString(LastTimePromptedPrefsKey, value.ToString());
        }

        private static bool NeedsToShowPopup => !Application.isBatchMode && 
                                                (DateTime.Now.Ticks - LastTimePrompted > PromptFrequencyTicks);
        

        [InitializeOnLoadMethod]
        private static async void Routine()
        {
            if (!NeedsToShowPopup) return;
            await Task.Delay(1000);
            var result =
                EditorUtility.DisplayDialog("Game Doctor 🩺",
                    "💊 Try scanning your project with Game Doctor, and automatically fix all issues !\n\n\n" +
                    "💊 A custom Validation Profile is already created and customized for your project,\n you just need to run the checks we cooked for you.", "Let's try",
                    "Later");
            if (result && AvailableProfiles.TryGetValidationProfileByName("Homa Validation Profile",
                    out var homaProfile))
            {
                GameDoctorWindow.Open(homaProfile);
            }

            LastTimePrompted = DateTime.Now.Ticks;
            
            await EditorAnalytics.TrackGenericEditorEvent(new GameDoctorPopupQueryModel(result));
        }
    }
}