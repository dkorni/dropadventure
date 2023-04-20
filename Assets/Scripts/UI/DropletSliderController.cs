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
    
    [Inject]
    private GameContext gameContext;
    
    void Start()
    {
        gameContext.OnMaxHealthUpdate += UpdateMax;
        gameContext.OnHealthUpdate += UpdateSlider;
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
    }
}