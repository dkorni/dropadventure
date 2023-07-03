using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Interface exposed with Homa Belly to interact
    /// with any of its products: mediations, attributions or analytics.
    /// </summary>
    public interface IHomaBellyBridge : IHomaBelly
    {
        void Initialize();
        
        void InitializeInternetConnectivityDependantComponents();

        void InitializeRemoteConfigurationDependantComponents(
            RemoteConfiguration.RemoteConfigurationModelEveryTime remoteConfigurationModel);

        void OnApplicationPause(bool pause);
    }
}
