using HomaGames.GameDoctor.Core;
using UnityEditor;

namespace HomaGames.HomaBelly.GameDoctor
{
    public static class CheckRegistration
    {
        [InitializeOnLoadMethod]
        public static void AddHomaBellyChecks()
        {
            AvailableChecks.RegisterCheck(new HomaBellyImplementationCheck());
        }
    }
}