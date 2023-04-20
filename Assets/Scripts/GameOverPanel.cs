using System;
using UnityEngine;
using Zenject;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private GameContext _gameContext;

    void Start()
    {
        _gameContext.OnStateChanged += OnStateChanged;
        gameObject.SetActive(false);
    }

    private void OnStateChanged(GameStates e)
    {
       if(e == GameStates.GameOver)
           gameObject.SetActive(true);
    }

    public void Retry()
    {
        //_game.Retry();
    }
}
