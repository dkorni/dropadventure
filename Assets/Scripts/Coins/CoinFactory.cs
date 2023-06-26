using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Analytics;
using ModestTree;
using UnityEngine;
using Zenject;

public class CoinFactory : MonoBehaviour
{
    [Inject] private CoinBank Bank;
    [Inject] private DropletController DropletController;
    
    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private float withdrawDelay = 1.2f;
    
    public int witdrawed;
    private int maxCoins;

    private List<int> steps;
    private const int MaxPersantage = 100; 

    private void Start()
    {
        steps = new List<int>();
    }

    public void InitializeCoins(int count)
    {
        maxCoins = Random.Range(3, 5);
        int persantageStep = MaxPersantage / maxCoins;
        for (int i = 1; i < maxCoins+1; i++) steps.Add(persantageStep*i);
        steps.Reverse();
    }

    public void Withdraw(Vector3 position)
    {
        for (int i = 0; i < maxCoins; i++)
        {
            if(DropletController.HealthPersantage < steps[i])
            {
                Instantiate(coinPrefab, position, Quaternion.identity);
                steps[i] = -1;
                Bank.UpdateBalance(1);
            }
        }
    }
}