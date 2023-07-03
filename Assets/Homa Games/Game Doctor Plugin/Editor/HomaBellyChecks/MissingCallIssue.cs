using System.Reflection;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor.Compilation;
using UnityEngine;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class MissingCallIssue : ImplementationIssue
    {
        public MissingCallIssue(MethodDescription shouldImplement, Priority priority) : base(shouldImplement)
        {
            Name = $"{shouldImplement.MethodName} implementation is missing";
            Description =
                $"Implementation of {shouldImplement.FullMethodName} is required by Homa Belly.\n\n{shouldImplement.AdditionalDescription}";
            AutomationType = AutomationType.Interactive;
            Priority = priority;
        }
    }
}