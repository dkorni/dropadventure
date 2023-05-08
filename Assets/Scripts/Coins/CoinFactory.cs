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
    [Inject] private GameAnalyticManager analyticManager;
    
    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private float withdrawDelay = 1.2f;

    private Queue<GameObject> coins = new Queue<GameObject>();
    
    private int increement = 1;
    public int witdrawed;
    private int maxCoins;
    private int devider;

    private bool IsWithdrawing;
    
    public void InitializeCoins(int count)
    {
        maxCoins = UnityEngine.Random.Range(3, 5);
        devider = count / maxCoins;

        for (int a = 0; a < maxCoins; a++)
        {
            var g = Instantiate(coinPrefab);
            coins.Enqueue(g);
            g.SetActive(false);
        }
    }

    public void Withdraw(Vector3 position)
    {
        StartCoroutine(WithdrawCoroutine(position));
    }
    
    private IEnumerator WithdrawCoroutine(Vector3 position)
    {
        while (IsWithdrawing)
        {
            yield return null;
        }

        IsWithdrawing = true;

        if (coins.IsEmpty())
            yield break;

        witdrawed++;
        yield return new WaitForSeconds(withdrawDelay * witdrawed);


        var coin = coins.Dequeue();
        coin.SetActive(true);
        coin.transform.position = position;
        Bank.UpdateBalance(1);
        IsWithdrawing = false;

    }
}
