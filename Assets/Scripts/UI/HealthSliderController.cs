using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    
    [Inject]
    private GameContext gameContext;
    
    void Awake()
    {
        gameContext.OnMaxHealthUpdate += UpdateMax;
        gameContext.OnHealthUpdate += UpdateSlider;
        gameContext.OnLevelUpdated += OnLevelUpdated;
    }

    private void OnLevelUpdated(LevelData obj)
    {
        fill.color = obj.HealthSliderColor;
    }

    void UpdateSlider(int value)
    {
        slider.value = value;
    }

    void UpdateMax(int value)
    {
        slider.maxValue = value;
    }

    private void OnDisable()
    {
        gameContext.OnMaxHealthUpdate -= UpdateMax;
        gameContext.OnHealthUpdate -= UpdateSlider;
        gameContext.OnLevelUpdated -= OnLevelUpdated;
    }
}