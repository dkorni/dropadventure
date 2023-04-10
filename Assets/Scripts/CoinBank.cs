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
    private int amount;

    public Action<int> OnBalanceUpdated;

    private int increement = 1;
    private int maxCoins;
    private int devider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("balance"))
            amount = PlayerPrefs.GetInt("balance");
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
        this.amount += amount;
        PlayerPrefs.SetInt("balance", this.amount);
    }

    private IEnumerator WithdrawCoroutine(Vector3 position)
    {
        increement++;
        if (increement%devider == 0)
        {
            yield return new WaitForSeconds(withdrawDelay);
            var coin = coins.Dequeue();
            coin.SetActive(true);
            coin.transform.position = position;
            UpdateBalance(amount);
            OnBalanceUpdated?.Invoke(amount);
        }
    }
}