using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// You can start measuring FPS calling CreateFpsTool or just placing this component in a GameObject if you need more control over its settings. 
    /// </summary>
    public class HomaAnalyticsFpsTool
    {
        private const string AVERAGE_FPS_EVENT_NAME = "AverageFPS";
        private const string CRITICAL_FPS_EVENT_NAME = "CriticalFPS";
        private const string APP_TARGET_FPS_EVENT_NAME = "AppTargetFPS";
        
        // Use this to avoid measuring FPS while the app is initializing
        private readonly float m_secondsBeforeStartMeasuring;
        
        // We will send a single event with the average fps after this time.
        private float m_secondsToSendAverageFps;
        
        // Amount of average FPS at which we will count a critical FPS event
        private readonly float m_fpsCriticalThreshold;
        
        private float m_fpsCountForAverage;
        private float m_fpsCountForCritical;
        private float m_lastUpdateCritical;
        private float m_secondsWithCriticalFps;
        private bool m_fpsAnalysisStarted = false;
        private float m_averageFpsTimeCounter = 0f;
        private float m_criticalFpsTimeCounter = 0f;
        private HomaAnalytics m_analytics = null;

        /// <summary>
        /// Call it during play mode. This will create a game object with the needed component to start measuring FPS.
        /// </summary>
        public HomaAnalyticsFpsTool(HomaAnalytics homaAnalytics,
            float delayBeforeFirstMeasure,
            float fpsCriticalThreshold,
            float secondsToSendAverageFps )
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WARNING] You can't create the Fps tool if the application isn't playing");
                return;
            }

            m_analytics = homaAnalytics;
            m_secondsToSendAverageFps = secondsToSendAverageFps;
            m_secondsBeforeStartMeasuring = delayBeforeFirstMeasure;
            m_fpsCriticalThreshold = fpsCriticalThreshold;
        }
        public void ManualUpdate(float unscaledDeltaTime)
        {
            // We let some time so the application initializes before start analyzing the FPS
            m_averageFpsTimeCounter += unscaledDeltaTime;
            if (!m_fpsAnalysisStarted)
            {
                m_fpsAnalysisStarted = m_averageFpsTimeCounter >= m_secondsBeforeStartMeasuring;

                if (m_fpsAnalysisStarted)
                {
                    m_averageFpsTimeCounter = 0;
                }
                else
                {
                    return;
                }
            }
            
            m_criticalFpsTimeCounter += unscaledDeltaTime;
            m_fpsCountForCritical++;
            // Critical FPS are check each second to see if we hit the threshold of low FPS
            if (m_criticalFpsTimeCounter >= 1f)
            {
                if (CheckCriticalFpsThreshold(m_criticalFpsTimeCounter, m_fpsCountForCritical))
                {
                    m_secondsWithCriticalFps++;
                }
                
                m_criticalFpsTimeCounter = 0;
                m_fpsCountForCritical = 0;
            }
            
            m_fpsCountForAverage++;
            if (m_averageFpsTimeCounter >= m_secondsToSendAverageFps)
            {
                SubmitFpsEvents(m_averageFpsTimeCounter,m_fpsCountForAverage,m_secondsWithCriticalFps);
                
                m_averageFpsTimeCounter = 0f;
                m_fpsCountForAverage = 0;
                m_secondsWithCriticalFps = 0;
            }
        }

        private void SubmitFpsEvents(float timeFrame,float fpsCount,float criticalFpsCount)
        {
            // Average FPS
            if (timeFrame > 1.0f)
            {
                float fpsSinceUpdate = fpsCount / timeFrame;

                if (fpsSinceUpdate > 0)
                {
                    if (m_analytics != null)
                    {
                        TrackEvent(AVERAGE_FPS_EVENT_NAME, "AverageFps",Mathf.RoundToInt(fpsSinceUpdate),true);
                    }
                }
            }
            
            // Critical FPS
            if (criticalFpsCount > 0)
            {
                TrackEvent(CRITICAL_FPS_EVENT_NAME,"CriticalFpsCount",criticalFpsCount,false);
            }
        }
        
        private bool CheckCriticalFpsThreshold(float timeFrame,float fpsCount)
        {
            if (timeFrame >= 1.0f)
            {
                float fpsSinceUpdate = fpsCount / timeFrame;

                if (fpsSinceUpdate <= m_fpsCriticalThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        private void TrackEvent(string eventName,string fpsValueKey, float fps,bool trackTargetFps)
        {
            var fpsString = Mathf.RoundToInt(fps).ToString(CultureInfo.InvariantCulture);
            
            var eventValues = new Dictionary<string, object>(1){{fpsValueKey,fpsString}};

            if (trackTargetFps)
            {
                eventValues.Add(APP_TARGET_FPS_EVENT_NAME,
                    Application.targetFrameRate.ToString(CultureInfo.InvariantCulture));
            }

            var fpsEvent = new RuntimeAnalyticsEvent(eventName, 
                HomaAnalytics.PROFILE_CATEGORY, 
                eventValues);

            m_analytics?.TrackEvent(fpsEvent);
        }
    }
}