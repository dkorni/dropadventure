using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnalyticClient 
{
    void Init();

    void ActivateApp();

    void StartLevel(int level);

    void CompleteLevel(int level);

    void FailLevel(int level);
}