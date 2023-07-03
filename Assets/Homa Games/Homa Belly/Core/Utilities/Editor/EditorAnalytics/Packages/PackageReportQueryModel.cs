#if !HOMA_BELLY_EDITOR_ANALYTICS_DISABLED
using System.Collections.Generic;

namespace HomaGames.HomaBelly
{
    public class PackageReportQueryModel : EditorAnalyticsModelBase
    {
        private static readonly string PARAM_PACKAGES_LIST = "packages";

        public PackageReportQueryModel(string eventName, Dictionary<string,string> packageData) : base(eventName)
        {
            EventValues.Add(PARAM_PACKAGES_LIST, packageData);
        }
    }
}
#endif