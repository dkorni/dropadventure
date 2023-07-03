using System.Collections.Generic;
using HomaGames.HomaBelly.DataPrivacy;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Inspect scene changes so we can automatically log events and benchmark times
    /// </summary>
    public class HomaAnalyticsSceneChangeTool
    {
        private const string FIRST_GAME_SCENE_EVENT_NAME = "first_scene_loaded";
        private const string GAME_SCENE_EVENT_NAME = "scene_loaded";
        private const string LOAD_TIME_KEY = "app_load_time";
        private const string SCENE_NAME_KEY = "scene_name";
        private const string SCENE_INDEX_KEY = "scene_index";
        
        private readonly HomaAnalytics m_homaAnalytics = null;
        
        private bool m_firstGameSceneAlreadyLoaded = false;

        /// <summary>
        /// True after the first game scene has been loaded.
        /// It will be false in GDPR/Data Privacy scenes.
        /// </summary>
        public bool FirstGameSceneAlreadyLoaded => m_firstGameSceneAlreadyLoaded;

        private float m_totalPausedTime = 0f;

        public HomaAnalyticsSceneChangeTool(HomaAnalytics homaAnalytics)
        {
            m_homaAnalytics = homaAnalytics;

            // Take care of situations in which we directly starts in a scene
            bool skipSceneLoadedEventOnStartup = false;
            var activeScene = SceneManager.GetActiveScene();
            if (!DataPrivacyUtils.IsSceneDataPrivacyScene(activeScene.buildIndex))
            {
                m_firstGameSceneAlreadyLoaded = true;
                skipSceneLoadedEventOnStartup = true;
                TrackSceneLoad(activeScene,FIRST_GAME_SCENE_EVENT_NAME);
            }

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                // It could happen sceneLoaded to be called on the first scene
                // depending on when HA is pre initialized.
                // To avoid bugs due to initialization changes, just control this scenario
                // avoiding to register the first scene twice
                if (skipSceneLoadedEventOnStartup)
                {
                    skipSceneLoadedEventOnStartup = false;
                    return;
                }
                
                if (mode == LoadSceneMode.Additive)
                {
                    return;
                }

                if (DataPrivacyUtils.IsSceneDataPrivacyScene(scene.buildIndex))
                {
                    return;
                }

                if (!m_firstGameSceneAlreadyLoaded)
                {
                    m_firstGameSceneAlreadyLoaded = true;
                    TrackSceneLoad(scene,FIRST_GAME_SCENE_EVENT_NAME);
                }
                else
                {
                    TrackSceneLoad(scene,GAME_SCENE_EVENT_NAME);
                }
            };
        }

        /// <summary>
        /// Track how much time it took to get until this scene since the game started
        /// </summary>
        private void TrackSceneLoad(Scene scene,string eventName)
        {
            m_homaAnalytics.TrackEvent(new RuntimeAnalyticsEvent(eventName,
                HomaAnalytics.SYSTEM_CATEGORY,
                new Dictionary<string, object>
            {
                {SCENE_NAME_KEY,scene.name},
                {SCENE_INDEX_KEY,scene.buildIndex},
                {LOAD_TIME_KEY, GetActiveTimeSinceStartup()},
            }));
        }

        /// <summary>
        /// Return the active time since startup discarding the paused time. 
        /// </summary>
        private float GetActiveTimeSinceStartup()
        {
            return Time.realtimeSinceStartup - m_totalPausedTime;
        }

        /// <summary>
        /// Add paused time.
        /// </summary>
        public void AddPausedTime(float pausedTimeInSeconds)
        {
            if (pausedTimeInSeconds > 0)
            {
                m_totalPausedTime += pausedTimeInSeconds;
            }
        }
    }
}