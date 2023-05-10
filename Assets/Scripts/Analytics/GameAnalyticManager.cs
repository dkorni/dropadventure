using GameAnalyticsSDK;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
  internal class GameAnalyticManager :  MonoBehaviour, IGameAnalyticsATTListener
  {
        void Start()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
            }
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
        }

        public void StartLevel(int levelId)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"Level_{levelId}");
            YsoCorp.GameUtils.YCManager.instance.OnGameStarted(levelId);
        }

        public void CompleteLevel(int levelId, int health, int seconds)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level_{levelId}");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level_{levelId}", "Health", health);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level_{levelId}", "Time", seconds);

            var hasWon = true;
            YsoCorp.GameUtils.YCManager.instance.OnGameFinished(hasWon);
        }

        public void FailLevel(int levelId, int health, int seconds)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"Level_{levelId}");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"Level_{levelId}", "Health", health);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"Level_{levelId}", "Time", seconds);

            var hasWon = false;
            YsoCorp.GameUtils.YCManager.instance.OnGameFinished(hasWon);
        }

        public void AddCoins(int coins, int levelId)
        {
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coins", coins, "Reward", $"Level_{levelId}");
        }
    }
}
