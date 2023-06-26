using UnityEngine;

public class ConsoleAnalyticClient : IAnalyticClient
{
    public void CompleteLevel(int level)
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Completed level {level}");
    }

    public void FailLevel(int level)
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Failed level {level}");
    }

    public void StartLevel(int level)
    {
        Debug.Log($"{nameof(ConsoleAnalyticClient)}: Started level {level}");
    }
}