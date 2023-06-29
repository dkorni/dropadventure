using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TinySauceClientDecorator : BaseAnalyticClientDecorator
{
    public TinySauceClientDecorator(IAnalyticClient baseAnalyticClient) : base(baseAnalyticClient)
    {
    }


    public override void Init()
    {
        base.Init();
        TinySauce.SubscribeOnInitFinishedEvent(null);
    }

    public override void StartLevel(int level)
    {
        base.StartLevel(level);
        TinySauce.OnGameStarted(level);
    }

    public override void CompleteLevel(int level)
    {
        base.CompleteLevel(level);
        TinySauce.OnGameFinished(true, 0, level);
    }

    public override void FailLevel(int level)
    {
        base.FailLevel(level);
        TinySauce.OnGameFinished(false, 0, level);
    }
}
