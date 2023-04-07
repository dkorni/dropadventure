using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DropletSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [Inject] private DropletController dropletController;
    
    // Start is called before the first frame update
    void Start()
    {
        dropletController.OnMaxHealthUpdate += UpdateMax;
        dropletController.OnHealthUpdate += UpdateSlider;
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
        dropletController.OnMaxHealthUpdate -= UpdateMax;
        dropletController.OnHealthUpdate -= UpdateSlider;
    }
}