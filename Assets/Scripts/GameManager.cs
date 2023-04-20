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
    
    private int maxDrops;
    private int joinedDrops;
    private int processedComposites;
    
    [Inject] private GameContext _context;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
    private void Start()
    {
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
        if (joinedDrops == maxDrops) StartFlushStep();
    }

    private void OnFlush()
    {
        _coinFactory.Withdraw(sink.position);
    }

    private void PrepareScene()
    {
        maxDrops = FindObjectsOfType<Puddle>().Length;

        _context.UpdateStatus(GameStates.Preparing);
        _context.UpdateLevel(_levelData);
        _context.UpdateMaxDropCount(maxDrops);
        _dropletController.OnMaxHealthUpdate += _context.UpdateMaxHealth;
        _dropletController.OnHealthUpdate += _context.UpdateHealth;

        foreach (var c in composites)
            c.OnFinished += OnCompositeFinished;

        _beginPlatform.gameObject.SetActive(true);
        _endPlatform.gameObject.SetActive(true);
        _coinFactory.InitializeCoins(_dropletController.MaxHealth);
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