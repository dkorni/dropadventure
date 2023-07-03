using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEngine;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class UnwantedCallIssue : ImplementationIssue
    {
        public UnwantedCallIssue(MethodDescription shouldNotImplement, List<MethodBase> usages,
            Priority priority) : base(shouldNotImplement)
        {
            Name = $"{shouldNotImplement.MethodName} shouldn't be called";
            Description = $"Found implementation of {shouldNotImplement.FullMethodName} in \n";
            foreach (var usage in usages)
            {
                if (usage.DeclaringType != null)
                    Description += $" - {usage.DeclaringType.FullName}.{usage.Name}\n";
            }

            Description += $"\n{shouldNotImplement.AdditionalDescription}";
            AutomationType = AutomationType.Interactive;
            Priority = priority;
        }
    }
}