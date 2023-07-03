using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;

namespace HomaGames.GameDoctor.Checks
{
    public class GraphicsSettingsCheck : BaseCheck
    {
        public GraphicsSettingsCheck() : base(
            "Graphics Settings",
            "Checking for issues in the Graphics Settings.", new HashSet<string>() {"settings", "graphics"},
            ImportanceType.Mandatory,
            Priority.Medium)
        {
        }

        protected override async Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();
            MissingPipelineFile missingPipelineFile = new MissingPipelineFile();
            bool hasIssue = await missingPipelineFile.HasIssue();
            if(hasIssue)
                result.Issues.Add(missingPipelineFile);
            return result;
        }
    }
}