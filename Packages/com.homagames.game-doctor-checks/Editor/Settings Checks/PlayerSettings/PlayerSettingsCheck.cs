using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class PlayerSettingsCheck : BaseCheck
    {
        public PlayerSettingsCheck() : base(
            "Player Settings",
            "Checking for issues in the Player Settings.", new HashSet<string>() {"player", "settings"},
            ImportanceType.Mandatory,
            Priority.Medium)
        {
        }

        protected override Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();

            bool CompanyCheck() => PlayerSettings.companyName != "DefaultCompany";
            if (!CompanyCheck())
            {
                result.Issues.Add(new StepBasedIssue(new List<Step>()
                {
                    new Step(CompanyCheck, "Rename your company to something else.", (issue, step) =>
                    {
                        PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);
                    })
                }, "Company Name", "You should choose a company name in Player Settings.", true, Priority.High));
            }

            var bundleIssue = new BundleNameIssue();
            if (bundleIssue.HasIssue)
                result.Issues.Add(bundleIssue);

            bool SplashImageCheck() => !Application.HasProLicense() || !PlayerSettings.SplashScreen.showUnityLogo;

            if (!SplashImageCheck())
            {
                result.Issues.Add(new SimpleIssue(() =>
                    {
                        PlayerSettings.SplashScreen.showUnityLogo = false;
                        return Task.FromResult(true);
                    }, "Unity Logo",
                    "You shouldn't show the Unity logo in the splashscreen if you have a pro licence."));
            }

            if (IconIssue.HasIssue)
                result.Issues.Add(new IconIssue());

            return Task.FromResult(result);
        }
    }
}