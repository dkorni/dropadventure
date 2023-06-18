using Assets.Scripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DependencyInstaller : MonoInstaller
{
    public GameContext Context;
    public CoinBank CoinBank;
    public PlaneSO PlaneSO;
    public GameObject LaserPrefab;

    public override void InstallBindings()
    {
        Container.Bind<GameContext>().FromInstance(Context);
        Container.Bind<CoinBank>().FromInstance(CoinBank);
        Container.Bind<PlaneSO>().FromInstance(PlaneSO);
        Container.InstantiatePrefab(LaserPrefab);
    }
}
