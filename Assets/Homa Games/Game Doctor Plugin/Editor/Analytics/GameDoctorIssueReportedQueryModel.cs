using System.Collections.Generic;
using HomaGames.GameDoctor.Core;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class GameDoctorIssueReportedQueryModel : EditorAnalyticsModelBase
    {
        public GameDoctorIssueReportedQueryModel(ICheck check, IIssue issue) : base("issue_reported")
        {
            EventCategory = GameDoctorPluginConstants.ANALYTICS_CATEGORY;
            EventValues.Add("issue", new Dictionary<string, object>()
            {
                {"hash", issue.GetHash()},
                {"check_hash", check.GetHash()},
                {"type", issue.GetType().FullName},
                {"name", issue.Name},
                {"priority",issue.Priority.ToString()},
                {"automation", issue.AutomationType},
                {"dismissed", issue.HasBeenDismissed()}
            });
        }
    }
}