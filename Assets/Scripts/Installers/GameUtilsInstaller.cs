using Assets.Scripts.Analytics;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    internal class GameUtilsInstaller : MonoInstaller
    {
        public GameObject Prefab;

        public override void InstallBindings()
        {
            var instance = Container.InstantiatePrefab(Prefab);
        }
    }
}
