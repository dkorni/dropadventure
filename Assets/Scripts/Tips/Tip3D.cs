using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Tip3D : MonoBehaviour
{
    public GameStates ExpectedState;

    [Inject]
    private GameContext gameContext;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        gameContext.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameStates states)
    {
        if(states == ExpectedState)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        gameContext.OnStateChanged -= OnStateChanged;
    }
}
