using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Flushfluids.SceneManagement
{
    public class SceneManager : MonoBehaviour
    {
        private int _loadedLevel = -1;

        [SerializeField]
        private float _timeoutBeforeNextLevel = 3;

        [Inject]
        private GameContext _context;

        void Start()
        {
            if (PlayerPrefs.HasKey("level"))
                _context.CurrentLevel = PlayerPrefs.GetInt("level");
            else
            {
                _context.CurrentLevel = 1;
                PlayerPrefs.SetInt("level", _context.CurrentLevel);
            }

            LoadLevel(_context.CurrentLevel);
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

        private AsyncOperation LoadLevel(int level)
        {
            if (_loadedLevel != -1)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_loadedLevel);
            }

            _loadedLevel = level;
            var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
            op.completed += op => SceneManager_completed(op, () => _loadedLevel = level);
            return op;
        }

        private IEnumerator LoadNextLevelCoroutine()
        {
            var nextLevel = _context.CurrentLevel + 1;
            if (nextLevel == UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                nextLevel = 1;

            yield return new WaitForSeconds(_timeoutBeforeNextLevel);

            LoadLevel(nextLevel).completed += op => SceneManager_completed(op, () =>
            {
                _context.CurrentLevel = nextLevel;
                PlayerPrefs.SetInt("level", _context.CurrentLevel);
            });
        }

        private void SceneManager_completed(AsyncOperation obj, Action action)
        {
            if (obj.isDone)
            {
                action();
            }
        }

        private void OnReload()
        {
            LoadLevel(_context.CurrentLevel);
        }

        private void OnDisable()
        {
            _context.OnStateChanged -= OnStateChanged;
            _context.OnReload -= OnReload;
        }
    } 
}