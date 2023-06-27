using UnityEngine;

public class ConsoleAnalyticClient : IAnalyticClient
{
    public void ActivateApp()
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Analytic client was activated");
    }

    public void CompleteLevel(int level)
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Completed level {level}");
    }

    public void FailLevel(int level)
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Failed level {level}");
    }

    public void Init()
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Analytic client was initialized");
    }

    public void StartLevel(int level)
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Started level {level}");
    }
}