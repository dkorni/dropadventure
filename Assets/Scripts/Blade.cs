using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Blade : MonoBehaviour
{
    private bool isCutting;
    public float MaxDistance = 1000;

    [Inject] GameContext gameContext;
    private AudioSource audioSource;

    private void Start()
    {
        gameContext.OnStartCut += StartCutting;
        gameContext.OnStopCut += StopCutting;
        gameContext.OnUpdateCut += UpdateCutting;
        audioSource = GetComponent<AudioSource>();
    }

    private void StartCutting()
    {
        isCutting = true;
    }

    private void StopCutting()
    {
        isCutting = false;
    }

    private void UpdateCutting(Vector3 position)
    {
        var ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out var hit, MaxDistance))
        {
            if (hit.transform.CompareTag("Chain"))
            {
                var joint = hit.transform.GetComponent<HingeJoint>();
                if (joint != null)
                {
                    Destroy(joint);
                    audioSource.Play();
                }
            }
        }
    }

    public void OnDisable()
    {
        gameContext.OnStartCut -= StartCutting;
        gameContext.OnStopCut -= StopCutting;
        gameContext.OnUpdateCut -= UpdateCutting;
    }
}
