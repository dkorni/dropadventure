using System.Collections.Generic;
using HomaGames.GameDoctor.Core;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class GameDoctorIssueFixExecutedQueryModel : EditorAnalyticsModelBase
    {
        public GameDoctorIssueFixExecutedQueryModel(IIssue issue, bool actuallyFixed) : base("issue_fixed")
        {
            EventCategory = GameDoctorPluginConstants.ANALYTICS_CATEGORY;
            EventValues.Add("issue", new Dictionary<string, object>()
            {
                {"hash", issue.GetHash()},
                {"fixed", actuallyFixed},
                {"priority", issue.Priority.ToString()},
                {"automation_type", issue.AutomationType.ToString()}
            });
        }
    }
}