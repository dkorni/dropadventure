using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MinHealthSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [Inject]
    private GameContext gameContext;
    
    void Awake()
    {
        gameContext.OnMaxHealthUpdate += UpdateMax;
        gameContext.OnLevelUpdated += OnLevelUpdated;
    }

    private void OnLevelUpdated(LevelData obj)
    {
        slider.value = obj.MinHealth;
    }

    void UpdateMax(int value)
    {
        slider.maxValue = value;
    }

    private void OnDisable()
    {
        gameContext.OnMaxHealthUpdate -= UpdateMax;
        gameContext.OnLevelUpdated -= OnLevelUpdated;
    }
}