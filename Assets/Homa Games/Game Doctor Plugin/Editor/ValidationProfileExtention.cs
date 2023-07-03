using HomaGames.GameDoctor.Core;

namespace HomaGames.HomaBelly.GameDoctor
{
#pragma warning disable CS4014
    public static class ValidationProfileExtention
    {
        private static void OnResultGenerated(ICheck check)
        {
            EditorAnalytics.TrackGenericEditorEvent(new GameDoctorCheckExecutedQueryModel(check));
            foreach (var issue in check.CheckResult.Issues)
            {
                EditorAnalytics.TrackGenericEditorEvent(new GameDoctorIssueReportedQueryModel(check, issue));
            }
        }

        private static void OnIssueFixExecuted(ICheck check, IIssue issue, bool actuallyFixed)
        {
            EditorAnalytics.TrackGenericEditorEvent(new GameDoctorIssueFixExecutedQueryModel(issue, actuallyFixed));
        }

        public static void RegisterHomaAnalytics(this IValidationProfile validationProfile)
        {
            // Registering analytics
            foreach (var c in validationProfile.CheckList)
            {
                c.OnResultGenerated -= OnResultGenerated;
                c.OnIssueFixExecuted -= OnIssueFixExecuted;
                c.OnResultGenerated += OnResultGenerated;
                c.OnIssueFixExecuted += OnIssueFixExecuted;
            }
        }
    }
#pragma warning restore CS4014
}