using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class CanvasInstaller : MonoInstaller
    {
        public GameObject Prefab;

        public override void InstallBindings()
        {
            var canvas = FindObjectOfType<Canvas>();
            if(canvas == null)
            {
                canvas = Container.InstantiatePrefab(Prefab).GetComponent<Canvas>();
            }

            Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
        }
    }
}
