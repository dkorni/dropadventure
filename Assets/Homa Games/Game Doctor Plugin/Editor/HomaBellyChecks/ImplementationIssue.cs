using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEngine;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class ImplementationIssue : BaseIssue
    {
        protected MethodDescription _methodDescription;

        public ImplementationIssue(MethodDescription methodDescription)
        {
            _methodDescription = methodDescription;
        }

        protected override async Task<bool> InternalFix()
        {
            Application.OpenURL(_methodDescription.MainDocumentationLink);
            await Task.Delay(10000000);
            return await Task.FromResult(true);
        }
    }
}