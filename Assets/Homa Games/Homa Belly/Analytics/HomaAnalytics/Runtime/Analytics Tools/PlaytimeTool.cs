using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Tool to keep track session playtime and the total playtime.
    /// </summary>
    public class PlaytimeTool
    {
        private const string TOTAL_PLAYTIME_VALUE_NAME = "tpt";

        private const string SESSION_TIME_KEY = "HomaAnalyticsSessionTime";
        private const string TOTAL_TIME_KEY = "HomaAnalyticsTotalPlaytime";
        
        private readonly float m_saveOnDiskInterval = 0f;
        private float m_sessionPlayTime = 0f;
        private float m_totalPlayTime = 0f;
        private float m_saveTimer = 0f;

        /// <summary>
        /// Length in seconds of the active session. This will be reset when a new session id is generated
        /// </summary>
        public float SessionPlayTime => m_sessionPlayTime;
        
        /// <summary>
        /// Total playtime since the application was opened.
        /// </summary>
        public float TotalPlaytime => m_totalPlayTime + m_sessionPlayTime;
        
        public PlaytimeTool(float saveOnDiskIntervalSeconds)
        {
            // We need to save from time to time to improve precision in cases like the app crashing
            m_saveOnDiskInterval = saveOnDiskIntervalSeconds;

            m_sessionPlayTime = PlayerPrefs.GetFloat(SESSION_TIME_KEY, 0f);
            m_totalPlayTime = PlayerPrefs.GetFloat(TOTAL_TIME_KEY, 0f);
        }

        public void CountTime(float unscaledDeltaTime)
        {
            m_sessionPlayTime += unscaledDeltaTime;
            m_saveTimer += unscaledDeltaTime;
            
            if (m_saveTimer >= m_saveOnDiskInterval)
            {
                SaveTime(true);
            }
        }

        public void ResetSessionTime(bool saveOnDiskNow)
        {
            m_totalPlayTime += m_sessionPlayTime;
            m_sessionPlayTime = 0;
            
            SaveTime(saveOnDiskNow);
        }

        public void SaveTime(bool savePlayerPrefs)
        {
            PlayerPrefs.SetFloat(SESSION_TIME_KEY,m_sessionPlayTime);
            PlayerPrefs.SetFloat(TOTAL_TIME_KEY,m_totalPlayTime);

            if (savePlayerPrefs)
            {
                PlayerPrefs.Save();
            }
        }

        public static void ClearSavedData()
        {
            PlayerPrefs.DeleteKey(TOTAL_TIME_KEY);
            PlayerPrefs.DeleteKey(SESSION_TIME_KEY);
        }
    }
}