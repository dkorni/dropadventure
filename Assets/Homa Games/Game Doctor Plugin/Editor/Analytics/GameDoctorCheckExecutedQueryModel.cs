using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using HomaGames.HomaBelly;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class GameDoctorCheckExecutedQueryModel : EditorAnalyticsModelBase
    {
        public GameDoctorCheckExecutedQueryModel(ICheck check) : base("check_executed")
        {
            EventCategory = GameDoctorPluginConstants.ANALYTICS_CATEGORY;
            EventValues.Add("check", new Dictionary<string, object>()
            {
                {"hash", check.GetHash()},
                {"type", check.GetType().FullName},
                {"name", check.Name},
                {"passed", check.CheckResult.Passed},
                {"priority", check.Priority.ToString()}
            });
        }
    }
}