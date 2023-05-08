using Assets.Scripts.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    internal class GameAnalyticInstaller : MonoInstaller
    {
        public GameObject Prefab;

        public override void InstallBindings()
        {
            var instance = Container.InstantiatePrefab(Prefab).GetComponent<GameAnalyticManager>();
            Container.Bind<GameAnalyticManager>().FromInstance(instance).AsSingle();
        }
    }
}
