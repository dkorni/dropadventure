using System;
using UnityEngine;
using Zenject;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private GameContext _gameContext;
    [SerializeField] private Animator animator;

    void Start()
    {
        _gameContext.OnStateChanged += OnStateChanged;
        gameObject.SetActive(false);
    }

    private void OnStateChanged(GameStates e)
    {
        if(e == GameStates.GameOver)
        {
            gameObject.SetActive(true);
            var hash = animator.GetCurrentAnimatorStateInfo(0).nameHash;
            animator.Play(hash);
        }
    }

    public void Retry()
    {
        _gameContext.Reload();
        gameObject.SetActive(false);
    }
}
