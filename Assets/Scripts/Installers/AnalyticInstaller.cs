using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnalyticInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IAnalyticClient>().To<ConsoleAnalyticClient>().AsSingle();
        Container.Decorate<IAnalyticClient>().With<TinySauceClientDecorator>().AsSingle();
    }
}