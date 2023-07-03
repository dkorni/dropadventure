using System;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class PendingEvent
    {
        [CanBeNull]
        public string Id { get; }
        [CanBeNull]
        public string EventName { get; }
        [CanBeNull]
        public string Json { get; }
        
        /// <summary>
        /// How many times we have tried to send this event.
        /// </summary>
        public int PostRetries { get; set; }
        
        /// <summary>
        /// Special property for events that have to be dispatched as fast as possible.
        /// - We process the event synchronously instead of asynchronously
        /// - We store at the same time we send the event (this is still an experimental feature)
        /// </summary>
        public bool FastDispatch { get; set; }
        
        /// <summary>
        /// True when the event has already been sent.
        /// </summary>
        public bool IsDispatched { get; private set; }


        private CancellationTokenSource m_tokenSource;
        private DateTime m_timeStamp = DateTime.MinValue;
        private Action<PendingEvent> m_dispatchedCallback = null;
        
        public PendingEvent([CanBeNull] string eventName, [CanBeNull] string eventId, [CanBeNull] string eventJson)
        {
            EventName = eventName;
            Id = eventId;
            Json = eventJson;
        }

        public void CancelAndDispose()
        {
            if (m_tokenSource == null)
            {
                return;
            }
            
            m_tokenSource.Cancel();
            Dispose();
        }

        public float GetElapsedTime()
        {
            return (float)(DateTime.UtcNow - m_timeStamp).TotalSeconds;
        }

        public CancellationToken PrepareToSend(int requestTimeoutSeconds)
        {
            m_tokenSource = new CancellationTokenSource(requestTimeoutSeconds * 1000);
            m_timeStamp = DateTime.UtcNow;
            return m_tokenSource.Token;
        }

        public void Dispose()
        {
            if (m_tokenSource != null)
            {
                m_tokenSource.Dispose();
                m_tokenSource = null;
            }
        }

        /// <summary>
        /// Subscribe to know when the event has been dispatched. True if the dispatch was successful, false otherwise.
        /// </summary>
        public void NotifyOnDispatch(Action<PendingEvent> dispatched)
        {
            if (m_dispatchedCallback != null)
            {
                HomaGamesLog.Error("[ERROR] Subscribing twice to a pending event isn't supported. "+ EventName);
                return;
            }
            
            m_dispatchedCallback = dispatched;
        }

        public void Dispatched()
        {
            if (IsDispatched)
            {
                Debug.LogWarning($"$[HomaAnalytics] Events can be dispatched only once. {ToString()}");
                return;
            }
                
            IsDispatched = true;
            m_dispatchedCallback?.Invoke(this);
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Json) ? Json : $"{EventName} {Id}";
        }
    }
}