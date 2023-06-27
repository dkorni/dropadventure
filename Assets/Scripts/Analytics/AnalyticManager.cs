using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnalyticManager : MonoBehaviour
{
    [Inject]
    private IAnalyticClient analyticClient;

    // Start is called before the first frame update
    void Start()
    {
        analyticClient.Init();
    }

    public void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            analyticClient.ActivateApp();
        }
    }
}
