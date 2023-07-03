using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class HomaBellyImplementationCheck : BaseCheck
    {
        private static readonly List<MethodDescription> MethodsThatNeedsToBeCalled =
            new List<MethodDescription>()
            {
                new MethodDescription(typeof(HomaBelly), new HashSet<string>() {"HomaGames.HomaBelly"},
                    "ShowInterstitial",
                    "https://support.homagames.com/knowledge/homa-belly-interstitial-ads",
                    "You should consider doing so by looking at the documentation <a href=\"https://support.homagames.com/knowledge/homa-belly-interstitial-ads\">here</a>."),
                new MethodDescription(typeof(DefaultAnalytics), new HashSet<string>() {"HomaGames.HomaBelly"},
                    "GameplayStarted",
                    "https://support.homagames.com/knowledge/homa-belly-analytics-and-events#default-analytics",
                    "Have a look at the documentation for events you should implement <a href=\"https://support.homagames.com/knowledge/homa-belly-analytics-and-events#default-analytics\">here</a>."),
                new MethodDescription(typeof(DefaultAnalytics), new HashSet<string>() {"HomaGames.HomaBelly"},
                    "MainMenuLoaded",
                    "https://support.homagames.com/knowledge/homa-belly-analytics-and-events#default-analytics",
                    "Have a look at the documentation for events you should implement <a href=\"https://support.homagames.com/knowledge/homa-belly-analytics-and-events#default-analytics\">here</a>."),
                new MethodDescription(typeof(DefaultAnalytics), new HashSet<string>() {"HomaGames.HomaBelly"},
                    "SuggestedRewardedAd",
                    "https://support.homagames.com/knowledge/homa-belly-analytics-and-events#default-analytics",
                    "Have a look at the documentation for events you should implement <a href=\"https://support.homagames.com/knowledge/homa-belly-analytics-and-events#default-analytics\">here</a>.")
            };

        private static readonly List<MethodDescription> MethodsThatShouldNotBeCalled =
            new List<MethodDescription>()
            {
                new MethodDescription(
                    Type.GetType(
                        "MaxSdkUnityEditor, MaxSdk.Scripts, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
                    new HashSet<string>() {"HomaGames.HomaBelly"},
                    "CreateBanner", "https://support.homagames.com/knowledge/homa-belly-banner-ads",
                    "You must not call MaxSdk directly, Homa Belly provides an API to display ads, have a look at the documentation <a href=\"https://support.homagames.com/knowledge/homa-belly-banner-ads\">here</a>"),
                new MethodDescription(
                    Type.GetType(
                        "MaxSdkUnityEditor, MaxSdk.Scripts, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
                    new HashSet<string>() {"HomaGames.HomaBelly"},
                    "ShowInterstitial", "https://support.homagames.com/knowledge/homa-belly-interstitial-ads",
                    "You must not call MaxSdk directly, Homa Belly provides an API to display ads, have a look at the documentation <a href=\"https://support.homagames.com/knowledge/homa-belly-interstitial-ads\">here</a>"),
                new MethodDescription(
                    Type.GetType(
                        "MaxSdkUnityEditor, MaxSdk.Scripts, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
                    new HashSet<string>() {"HomaGames.HomaBelly"},
                    "ShowRewardedAd", "https://support.homagames.com/knowledge/homa-belly-rewarded-video-ads",
                    "You must not call MaxSdk directly, Homa Belly provides an API to display ads, have a look at the documentation <a href=\"https://support.homagames.com/knowledge/homa-belly-rewarded-video-ads\">here</a>"),
                new MethodDescription(
                    Type.GetType(
                        "GameAnalyticsSDK.GameAnalytics, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
                    new HashSet<string>() {"HomaGames.HomaBelly", "GameAnalyticsSDK"},
                    "NewDesignEvent",
                    "https://support.homagames.com/knowledge/homa-belly-analytics-and-events#tracking-events",
                    "You must not call GameAnalytics directly, Homa Belly provides an API to track events, have a look at the documentation <a href=\"https://support.homagames.com/knowledge/homa-belly-analytics-and-events#tracking-events\">here</a>")
            };

        public HomaBellyImplementationCheck() : base("Homa Belly Implementation",
            "Checks for issues in how Homa Belly is implemented in the project.",
            new HashSet<string>() {"Homa Belly", "implementation", "analysis"},
            ImportanceType.Advised, Priority.Medium)
        {
        }

        protected override Task<CheckResult> GenerateCheckResult()
        {
            var result = new CheckResult();
            foreach (var method in MethodsThatNeedsToBeCalled)
            {
                var usages = method.GetUsages();
                if (usages.Count == 0)
                    result.Issues.Add(new MissingCallIssue(method, Priority.Medium));
            }

            foreach (var method in MethodsThatShouldNotBeCalled)
            {
                var usages = method.GetUsages();
                if (usages.Count > 0)
                    result.Issues.Add(new UnwantedCallIssue(method, usages, Priority.Medium));
            }

            return Task.FromResult(result);
        }
    }
}