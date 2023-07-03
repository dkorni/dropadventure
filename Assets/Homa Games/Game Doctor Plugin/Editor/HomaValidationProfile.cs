using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly.GameDoctor
{
    /// <summary>
    /// Validation Profile populated with tags from the game_doctor "validation_tags" fields on Homa Lab.
    /// </summary>
    public class HomaValidationProfile : IValidationProfile
    {
        [InitializeOnLoadMethod]
        public static void Register()
        {
            var profile = new HomaValidationProfile();
            AvailableProfiles.SetDefaultValidationProfile(profile);
            // Giving time for checks to be registered
            EditorApplication.delayCall += profile.RegisterHomaAnalytics;
        }

        public string Name => "Homa Validation Profile";
        public string Description => "A dynamically generated Validation Profile from Homa Games";

        public HashSet<ICheck> CheckList
        {
            get
            {
                if (!HomaBellyManifestConfiguration.TryGetString(out string tags, "game_doctor", "s_validation_tags"))
                    return new HashSet<ICheck>(AvailableChecks.GetAllChecks());
                var allTags = tags.Split(';');
                return new HashSet<ICheck>(allTags.SelectMany(AvailableChecks.GetAllChecksWithTag));
            }
        }
    }
}