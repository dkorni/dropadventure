using System;
using Obi;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SolverStore : MonoBehaviour
{
    [Inject] private Game game;

    [SerializeField] private List<ObiEmitter> _playerPuddles;

    [SerializeField] private ObiEmitter[] _puddles;

    private int _onlyPuddlesCount;

    private int _count;

    private int _activePuddleCount;

    private void Start()
    {
        _playerPuddles.Capacity += _puddles.Length;
        _count += _playerPuddles.Sum(e => e.activeParticleCount);
        _onlyPuddlesCount = _puddles.Sum(e => e.activeParticleCount);
        _count += _onlyPuddlesCount;
        
    }

    public void UpdatePlayerPuddles(ObiEmitter newPlayerPart)
    {
        _playerPuddles.Add(newPlayerPart);

        if (_playerPuddles.Count == _puddles.Length + 1)
        {
            // win
            game.OnStateChanged.Invoke(this, GameStates.Win);
        }
    }

    public bool IsAllPlayerPuddlesDied()
    {
        // todo optimize count only when update
        return _playerPuddles.Sum(p => p.activeParticleCount) == 0;
    }
    
    public int GetAllDropCount()
    {
        return _puddles.Sum(p => p.particleCount);
    }
    
    public int GetPlayerDropCount()
    {
        return _playerPuddles.Sum(p => p.particleCount);
    }
}
