using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Utilities;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class HomaAnalytics : IDisposable
    {
        public const int MAX_EVENT_NAME_LENGTH = 64;
        public const int MAX_EVENT_VALUES_LENGTH = 2048;
        private const int SLOT_MAX_EVENTS = 10;
        private const int SLOT_TIME_TO_AUTO_DISPATCH_MS = 500;
        private const int MAX_SIZE_IN_BYTES = 1000000; // 1MB
        private const int AVERAGE_FPS_SEND_INTERVAL_SECONDS = 30;
        private const int CRITICAL_FPS_THRESHOLD = 15;
        private const int SECONDS_BEFORE_START_MEASURING_FPS = 15;
        /// <summary>
        /// Time to save on disk
        /// </summary>
        private const int SAVE_PLAYTIME_SESSION_INTERVAL = 10;
        
        public static readonly string SLOTS_PATH = $"{Application.persistentDataPath}/EventStorage";
        
        #region Player Pref Keys
        private const string INSTALL_ID_KEY = "ha_install_id";
        private const string SESSION_OPENED_TIME_STAMP_KEY = "ha_session_opened_time_stamp";
        private const string SESSION_ID_KEY = "ha_session_id";
        #endregion
        
        #region Event Names
        private const string SESSION_OPENED_EVENT_NAME = "session_opened";
        private const string APPLICATION_PAUSED_EVENT_NAME = "application_paused";
        private const string NEW_INSTALL_EVENT_NAME = "new_install";
        private const string UNITY_VERSION_VALUE_NAME = "unity_version";
        #endregion
        
        #region Event Categories
        public const string PROGRESSION_CATEGORY = "progression_event";
        public const string REVENUE_CATEGORY = "revenue_event";
        public const string DESIGN_CATEGORY = "design_event";
        public const string AD_CATEGORY = "ad_event";
        public const string SKELETON_CATEGORY = "skeleton_event";
        public const string PROFILE_CATEGORY = "profile_event";
        /// <summary>
        ///  Used for system events like game opened, session opened/closed, etc.
        /// </summary>
        public const string SYSTEM_CATEGORY = "system_event";
        #endregion

        private static readonly Dictionary<string, bool> ValidCategories = new Dictionary<string, bool>()
        {
            {PROGRESSION_CATEGORY,true},
            {REVENUE_CATEGORY,true},
            {DESIGN_CATEGORY,true},
            {AD_CATEGORY,true},
            {SKELETON_CATEGORY,true},
            {SYSTEM_CATEGORY,true},
            {PROFILE_CATEGORY,true},
        };

        private readonly Queue<RuntimeAnalyticsEvent> m_preInitializationEvents = new Queue<RuntimeAnalyticsEvent>();
        private readonly StringBuilder m_stringBuilderForDebugInfo = new StringBuilder();
        private readonly string m_persistentDataPath;
        private PlaytimeTool m_playtimeTool = new PlaytimeTool(SAVE_PLAYTIME_SESSION_INTERVAL);
        private HomaAnalyticsSceneChangeTool m_sceneChangeTool = null;
        private HomaAnalyticsOptions m_analyticsOptions = null;
        private bool m_analyticsEnabled = true;
        private bool m_initialized = false;
        private bool m_firstSessionOpened = false;
        private string m_installId = null;
        private string m_sessionId = null;
        private DateTime m_sessionOpenedTimeStampInSeconds = DateTime.MinValue;
        private EventDispatcher m_eventDispatcher = new EventDispatcher();
        private EventSlotManager m_eventSlotManager = null;
        private HomaAnalyticsFpsTool m_analyticsFpsTool = null;
        private GameObject m_proxyGameObject = null;

        /// <summary>
        /// Get events saved into disk
        /// </summary>
        public int TotalSavedEvents => m_eventSlotManager?.GetTotalSavedEvents() ?? 0;
        
        /// <summary>
        /// Get total events dispatched
        /// </summary>
        public int DispatchedEvents => m_eventSlotManager?.GetTotalDispatchedEvents() ?? 0;
        
        /// <summary>
        /// Count events that are waiting to be sent when HomaAnalytics is initialized.
        /// </summary>
        public int PreInitializationEventsCount => m_preInitializationEvents.Count;
        
        /// <summary>
        /// Events waiting to be sent.
        /// </summary>
        public int PendingEventsCount => m_eventDispatcher.PendingEventsCount;
        
        /// <summary>
        /// Events which response we are waiting to receive from the server. 
        /// </summary>
        public int WaitingEventsResponseCount => m_eventDispatcher.WaitingEventsResponseCount;
        
        public bool Initialized => m_initialized;

        public PlaytimeTool PlaytimeTool => m_playtimeTool;

        /// <summary>
        /// Although you can start to track events in HomaAnalytics after the objects is constructed,
        /// we won't send any events until Initialize() is called.
        /// </summary>
        public HomaAnalytics(HomaAnalyticsOptions homaAnalyticsOptions)
        {
            UnityEngine.Debug.Log($"[HomaAnalytics] Pre-Initialized. Events will be captured and sent later on initialization.");
            
            m_analyticsOptions = homaAnalyticsOptions;
            m_persistentDataPath = Application.persistentDataPath;
            m_sceneChangeTool = new HomaAnalyticsSceneChangeTool(this);
            
            m_proxyGameObject = new GameObject("HomaAnalyticsProxy");
            m_proxyGameObject.SetActive(false);
            m_proxyGameObject.hideFlags = HideFlags.HideAndDontSave;
            var componentProxy = m_proxyGameObject.AddComponent<HomaAnalyticsComponentProxy>();
            componentProxy.HomaAnalyticsInstance = this;
            // Activate proxy GO after setting WeakReference to gather first events
            m_proxyGameObject.SetActive(true);
        }

        /// <summary>
        /// You can override the options used in the constructor. Useful if you have some delayed initialization.
        /// </summary>
        public void Initialize(HomaAnalyticsOptions overrideOptionsUsedInConstructor = null)
        {
            if (m_initialized)
            {
                Debug.LogWarning("[HomaAnalytics] Analytics already initialized.");
                return;
            }
                
            if (overrideOptionsUsedInConstructor != null)
            {
                m_analyticsOptions = overrideOptionsUsedInConstructor;
            }

            m_eventDispatcher.Initialize(m_analyticsOptions);
            
            m_eventSlotManager = new EventSlotManager(m_eventDispatcher,
                SLOTS_PATH,
                SLOT_MAX_EVENTS,
                SLOT_TIME_TO_AUTO_DISPATCH_MS,
                MAX_SIZE_IN_BYTES);

            // Don't toggle analytics. Maybe a developer want to disable them before initializing the service 
            // m_analyticsEnabled = true;

            RuntimeAnalyticsEvent.UserId = Identifiers.HomaGamesId;

            CheckFirstSessionOpened();

            if (m_analyticsOptions.SendFpsEvents)
            {
                m_analyticsFpsTool = new HomaAnalyticsFpsTool(this,
                    SECONDS_BEFORE_START_MEASURING_FPS,
                    CRITICAL_FPS_THRESHOLD,
                    AVERAGE_FPS_SEND_INTERVAL_SECONDS);
            }

            HomaAnalyticsLogListener.SetLogReceivedCallback(OnLogReceived);

            // Important: Initialize before tracking pre initialization events
            m_initialized = true;

            while (m_preInitializationEvents.Count > 0)
            {
                TrackEvent(m_preInitializationEvents.Dequeue());
            }
            
            // We need to manually call application_paused because we are dynamically creating the HomaAnalyticsProxy on Awake
            // so the HomaAnalyticsComponentProxy.OnApplicationPaused isn't called. 
            ApplicationPaused(false);
            
            UnityEngine.Debug.Log($"[HomaAnalytics] Initialized. Install Id: {GetInstallId()} Session Id: {GetSessionId()} NTestingId: {m_analyticsOptions.NTestingId} NTestingOverrideId: {m_analyticsOptions.NTestingOverrideId}");

            if (m_analyticsOptions.VerboseLogs)
            {
                Debug.Log($"[HomaAnalytics] Pending Events: {m_preInitializationEvents.Count}");
                Debug.Log($"[HomaAnalytics] Options: {m_analyticsOptions}");
            }
        }

        private void OnLogReceived(string condition, string stacktrace, LogType type)
        {
            if (! m_analyticsOptions.RecordLogs)
                return;
            
            if (type == LogType.Log
                || condition.StartsWith("[HomaAnalytics Event]"))
                return;

            TrackEvent(new LogAnalyticsEvent(condition, stacktrace, type));
        }
        
        /// <summary>
        /// Return true if the installation is new.
        /// </summary>
        private bool RecoverOrGenerateInstallId()
        {
            m_installId = PlayerPrefs.GetString(INSTALL_ID_KEY, null);

            bool newInstall = false;
            if (string.IsNullOrWhiteSpace(m_installId))
            {
                m_installId = Guid.NewGuid().ToString();
                newInstall = true;
                if (m_analyticsOptions.VerboseLogs)
                {
                    Debug.Log($"[HomaAnalytics] New install id generated: {m_installId}");
                }

                PlayerPrefs.SetString(INSTALL_ID_KEY, m_installId);
                PlayerPrefs.Save();
            }

            return newInstall;
        }

        public void ToggleAnalytics(bool toggle)
        {
            if (m_analyticsOptions.VerboseLogs && m_analyticsEnabled != toggle)
            {
                Debug.Log($"[HomaAnalytics] Toggled: {toggle}");
            }
            
            m_analyticsEnabled = toggle;
            
            if (!toggle)
            {
                m_preInitializationEvents.Clear();
            }
            
            m_eventDispatcher.Toggle(toggle);
        }

        /// <summary>
        /// Check if first session has been opened.
        /// We shouldn't have events until the first open session event is sent.
        /// </summary>
        private void CheckFirstSessionOpened()
        {
            if (m_firstSessionOpened)
            {
                return;
            }
            
            // Order is important to avoid StackOverflowException
            m_firstSessionOpened = true;
            
            bool newInstall = RecoverOrGenerateInstallId();
            CheckNewSession();

            // Send the new install after the session is opened
            if (newInstall)
            {
                var newInstallEvent = new RuntimeAnalyticsEvent(NEW_INSTALL_EVENT_NAME, SYSTEM_CATEGORY);
                TrackEvent(newInstallEvent);
            }
        }

        /// <summary>
        /// We only generate a new session id and we send the open session event
        /// if the time threshold with the application closed is overcome.
        /// Calling OpenSession always will update the session time stamp
        /// </summary>
        private void CheckNewSession()
        {
            // First time we open a session, check if there is one saved in player prefs
            if (string.IsNullOrEmpty(m_sessionId))
            {
                string sessionOpenedTimeStampString = PlayerPrefs.GetString(SESSION_OPENED_TIME_STAMP_KEY, null);
                if (!string.IsNullOrEmpty(sessionOpenedTimeStampString))
                {
                    DateTime.TryParse(sessionOpenedTimeStampString, out m_sessionOpenedTimeStampInSeconds);
                }

                m_sessionId = PlayerPrefs.GetString(SESSION_ID_KEY, null);
            }
            
            // Check if we have to generate a new session id
            var elapsedSeconds = Mathf.Abs((float)(DateTime.UtcNow - m_sessionOpenedTimeStampInSeconds).TotalSeconds);
            if(string.IsNullOrWhiteSpace(m_sessionId) 
               || elapsedSeconds > m_analyticsOptions.SecondsToGenerateNewSessionId)
            {
                m_sessionId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(SESSION_ID_KEY,m_sessionId);
                m_playtimeTool.ResetSessionTime(false);
                // PlayerPrefs.Save() -> is called in UpdateSessionTimeStampAndSavePlayerPrefs(), so we can save writing in disk twice
                
                if (m_analyticsOptions.VerboseLogs)
                {
                    Debug.Log($"[HomaAnalytics] New session id set: {m_sessionId}");
                }
                
                var eventValues = new Dictionary<string, object>();
                eventValues.Add(UNITY_VERSION_VALUE_NAME,Application.unityVersion);
                var openSessionEvent = new RuntimeAnalyticsEvent(SESSION_OPENED_EVENT_NAME, SYSTEM_CATEGORY,eventValues);
                TrackEvent(openSessionEvent);
            }
            
            UpdateSessionTimeStampAndSavePlayerPrefs();
        }

        private void UpdateSessionTimeStampAndSavePlayerPrefs()
        {
            m_sessionOpenedTimeStampInSeconds = DateTime.UtcNow;
            PlayerPrefs.SetString(SESSION_OPENED_TIME_STAMP_KEY, m_sessionOpenedTimeStampInSeconds.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
        }

        #region Unity Lifecycle
        
        private void OnApplicationQuit()
        {
            if (!m_initialized)
            {
                return;
            }

            #if UNITY_EDITOR
            // I want to stop sending events when exiting play mode.
            Dispose();
            #endif
        }
        
        /// <summary>
        /// This callback is 'public' just to use it on Unit Tests
        /// </summary>
        /// <param name="paused"></param>
        public void ApplicationPaused(bool paused)
        {
            if (!m_initialized)
            {
                return;
            }
            
            var values = new Dictionary<string, object>() {{"paused", paused}};
            var applicationPausedEvent = new RuntimeAnalyticsEvent(APPLICATION_PAUSED_EVENT_NAME, SYSTEM_CATEGORY,values);
            applicationPausedEvent.FastDispatch = true;
            
            TrackEvent(applicationPausedEvent);
            
            if (paused)
            {
                // SaveOnDiskNow is false because UpdateSessionTimeStampAndSavePlayerPrefs will trigger PlayerPref.Save()
                m_playtimeTool.SaveTime(false);
                UpdateSessionTimeStampAndSavePlayerPrefs();
                m_eventSlotManager.ForceWriteAllSlotsInDisk();
            }
            else
            {
                CheckNewSession();
            }
        }
        
        #endregion

        /// <summary>
        /// Called to cancel ongoing events 
        /// </summary>
        public void Dispose()
        {
            m_eventDispatcher?.Dispose();
            m_eventDispatcher = null;
            m_playtimeTool = null;
            m_analyticsFpsTool = null;
            m_sceneChangeTool = null;
            m_analyticsOptions = null;
            GameObject.Destroy(m_proxyGameObject);
        }

        #region Homa Analytics Methods

        public void TrackEvent(RuntimeAnalyticsEvent runtimeAnalyticsEvent)
        {
            HomaAnalyticsTaskUtils.Consume(DoTrackEventAsync(runtimeAnalyticsEvent));
        }
        
        private async Task DoTrackEventAsync(RuntimeAnalyticsEvent runtimeAnalyticsEvent)
        {
            if (runtimeAnalyticsEvent == null)
            {
                Debug.LogWarning("Can't track null events.");
                return;
            }
            
            // Set the total play time when the event is captured, not when we send the event.
            // Note that we can capture events prior to HA initialization
            runtimeAnalyticsEvent.SetTotalPlayTimeOnlyOnce(m_playtimeTool.TotalPlaytime);
            
            if (!m_initialized)
            {
                CheckFirstSessionOpened();
                StorePreInitializationEvent(runtimeAnalyticsEvent);
                return;
            }
            
            if (!m_analyticsEnabled)
            {
                return;
            }

            // TokenIdentifier & ManifestVersionId are static field.
            // I want to set them everytime I send an event 
            ApiQueryModel.TokenIdentifier = m_analyticsOptions.TokenIdentifier;
            ApiQueryModel.ManifestVersionId = m_analyticsOptions.ManifestVersionId;
            
            // Inject install id and session id
            runtimeAnalyticsEvent.InstallId = GetInstallId();
            runtimeAnalyticsEvent.SessionId = GetSessionId();
            runtimeAnalyticsEvent.NTestingId = m_analyticsOptions.NTestingId;
            runtimeAnalyticsEvent.NTestingOverrideId = m_analyticsOptions.NTestingOverrideId;

            if (m_analyticsOptions.VerboseLogs)
            {
                Debug.Log($"[HomaAnalytics Event] Tracking: {runtimeAnalyticsEvent.EventName}: {runtimeAnalyticsEvent}");
            }
            
            if (!m_analyticsOptions.EventValidation 
                || IsEventValid(runtimeAnalyticsEvent))
            {
                await StoreEventBeforeDispatching(runtimeAnalyticsEvent);
            }
        }

        private async Task StoreEventBeforeDispatching(RuntimeAnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent == null)
            {
                return;
            }
            
            Task<string> serializeTask; 
            if (analyticsEvent.FastDispatch)
            {
                serializeTask = Task.Run(() => AsyncSerializeEvent(analyticsEvent));
                serializeTask.Wait();
            }
            else
            {
                serializeTask = AsyncSerializeEvent(analyticsEvent);
                await serializeTask;
            }
            
            var bodyAsJsonString = serializeTask.Result;

            if (bodyAsJsonString == null)
            {
                Debug.LogWarning($"[HomaAnalytics] EventValue is null");
                return;
            }
            
            if (bodyAsJsonString.Length > MAX_EVENT_VALUES_LENGTH)
            {
                Debug.LogWarning($"[HomaAnalytics] EventValues can't have more than {MAX_EVENT_VALUES_LENGTH} characters. Current: {bodyAsJsonString.Length}");
                return;
            }

            var pendingEvent = new PendingEvent(analyticsEvent.EventName,
                analyticsEvent.EventId,
                bodyAsJsonString);

            pendingEvent.FastDispatch = analyticsEvent.FastDispatch;

            if (analyticsEvent.FastDispatch)
            {
                // Developer Notes:
                // In order to try to save as much time as possible, we will execute the event storage at the same time that we dispatch the events.
                // This may be a good global change to speedup the whole analytics systems, but I'm not 100% confident of it.
                // For example, I think it may increase the amount of duplicated events in the server and the server isn't ready for that yet.
                m_eventDispatcher.DispatchPendingEvent(pendingEvent);
                m_eventSlotManager.StoreEvent(pendingEvent);
            }
            else
            {
                m_eventSlotManager.StoreEvent(pendingEvent);
            }
        }

        private async Task<string> AsyncSerializeEvent(RuntimeAnalyticsEvent analyticsEvent)
        {
            string bodyAsJsonString = null;
            var dictionary = analyticsEvent.ToDictionary();

            if (m_analyticsOptions.RecordAllEventsInCsv)
            {
                HomaAnalyticsEventRecorderDebugTool.RecordInCsv(m_persistentDataPath,
                    GetSessionId(),
                    analyticsEvent.EventId,
                    in dictionary);
            }
            
            await Task.Run(delegate
            {
                try
                {
                    bodyAsJsonString = Json.Serialize(dictionary);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ERROR] EventDispatcher: {e}");
                }
            });

            return bodyAsJsonString;
        }

        private bool IsEventValid(RuntimeAnalyticsEvent runtimeAnalyticsEvent)
        {
            if (runtimeAnalyticsEvent.InstallId != GetInstallId())
            {
                Debug.LogWarning($"[HomaAnalytics] Event install id: {runtimeAnalyticsEvent.InstallId} is different to the current install id: {GetInstallId()}. This only can happen if the game data was partially deleted.");
            }
            
            if (!ValidCategories.ContainsKey(runtimeAnalyticsEvent.EventCategory))
            {
                Debug.LogWarning($"[HomaAnalytics] Can't recognize category: {runtimeAnalyticsEvent.EventCategory} in event: {runtimeAnalyticsEvent.EventName}.");
            }
            
            if (string.IsNullOrWhiteSpace(runtimeAnalyticsEvent.EventName))
            {
                Debug.LogError("[HomaAnalytics] EventName must be set.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(runtimeAnalyticsEvent.InstallId))
            {
                Debug.LogError($"[HomaAnalytics] InstallId can't be null or empty in event: {runtimeAnalyticsEvent.EventName}");
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(runtimeAnalyticsEvent.SessionId))
            {
                Debug.LogError($"[HomaAnalytics] SessionId can't be null or empty in event: {runtimeAnalyticsEvent.EventName}");
                return false;
            }

            if (runtimeAnalyticsEvent.EventName.Length > MAX_EVENT_NAME_LENGTH)
            {
                Debug.LogError($"[HomaAnalytics] EventName can't have a length major than {MAX_EVENT_NAME_LENGTH} characters.");
                return false;
            }

            return true;
        }

        public string GetInstallId()
        {
            return m_installId;
        }

        public string GetSessionId()
        {
            return m_sessionId;
        }

        private void StorePreInitializationEvent(RuntimeAnalyticsEvent runtimeAnalyticsEvent)
        {
            Debug.Log($"[HomaAnalytics] Event {runtimeAnalyticsEvent.EventName} will be sent after HomaAnalytics was initialized.");
            // TODO: Should I store this in the file system as well as regular events?
            // Note: Storing the event ant not a copy can create issues if we decide to optimize
            // the memory allocation on the event creation
            m_preInitializationEvents.Enqueue(runtimeAnalyticsEvent);
        }

        /// <summary>
        /// This will clear all persistent data related with HA.
        /// </summary>
        public static void ClearPersistentData()
        {
            PlayerPrefs.DeleteKey(INSTALL_ID_KEY);
            PlayerPrefs.DeleteKey(SESSION_ID_KEY);
            PlayerPrefs.DeleteKey(SESSION_OPENED_TIME_STAMP_KEY);
            PlaytimeTool.ClearSavedData();

            if (System.IO.Directory.Exists(SLOTS_PATH))
            {
                System.IO.Directory.Delete(SLOTS_PATH,true);
            }
            
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Get useful debug info in formated text
        /// </summary>
        public string GetDebugInfo()
        {
            m_stringBuilderForDebugInfo.Clear();
            m_stringBuilderForDebugInfo.AppendLine($"Pre-Initialized: True, Initialized: {Initialized}");
            m_stringBuilderForDebugInfo.AppendLine($"ASID: {Identifiers.Asid}");
            m_stringBuilderForDebugInfo.AppendLine($"GAID: {Identifiers.Gaid}");
            m_stringBuilderForDebugInfo.AppendLine($"IDFA: {Identifiers.Idfa}");
            m_stringBuilderForDebugInfo.AppendLine($"IDFV: {Identifiers.Idfv}");
            m_stringBuilderForDebugInfo.AppendLine($"DeviceId: {Identifiers.DeviceId}");
            m_stringBuilderForDebugInfo.AppendLine($"HomaGamesId: {Identifiers.HomaGamesId}");
            
            // Group and count states
            if (m_eventSlotManager != null)
            {
                var activeSlotsCount = 0;
                var slotSummary = m_eventSlotManager.GetSlotsSummary();
                var statesCount = new Dictionary<string, int>();
                for (var index = 0; index < slotSummary.Count; index++)
                {
                    if (index == 0)
                    {
                        // First slot is always the active slot
                        m_stringBuilderForDebugInfo.AppendLine("Active Slot: "+slotSummary[index]);
                    }
                    
                    var summary = slotSummary[index];
                    if (statesCount.ContainsKey(summary.CurrentState.ToString()))
                    {
                        statesCount[summary.CurrentState.ToString()]++;
                    }
                    else
                    {
                        statesCount.Add(summary.CurrentState.ToString(), 1);
                    }

                    if (summary.CurrentState != EventStorageSlotStates.AllEventsDispatched)
                    {
                        activeSlotsCount++;
                    }
                }

                m_stringBuilderForDebugInfo.AppendLine($"Working Slots: {activeSlotsCount}");

                foreach (var stateCount in statesCount)
                {
                    m_stringBuilderForDebugInfo.AppendLine($"SlotsState: {stateCount.Key}={stateCount.Value}");
                }

                m_stringBuilderForDebugInfo.AppendLine($"Slots Index: {m_eventSlotManager.GetUsedSlotCount()}");
                m_stringBuilderForDebugInfo.AppendLine($"Saved Events: {m_eventSlotManager.GetTotalSavedEvents()}");
                m_stringBuilderForDebugInfo.AppendLine($"Dispatched Events: {m_eventSlotManager.GetTotalDispatchedEvents()}");
                m_stringBuilderForDebugInfo.AppendLine($"{m_eventSlotManager.GetTotalSizeInBytes()}bytes");
            }
            
            m_stringBuilderForDebugInfo.AppendLine($"Session Playtime: "+m_playtimeTool.SessionPlayTime+" Total Playtime: "+m_playtimeTool.TotalPlaytime);

            return m_stringBuilderForDebugInfo.ToString();
        }
        
        #endregion
        
        #region Homa Analytics Component Proxy
        
        /// <summary>
        /// Proxy GameObject to capture Unity lifecycle callbacks
        /// and keep track of them.
        ///
        /// This lifecycle callbacks track events within a `Task` so we make
        /// sure the event is fired to the engines when the app is being unfocused/paused
        /// </summary>
        public class HomaAnalyticsComponentProxy : MonoBehaviour
        {
            public HomaAnalytics HomaAnalyticsInstance { get; set; }

            private DateTime m_dateTimeWhenPaused = DateTime.MaxValue;
            private float m_pausedTimeInSeconds = 0f;
            private void OnApplicationQuit()
            {
                HomaAnalyticsInstance?.OnApplicationQuit();
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                if (pauseStatus)
                {
                    m_dateTimeWhenPaused = DateTime.UtcNow;
                }
                else
                {
                    m_pausedTimeInSeconds = (float)(DateTime.UtcNow - m_dateTimeWhenPaused).TotalSeconds;
                    HomaAnalyticsInstance.m_sceneChangeTool.AddPausedTime(m_pausedTimeInSeconds);
                }
                
                HomaAnalyticsInstance?.ApplicationPaused(pauseStatus);
            }

            private void Update()
            {
                var unscaledTime = Time.unscaledDeltaTime;
                
                // UnscaledDeltaTime is affected when the app goes to background.
                // We need to compensate this time difference so we only take into account the playtime.
                if (m_pausedTimeInSeconds > 0f)
                {
                    unscaledTime -= m_pausedTimeInSeconds;
                    m_pausedTimeInSeconds = 0f;
                }

                // We agreed with the data team to start counting the playtime
                // after the first game scene was loaded.
                // Doing this we will have an homogeneous reference point for the play time
                if (HomaAnalyticsInstance.m_sceneChangeTool.FirstGameSceneAlreadyLoaded)
                {
                    HomaAnalyticsInstance.m_playtimeTool.CountTime(unscaledTime);
                }

                if (HomaAnalyticsInstance.Initialized)
                {
                    HomaAnalyticsInstance.m_analyticsFpsTool?.ManualUpdate(unscaledTime);
                }
            }
        }
        
        #endregion
    }
}