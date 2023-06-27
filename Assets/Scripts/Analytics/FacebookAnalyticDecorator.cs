using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacebookAnalyticDecorator : BaseAnalyticClientDecorator
{
    public FacebookAnalyticDecorator(IAnalyticClient baseAnalyticClient) : base(baseAnalyticClient)
    {
    }

    public override void Init()
    {
        base.Init();
        FB.Init(FBInitCallback);
    }

    public override void ActivateApp()
    {
        base.ActivateApp();
        FBInitCallback();
    }

    private void FBInitCallback()
    {
        if(FB.IsInitialized)
        {
            FB.ActivateApp();
        }
    }
}