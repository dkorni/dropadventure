using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneManager : MonoBehaviour
{
    private int _currentLevel;
    private int _loadedLevel = -1;
    
    [SerializeField]
    private float _timeoutBeforeNextLevel = 3;

    [Inject]
    private GameContext _context;

    void Start()
    {
        if (PlayerPrefs.HasKey("level"))
            _currentLevel = PlayerPrefs.GetInt("level");
        else
        {
            _currentLevel = 1;
            PlayerPrefs.SetInt("level", _currentLevel);
        }
        
        LoadLevel(_currentLevel);
        _context.OnStateChanged += OnStateChanged;
        _context.OnReload += OnReload;

    }

    private void OnStateChanged(GameStates state)
    {
        switch (state)
        {
            case GameStates.Win:
                StartCoroutine(LoadNextLevelCoroutine());
                break;
        }
    }

    private void LoadLevel(int level)
    {
        if (_loadedLevel != -1)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_loadedLevel);
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        _loadedLevel = level;
    }

    private IEnumerator LoadNextLevelCoroutine()
    {
        _currentLevel++;
        
        if (_currentLevel == UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            _currentLevel = 1;
        
        PlayerPrefs.SetInt("level", _currentLevel);
        yield return new WaitForSeconds(_timeoutBeforeNextLevel);
        LoadLevel(_currentLevel);
    }

    private void OnReload()
    {
        LoadLevel(_currentLevel);
    }

    private void OnDisable()
    {
        _context.OnStateChanged -= OnStateChanged;
        _context.OnReload -= OnReload;
    }
}
