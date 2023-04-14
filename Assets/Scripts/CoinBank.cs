using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinBank : MonoBehaviour
{
    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private float withdrawDelay = 1.2f;

    private Queue<GameObject> coins = new Queue<GameObject>();
    public int Amount;

    public Action<int> OnBalanceUpdated;

    private int increement = 1;
    private int maxCoins;
    private int devider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("balance"))
            Amount = PlayerPrefs.GetInt("balance");
    }

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

    private void UpdateBalance(int amount)
    {
        this.Amount += amount;
        PlayerPrefs.SetInt("balance", this.Amount);
    }

    private IEnumerator WithdrawCoroutine(Vector3 position)
    {
        // if(coins )
        increement++;
        if (increement%devider == 0)
        {
            yield return new WaitForSeconds(withdrawDelay);
            var coin = coins.Dequeue();
            coin.SetActive(true);
            coin.transform.position = position;
            UpdateBalance(1);
            OnBalanceUpdated?.Invoke(Amount);
        }
    }
}