using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public enum GameStates
{
    Preparing,
    Match,
    GameOver,
    Win
}

public class Game : MonoBehaviour
{
    private static int currentScene;

    [Inject]
    private DropletController _dropletController;

    public EventHandler<GameStates> OnStateChanged;

    // Start is called before the first frame update
    void Start()
    {
        _dropletController.OnDied += () => OnStateChanged?.Invoke(this, GameStates.GameOver);
    }

    public void Retry()
    {
        SceneManager.LoadScene(currentScene);
    }

    public void NextLevel()
    {
        currentScene++;
        SceneManager.LoadScene(currentScene);
    }
}
