using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CanvasTips : MonoBehaviour
{
    public GameObject DefaultTips;
    public GameObject LaserTips;
    public GameObject CutTips;

    [Inject] GameContext gameContext;

    // Start is called before the first frame update
    void Start()
    {
        gameContext.OnLevelUpdated += OnLevelUpdated;
        gameContext.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameStates states)
    {
        if(states == GameStates.Match)
        {
            LaserTips.SetActive(false);
            CutTips.SetActive(false);
            DefaultTips.SetActive(false);
        }
    }

    private void OnLevelUpdated(LevelData levelData)
    {
        if (levelData.IsLaserTutorialNeeded)
        {
            LaserTips.SetActive(true);
            CutTips.SetActive(false);
            DefaultTips.SetActive(false);
        }
        else if (levelData.IsCutTutorialNeeded)
        {
            LaserTips.SetActive(false);
            CutTips.SetActive(true);
            DefaultTips.SetActive(false);
        }
        else
        {
            LaserTips.SetActive(false);
            CutTips.SetActive(false);
            DefaultTips.SetActive(true);
        }
    }

    private void OnDisable()
    {
        gameContext.OnLevelUpdated -= OnLevelUpdated;
        gameContext.OnStateChanged -= OnStateChanged;
    }
}