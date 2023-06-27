using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAnalyticClientDecorator : IAnalyticClient
{
    private IAnalyticClient _baseClient;

    public BaseAnalyticClientDecorator(IAnalyticClient baseAnalyticClient)
    {
        if(baseAnalyticClient == null)
            throw new ArgumentNullException(nameof(baseAnalyticClient));

        _baseClient = baseAnalyticClient;
    }

    public virtual void ActivateApp()
    {
        _baseClient.ActivateApp();
    }

    public virtual void CompleteLevel(int level)
    {
        _baseClient.CompleteLevel(level);
    }

    public virtual void FailLevel(int level)
    {
        _baseClient.FailLevel(level);
    }

    public virtual void Init()
    {
        _baseClient.Init();
    }

    public virtual void StartLevel(int level)
    {
        _baseClient.StartLevel(level);
    }
}