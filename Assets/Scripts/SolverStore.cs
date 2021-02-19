using Obi;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolverStore : MonoBehaviour
{
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
    }

    public bool IsAllPlayerPuddlesDied()
    {
        return _playerPuddles.Sum(p => p.activeParticleCount) == 0;
    }
}
