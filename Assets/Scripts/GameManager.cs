using System;
using UnityEngine;
using Zenject;



public class GameManager : MonoBehaviour
{
    [SerializeField] private PlaneController _beginPlatform;
    [SerializeField] private PlaneController _endPlatform;

    [SerializeField] private CompositeObject[] composites;
    [SerializeField] private GameObject firework;
    [SerializeField] private Transform sink;

    [SerializeField] private LevelData _levelData;
    [SerializeField] private DropletController _dropletController;

    [SerializeField] private CoinFactory _coinFactory;

    [SerializeField] private Puddle[] puddles;

    [SerializeField] private int maxDrops;

    [SerializeField] private int joinedDrops;
    private int processedComposites;
    
    [SerializeField] private GameContext _context;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        _dropletController.OnJoin += IncreementDrops;
        _dropletController.OnFlush += OnFlush;
        _dropletController.OnDied += GameOver;
        _dropletController.OnWashedAway += Win;
        PrepareScene();
    }

    private void IncreementDrops()
    {
        joinedDrops++;
        _context.UpdateDropCount(joinedDrops);
        if (joinedDrops == maxDrops && _dropletController.health > _levelData.MinHealth) 
            StartFlushStep();
    }

    private void OnFlush()
    {
        _coinFactory.Withdraw(sink.position);
    }

    private void PrepareScene()
    {
        maxDrops = puddles.Length;
        _context.UpdateStatus(GameStates.Preparing);
        _context.UpdateMaxDropCount(maxDrops);
        _dropletController.OnMaxHealthUpdate += (x) =>
        {
            _context.UpdateMaxHealth(x);
            _coinFactory.InitializeCoins(x);
            _context.UpdateLevel(_levelData);
        };
        _dropletController.OnHealthUpdate += _context.UpdateHealth;

        foreach (var c in composites)
            c.OnFinished += OnCompositeFinished;

        _beginPlatform.gameObject.SetActive(true);
        _endPlatform.gameObject.SetActive(false);
    }

    private void OnCompositeFinished()
    {
        processedComposites++;
        if (processedComposites == composites.Length) StartMatch();
    }

    private void StartMatch()
    {
        _endPlatform.gameObject.SetActive(false);
        _context.UpdateStatus(GameStates.Match);
        _beginPlatform.gameObject.SetActive(true);
    }

    private void StartFlushStep()
    {
        _context.UpdateStatus(GameStates.Flush);
        _endPlatform.transform.rotation = _beginPlatform.transform.rotation;
        _beginPlatform.gameObject.SetActive(false);
        _endPlatform.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        _context.UpdateStatus(GameStates.GameOver);
    }

    private void Win()
    {
        _context.UpdateStatus(GameStates.Win);
        firework.SetActive(true);
    }

    private void OnDisable()
    {
        _dropletController.OnJoin -= IncreementDrops;
        _dropletController.OnDied -= GameOver;
        _dropletController.OnDied -= Win;

        foreach (var c in composites)
            c.OnFinished -= OnCompositeFinished;
    }
}