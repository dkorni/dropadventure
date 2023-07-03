using System.Collections;
using System.Collections.Generic;
using HomaGames.HomaBelly;
using UnityEngine;

public class HomoDecorator : BaseAnalyticClientDecorator
{
    public HomoDecorator(IAnalyticClient baseAnalyticClient) : base(baseAnalyticClient)
    {
    }

    public override void StartLevel(int level)
    {
        DefaultAnalytics.LevelStarted(level);
    }

    public override void CompleteLevel(int level)
    {
        DefaultAnalytics.LevelCompleted();
    }

    public override void FailLevel(int level)
    {
        DefaultAnalytics.LevelFailed();
    }
}
