#if !HOMA_BELLY_EDITOR_ANALYTICS_DISABLED
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Utilities;
using UnityEditor;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class EditorAnalyticsPoster
    {
        private const int EVENT_THRESHOLD_BLOCK_UI = 70;
        private const int EVENT_MAX_RETRY_COUNT = 4;
        private const int EVENT_POST_TIMEOUT_SECONDS = 5;
        private const int MAX_NETWORK_ERROR_COUNT = 50;

        private readonly Queue<EventToPost> QueueToPost = new Queue<EventToPost>();
        private readonly HttpClient HttpClient = GetHttpClient();
        private readonly UiBlocker UiBlockerInstance = new UiBlocker();

        private readonly int MaxPostingCount;
        
        private int CurrentlyPostingCount;
        private bool AwaitingEventPost;

        private int NetworkErrorCount;

        public EditorAnalyticsPoster(int maxPostingCount)
        {
            MaxPostingCount = maxPostingCount;
        }
        
        public async Task<EditorAnalyticsResponseModel> Post(string uri, EventApiQueryModel eventModel)
        {
            EventToPost eventToPost = new EventToPost(uri, eventModel);
            QueueToPost.Enqueue(eventToPost);
            ScheduleEventPosting();
            
            UiBlockerInstance.OnEventAdded();
            
            return await eventToPost.AssociatedTask;
        }

        private void ScheduleEventPosting()
        {
            if (! AwaitingEventPost)
            {
                AwaitingEventPost = true;
                ThreadUtils.RunAndForgetOnMainThread(() =>
                {
                    AwaitingEventPost = false;

                    PostEventsInQueueIfPossible();
                });
            }
        }

        private void PostEventsInQueueIfPossible()
        {
            while (CurrentlyPostingCount < MaxPostingCount && QueueToPost.Count != 0)
            {
                Post(QueueToPost.Dequeue()).ListenForErrors();
            }
        }


        private async Task Post(EventToPost eventToPost)
        {
            EditorAnalyticsResponseModel responseModel = null;
            Exception postException = null;
            
            try
            {

                CurrentlyPostingCount++;
                
                try
                {
                    responseModel = await DoPost(eventToPost.Uri, eventToPost.Event.ToDictionary());
                }
                catch (Exception e)
                {
                    postException = e;
                }

                CurrentlyPostingCount--;

                PostEventsInQueueIfPossible();

                if (postException == null)
                {
                    eventToPost.AssociatedTask.OnEventPosted(responseModel);
                }
                else
                {
                    OnPostError();
                    
                    if (eventToPost.RetryLeft > 0)
                    {
                        eventToPost.RetryLeft--;
                        QueueToPost.Enqueue(eventToPost);
                    }
                    else
                    {
                        eventToPost.AssociatedTask.OnError(postException);
                    }
                }
            }
            finally
            {
                if (postException == null)
                    UiBlockerInstance.OnEventSent();
                
                PostEventsInQueueIfPossible();
            }
        }

        private void OnPostError()
        {
            NetworkErrorCount++;

            if (NetworkErrorCount >= MAX_NETWORK_ERROR_COUNT)
            {
                // No internet, we flush the queue to prevent stalling
                QueueToPost.Clear();
                NetworkErrorCount = 0;
                
                HttpClient.CancelPendingRequests();
                
                UiBlockerInstance.Clear();
            }
        }

        private async Task<EditorAnalyticsResponseModel> DoPost(string uri, Dictionary<string, object> body)
        {
            string bodyAsJsonString = await Task.Run(() => Json.Serialize(body));
            StringContent data = new StringContent(bodyAsJsonString, Encoding.UTF8, "application/json");
            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            CancellationTokenSource timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(EVENT_POST_TIMEOUT_SECONDS));
            HttpResponseMessage response = await HttpClient.PostAsync(uri, data, timeoutSource.Token);
        
            if (response.IsSuccessStatusCode)
            {
                return new EditorAnalyticsResponseModel();
            }
            else
            {
                string errorString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(errorString))
                {
                    JsonObject responseObject = await Task.Run(() => Json.DeserializeObject(errorString));
                    // Detect any error
                    if (responseObject.TryGetString("status", out _) &&
                        responseObject.TryGetString("message", out var message))
                    {
                        throw new Exception(message);
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            return default;
        }

        private static HttpClient GetHttpClient()
        {
#if CHARLES_PROXY
        var httpClientHandler = new HttpClientHandler()
        {
            Proxy = new System.Net.WebProxy("http://localhost:8888", false),
            UseProxy = true
        };

        return new HttpClient(httpClientHandler);
#else
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(EVENT_POST_TIMEOUT_SECONDS)
            };
#endif
        }

        private class EventToPost
        {
            public EventApiQueryModel Event { get; }
            public string Uri { get; }
            public EventPostTask AssociatedTask { get; } = new EventPostTask();
            public int RetryLeft = EVENT_MAX_RETRY_COUNT;

            public EventToPost(string uri, EventApiQueryModel @event)
            {
                Event = @event;
                Uri = uri;
            }
        }

        private class EventPostTask : TaskBase<EditorAnalyticsResponseModel>
        {
            public void OnEventPosted(EditorAnalyticsResponseModel response)
            {
                OnTaskCompleted(response);
            }
        
            public void OnError(Exception exception)
            {
                OnTaskFailed(exception);
            }
        }

        private class UiBlocker
        {
            private int EventToPostCount;
            
            private int EventTotalCount;
            private bool CurrentlyBlockingUi;
        
            public void OnEventAdded()
            {
                EventToPostCount++;

                if (!CurrentlyBlockingUi && EventToPostCount > EVENT_THRESHOLD_BLOCK_UI)
                    Block();
                else if (CurrentlyBlockingUi)
                    EventTotalCount++;
                
                if (CurrentlyBlockingUi)
                    DisplayOrUpdateProgressBar();
            }
            
            public void OnEventSent()
            {
                EventToPostCount--;
                if (EventToPostCount < 0) EventToPostCount = 0;

                if (CurrentlyBlockingUi)
                {
                    if (EventToPostCount == 0)
                    {
                        Unblock();
                        EventTotalCount = 0;
                    }
                    else
                    {
                        DisplayOrUpdateProgressBar();
                    }
                }
            }

            private void Unblock()
            {
                EditorApplication.UnlockReloadAssemblies();
                EditorUtility.ClearProgressBar();
                CurrentlyBlockingUi = false;
            }


            private void Block()
            {
                EventTotalCount = EventToPostCount;
                CurrentlyBlockingUi = true;
                EditorApplication.LockReloadAssemblies();
            }

            private void DisplayOrUpdateProgressBar()
            {
                float progress = 1 - EventToPostCount / (float) EventTotalCount;
                EditorUtility.DisplayProgressBar("Analytics", "Sending analytics events...", progress);
            }

            public void Clear()
            {
                EventToPostCount = 0;
                EventTotalCount = 0;
                Unblock();
            }
        }
    }
}

#endif
