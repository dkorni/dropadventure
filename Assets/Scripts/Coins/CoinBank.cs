using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Analytics;
using ModestTree;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

[CreateAssetMenu(fileName = "CoinBank", menuName = "CoinBank", order = 1)]
public class CoinBank : ScriptableObject
{
    public int Amount;

    public Action<int> OnBalanceUpdated;
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("balance"))
            Amount = PlayerPrefs.GetInt("balance");
    }

    public void UpdateBalance(int amount)
    {
        Amount += amount;
        PlayerPrefs.SetInt("balance", this.Amount);
        OnBalanceUpdated?.Invoke(Amount);
    }
}