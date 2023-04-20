using System;
using UnityEngine;

public enum GameStates
{
    Preparing,
    Match,
    Flush,
    GameOver,
    Win
}

[CreateAssetMenu(fileName = "GameContext", menuName = "GameContext", order = 1)]
public class GameContext : ScriptableObject
{
    public GameStates CurrentState;

    public Action<GameStates> OnStateChanged;
    public Action<int> OnHealthUpdate;
    public Action<int> OnMaxHealthUpdate;
    public Action<int> OnDropCountCollectedUpdate;
    public Action<LevelData> OnLevelUpdated;

    private DropletController _dropletController;
    
    public void UpdateStatus(GameStates newState)
    {
        if(CurrentState == newState)
            return;

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void UpdateHealth(int value) => OnHealthUpdate?.Invoke(value);
    public void UpdateMaxHealth(int value) => OnMaxHealthUpdate?.Invoke(value);
    public void UpdateDropCount(int value) => OnDropCountCollectedUpdate?.Invoke(value);
    public void UpdateMaxDropCount(int value) => OnDropCountCollectedUpdate?.Invoke(value);
    public void UpdateLevel(LevelData level) => OnLevelUpdated?.Invoke(level);
}