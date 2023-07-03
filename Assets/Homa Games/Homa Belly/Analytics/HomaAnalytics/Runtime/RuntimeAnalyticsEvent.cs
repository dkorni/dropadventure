using System.Collections.Generic;
using System.Globalization;
using HomaGames.HomaBelly;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class RuntimeAnalyticsEvent : EventApiQueryModel
    {
        /// <summary>
        /// Apple Identifier For Advertisers
        /// </summary>
        private const string IDFA = "idfa";
        
        /// <summary>
        /// Apple Identifier For Vendors. This id should be persistent between installations and apps
        /// as long as the user has a application installed from the same company
        /// </summary>
        private const string IDFV = "idfv";
        
        /// <summary>
        /// Google Advertisers Identifier
        /// Similar to IDFA
        /// </summary>
        private const string GAID = "gaid";
        
        /// <summary>
        /// Android App Set Id.
        /// Similar to IDFV
        /// </summary>
        private const string ASID = "asid";
        
        /// <summary>
        /// User Id
        /// </summary>
        private const string USER_ID = "user_id";
        
        /// <summary>
        /// Debug build?
        /// </summary>
        private const string DEBUG = "debug";
        
        /// <summary>
        /// Total time the app was active since the application installation 
        /// </summary>
        private const string TOTAL_PLAY_TIME = "tpt";

        public static string UserId { get; set; }
        
        /// <summary>
        /// Special property for events that have to be dispatched as fast as possible.
        /// - We process the event synchronously instead of asynchronously
        /// - We store at the same time we send the event (this is still an experimental feature)
        /// </summary>
        public bool FastDispatch { get; set; }

        /// <summary>
        /// Measures the total play time when this event was created
        /// </summary>
        private float m_totalPlayTime = -1;

        public RuntimeAnalyticsEvent(string eventName, 
            string eventCategory,
            Dictionary<string,object> values = null)
        {
            EventCategory = eventCategory;
            EventName = eventName;
            EventValues = values;
            
            if (EventValues == null)
            {
                EventValues = new Dictionary<string, object>(1);
            }

            bool isDebug = false;
            #if HOMA_DEVELOPMENT
                isDebug = true;
            #endif
            EventValues.Add(DEBUG,isDebug.ToString());
        }

        public override Dictionary<string, object> ToDictionary()
        {
            var dictionary = base.ToDictionary();

            // Because this identifiers are very large, 
            // we can save some bandwidth sending empty strings in unused IDs
            #if UNITY_EDITOR
                dictionary.Add(IDFA,Identifiers.Idfa);
                dictionary.Add(IDFV,Identifiers.Idfv);
                dictionary.Add(GAID,Identifiers.Gaid);
                dictionary.Add(ASID,Identifiers.Asid);
            #elif UNITY_IOS
                dictionary.Add(IDFA,Identifiers.Idfa);
                dictionary.Add(IDFV,Identifiers.Idfv);
                dictionary.Add(GAID,"");
                dictionary.Add(ASID,"");
            #elif UNITY_ANDROID 
                dictionary.Add(IDFA,"");
                dictionary.Add(IDFV,"");
                dictionary.Add(GAID,Identifiers.Gaid);
                dictionary.Add(ASID,Identifiers.Asid);
            #endif

            dictionary.Add(USER_ID,UserId);
            dictionary.Add(TOTAL_PLAY_TIME,m_totalPlayTime.ToString("0.00",CultureInfo.InvariantCulture));

            return dictionary;
        }

        /// <summary>
        /// It will set the total play time if not set.
        /// Useful when handling pending events.
        /// </summary>
        public void SetTotalPlayTimeOnlyOnce(float totalPlayTime)
        {
            if (m_totalPlayTime < 0)
            {
                m_totalPlayTime = totalPlayTime;
            }
        }
    }
}
