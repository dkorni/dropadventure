using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Installers
{
    public class DynamicJoystickInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var canvas = Container.Resolve<Canvas>();
            var controls = canvas.GetComponentInChildren<DynamicJoystick>();
            Container.Bind<DynamicJoystick>().FromInstance(controls).AsSingle();
        }
    }
}
