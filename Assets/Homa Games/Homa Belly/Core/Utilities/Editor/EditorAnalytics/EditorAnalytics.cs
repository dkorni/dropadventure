using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Class to send Unity Editor Analytics to Homa Games servers.
    /// </summary>
    public static class EditorAnalytics
    {
#if !HOMA_BELLY_EDITOR_ANALYTICS_DISABLED

        private const int MAX_CONNECTION_LIMIT = 150;

        private static readonly EditorAnalyticsPoster Poster = new EditorAnalyticsPoster(MAX_CONNECTION_LIMIT);

        static EditorAnalytics()
        {
            ServicePointManager.FindServicePoint(new Uri(EventApiQueryModel.EVENT_API_ENDPOINT))
                .ConnectionLimit = MAX_CONNECTION_LIMIT;
        }
        
        [InitializeOnLoadMethod]
        public static void Initialise()
        {
            PackageReporter packageReporter = new PackageReporter();
            UsageReporter usageReporter = new UsageReporter();
            PerformanceReporter performanceReporter = new PerformanceReporter();
            performanceReporter.OnDataReported += OnDataReported;
            packageReporter.OnDataReported += OnDataReported;
            usageReporter.OnDataReported += OnDataReported;
        }

        private static void OnDataReported(EventApiQueryModel eventApiQueryModel)
        {
            TrackGenericEditorEvent(eventApiQueryModel)
                .ListenForErrors();
        }
#endif
        

        /// <summary>
        /// Track a Unity Editor Event with eventName and other optional values
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventDescription"></param>
        /// <param name="eventStackTrace"></param>
        /// <param name="eventValue"></param>
        /// <param name="eventFps"></param>
        public static void TrackEditorAnalyticsEvent(string eventName, string eventDescription = null, string eventStackTrace = null, float eventValue = 0, float eventFps = 0)
        {
#if !HOMA_BELLY_EDITOR_ANALYTICS_DISABLED
            DoTrackEditorAnalyticsEvent(eventName, eventDescription, eventStackTrace, eventValue, eventFps).ContinueWith(task =>
            {
#if HOMA_BELLY_DEV_ENV
                if (task.IsFaulted)
                {
                    Debug.LogException(task.Exception);
                }
#endif
            }, TaskContinuationOptions.OnlyOnFaulted);
#endif
        }

        [PublicAPI]
        public static async Task TrackGenericEditorEvent(EventApiQueryModel eventModel)
        {
#if !HOMA_BELLY_EDITOR_ANALYTICS_DISABLED
            await DoTrackGenericEditorEvent(eventModel);
#else
            // This is to prevent warning CS1998. We can remove this once TrackGenericEditorEvent is turned from async Task to regular void. 
            await Task.CompletedTask;
#endif
        }

        
#if !HOMA_BELLY_EDITOR_ANALYTICS_DISABLED
        private static async Task DoTrackEditorAnalyticsEvent(string eventName, string eventDescription = null,
            string eventStackTrace = null, float eventValue = 0, float eventFps = 0)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                Debug.LogError($"[Editor Analytics] Tracking empty event name");
                return;
            }

            EditorAnalyticsEventModel eventModel = new EditorAnalyticsEventModel(eventName, eventDescription, eventStackTrace, eventValue, eventFps);

            await DoTrackGenericEditorEvent(eventModel);
        }


        private static async Task DoTrackGenericEditorEvent(EventApiQueryModel eventModel)
        {
            string editorEventUrl = EventApiQueryModel.EVENT_API_ENDPOINT;

#if HOMA_BELLY_DEV_ENV
            Debug.Log($"[Editor Analytics] Tracking: {editorEventUrl}. With body: {eventModel}");

            try
            {
#endif
                EditorAnalyticsResponseModel responseModel = await Poster.Post(editorEventUrl, eventModel);
                
#if HOMA_BELLY_DEV_ENV
                if (responseModel != null)
                {
                    Debug.Log($"[Editor Analytics] Response: {responseModel}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Editor Analytics] Error while sending the event. Reason: {e}");
            }
#endif
        }
#endif
    }
}
