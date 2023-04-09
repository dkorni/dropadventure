using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public enum GameStates
{
    Preparing,
    Match,
    Flush,
    GameOver,
    Win
}

public class Game : MonoBehaviour
{
    private static int currentScene;

    [Inject]
    private DropletController _dropletController;

    public EventHandler<GameStates> OnStateChanged;

    [SerializeField] GameStates _currentState;

    [SerializeField] PlaneController _beginPlatform;
    [SerializeField] PlaneController _endPlatform;

    [SerializeField] CompositeObject[] composites;

    private int maxDrops;
    private int joinedDrops;
    private int processedComposites;

    // Start is called before the first frame update
    void Start()
    {
       PrepareScene();
        _dropletController.OnJoin += IncreementDrops;
        _dropletController.OnDied += GameOver;
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
            StartFlushStep();
        }
    }
    
    private void PrepareScene()
    {
        UpdateStatus(GameStates.Preparing);
        foreach (var c in composites)
            c.OnFinished += OnCompositeFinished;

        _beginPlatform.gameObject.SetActive(true);
        _endPlatform.gameObject.SetActive(true);
    }

    private void OnCompositeFinished()
    {
        processedComposites++;
        if(processedComposites == composites.Length)
        {
            StartMatch();
        }
    }

    private void StartMatch()
    {
        _endPlatform.gameObject.SetActive(false);
        UpdateStatus(GameStates.Match);
        _beginPlatform.gameObject.SetActive(true);
    }

    private void StartFlushStep()
    {
        UpdateStatus(GameStates.Flush);
        _beginPlatform.gameObject.SetActive(false);
        _endPlatform.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        UpdateStatus(GameStates.GameOver);
    }

    private void UpdateStatus(GameStates newState)
    {
        _currentState = newState;
        OnStateChanged?.Invoke(this, newState);
    }

    void OnDisable()
    {
        _dropletController.OnJoin -= IncreementDrops;
        _dropletController.OnDied -= GameOver;

        foreach (var c in composites)
            c.OnFinished -= OnCompositeFinished;
    }
}