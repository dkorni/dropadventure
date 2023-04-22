using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DependencyInstaller : MonoInstaller
{
    public GameContext Context;
    public CoinBank CoinBank;
    
    public override void InstallBindings()
    {
        Container.Bind<GameContext>().FromInstance(Context);
        Container.Bind<CoinBank>().FromInstance(CoinBank);
    }
}