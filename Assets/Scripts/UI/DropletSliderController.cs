using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DropletSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    
    [Inject]
    private GameContext gameContext;
    
    void Awake()
    {
        gameContext.OnMaxDropCountUpdate += UpdateMax;
        gameContext.OnDropCountCollectedUpdate += UpdateSlider;
        gameContext.OnLevelUpdated += OnLevelUpdated;
    }

    private void OnLevelUpdated(LevelData obj)
    {
        slider.value = 0;
        fill.color = obj.DropLeftSliderColor;
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
        gameContext.OnMaxDropCountUpdate -= UpdateMax;
        gameContext.OnDropCountCollectedUpdate -= UpdateSlider;
        gameContext.OnLevelUpdated -= OnLevelUpdated;
    }
}