using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ProgressController : MonoBehaviour
{
    [Inject] private GameContext _context;
    [SerializeField] private Text currentLevelText;
    [SerializeField] private Text nextLevelText;

    void Awake()
    {
        _context.OnLevelUpdated += OnLevelUpdated;
    }

    private void OnLevelUpdated(LevelData obj)
    {
        currentLevelText.text = obj.Index.ToString();
        nextLevelText.text = (obj.Index+1).ToString();
    }

    private void OnDisable()
    {
        _context.OnLevelUpdated -= OnLevelUpdated;
    }
}
