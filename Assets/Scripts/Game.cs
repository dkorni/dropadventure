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

    private int maxDrops;
    private int joinedDrops;

    // Start is called before the first frame update
    void Start()
    {
        _dropletController.OnJoin += IncreementDrops;
        _dropletController.OnDied += () => OnStateChanged?.Invoke(this, GameStates.GameOver);
        maxDrops = FindObjectsOfType<Puddle>().Length;
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

    private void IncreementDrops()
    {
        joinedDrops++;
        if(joinedDrops == maxDrops)
        {
            SpawnSink();
        }
    }

    private void SpawnSink()
    {
        Debug.Log("Sink spawned");
    }
}
