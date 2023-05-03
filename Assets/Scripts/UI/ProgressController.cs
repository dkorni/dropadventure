using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ProgressController : MonoBehaviour
{
    [Inject] private GameContext _context;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;

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
