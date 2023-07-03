using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;

namespace HomaGames.GameDoctor.Checks
{
    public class BuildSettingsCheck : BaseCheck
    {
        public BuildSettingsCheck() : base(
            "Build Settings",
            "Checking for issues in the Build Settings.", new HashSet<string>() {"build", "settings"},
            ImportanceType.Mandatory,
            Priority.High)
        {
        }

        protected override Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();
            MissingSceneIssue issue = new MissingSceneIssue();
            if (issue.HasIssue)
                result.Issues.Add(issue);
            return Task.FromResult(result);
        }
    }
}