using Assets.Scripts.Analytics;
using System;
using System.Diagnostics;
using UnityEngine;
using Zenject;
using System.Linq;


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
    private Puddle[] puddles;

    [SerializeField] private int maxDrops;

    [SerializeField] private int joinedDrops;
    private int processedComposites;

    [SerializeField] private GameContext _context;

    [Inject] private GameAnalyticManager _analyticManager;
    [Inject] private DynamicJoystick joystick;

    private Stopwatch stopwatch;

    private Bomb[] bombs;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        bombs = FindObjectsOfType<Bomb>();
        
        if(bombs.Length > 0)
        {
            var subscribers = FindObjectsOfType<MonoBehaviour>().OfType<IDetonationSubscriber>().ToArray();
            foreach (var bomb in bombs)
            {
                foreach (var subscriber in subscribers)
                {
                    bomb.OnDetonated += subscriber.OnDetonated;
                }
            }
        }

        puddles = FindObjectsOfType<Puddle>();
        _dropletController.OnJoin += IncreementDrops;
        _dropletController.OnFlush += OnFlush;
        _dropletController.OnDied += GameOver;
        _dropletController.OnWashedAway += Win;
        _context.OnReload += SilentGameOver;
        stopwatch = new Stopwatch();
        PrepareScene();
    }

    public void Update()
    {
        if(Input.touchCount > 0 && _context.CurrentState == GameStates.Preparing)
            StartMatch();
    }

    private void IncreementDrops()
    {
        joinedDrops++;
        _context.UpdateDropCount(joinedDrops);
        if (joinedDrops == maxDrops)
            StartFlushStep();
    }

    private void OnFlush()
    {
        _coinFactory.Withdraw(sink.position);
    }

    private void PrepareScene()
    {
        stopwatch.Start();
        _analyticManager.StartLevel(_context.CurrentLevel);
        
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

    private void SilentGameOver()
    {
        stopwatch.Stop();
        var seconds = stopwatch.Elapsed.Seconds;
        _analyticManager.FailLevel(_context.CurrentLevel, _dropletController.health, seconds);
    }

    private void GameOver()
    {
        if (_context.CurrentState == GameStates.GameOver)
            return;

        stopwatch.Stop();
        var seconds = stopwatch.Elapsed.Seconds;

        _context.UpdateStatus(GameStates.GameOver);

        _analyticManager.FailLevel(_context.CurrentLevel, _dropletController.health, seconds);
    }

    private void Win()
    {
        if (_context.CurrentState == GameStates.Win)
            return;

        stopwatch.Stop();
        var seconds = stopwatch.Elapsed.Seconds;
        _context.UpdateStatus(GameStates.Win);
        firework.SetActive(true);
        _analyticManager.CompleteLevel(_context.CurrentLevel, _dropletController.FlushedParticles, seconds);
        _analyticManager.AddCoins(_coinFactory.witdrawed, _context.CurrentLevel);
    }

    private void OnDisable()
    {
        _dropletController.OnJoin -= IncreementDrops;
        _dropletController.OnDied -= GameOver;
        _dropletController.OnDied -= Win;
        _context.OnReload -= SilentGameOver;
        _context.UpdateStatus(GameStates.None);
        foreach (var c in composites)
            c.OnFinished -= OnCompositeFinished;
    }
}