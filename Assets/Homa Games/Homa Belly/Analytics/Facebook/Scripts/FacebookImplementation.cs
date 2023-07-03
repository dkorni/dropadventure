using System;
using System.Collections.Generic;
using System.IO;
using Facebook.Unity;
using Facebook.Unity.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace HomaGames.HomaBelly
{
    public class FacebookImplementation : IAnalyticsWithInitializationCallback
    {
        private Stack<UnityAction> delayedActionsUntilInitialization = new Stack<UnityAction>();

        public void Initialize()
        {
            Initialize(null);
        }

        public void Initialize(Action onInitialized = null)
        {
            FB.Init(() =>
            {
                FB.ActivateApp();
                if (delayedActionsUntilInitialization != null)
                {
                    HomaGamesLog.Debug($"FB Initialized. Invoking delayed actions");
                    while (delayedActionsUntilInitialization.Count > 0)
                    {
                        UnityAction delayedAction = delayedActionsUntilInitialization.Pop();
                        if (delayedAction != null)
                        {
                            HomaGamesLog.Debug($"Invoking delayed action {delayedAction.Method.Name}");
                            delayedAction.Invoke();
                        }
                    }
                }

                // Invoke initialization callback
                if (onInitialized != null)
                {
                    onInitialized.Invoke();
                }
            });
        }

        public void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                // App Resumed
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    Initialize(null);
                }
            }
        }

        public void SetUserIsAboveRequiredAge(bool consent)
        {
            // NO-OP
        }

        public void SetTermsAndConditionsAcceptance(bool consent)
        {
            // NO-OP
        }

        public void SetAnalyticsTrackingConsentGranted(bool consent)
        {
            if (FB.IsInitialized)
            {
                FB.Mobile.SetAutoLogAppEventsEnabled(consent);
            }
            else
            {
                HomaGamesLog.Debug($"Delaying analytics tracking consent notification...");
                delayedActionsUntilInitialization.Push(() =>
                {
                    SetAnalyticsTrackingConsentGranted(consent);
                });
            }            
        }

        public void SetTailoredAdsConsentGranted(bool consent)
        {
            if (FB.IsInitialized)
            {
                FB.Mobile.SetAdvertiserTrackingEnabled(consent);
                FB.Mobile.SetAdvertiserIDCollectionEnabled(consent);
            }
            else
            {
                HomaGamesLog.Debug($"Delaying tailored ads consent notification...");
                delayedActionsUntilInitialization.Push(() =>
                {
                    SetTailoredAdsConsentGranted(consent);
                });
            }
        }

        public void ValidateIntegration()
        {
#if UNITY_EDITOR
            string facebookSettingsPath = Application.dataPath + "/FacebookSDK/SDK/Resources/FacebookSettings.asset";
            if (File.Exists(facebookSettingsPath))
            {
#endif
                if (FacebookSettings.AppIds != null)
                {
                    if (FacebookSettings.AppIds.Count > 0)
                    {
                        HomaGamesLog.Debug($"[Validate Integration] Facebook successfully integrated");
                    }
                }
                else
                {
                    HomaGamesLog.Warning($"[Validate Integration] Wrong configuration. Facebook APP_IDs not found");
                }
#if UNITY_EDITOR
            }
            else
            {
                HomaGamesLog.Warning($"[Validate Integration] Facebook Settings not found. Please see {facebookSettingsPath}");
            }
#endif
        }

#if UNITY_PURCHASING
        public void TrackInAppPurchaseEvent(UnityEngine.Purchasing.Product product, bool isRestored = false)
        {
            TrackInAppPurchaseEvent(product.definition.id, product.metadata.isoCurrencyCode, Convert.ToDouble(product.metadata.localizedPrice, System.Globalization.CultureInfo.InvariantCulture), product.transactionID, product.receipt, isRestored);
        }
#endif
        
        public void TrackInAppPurchaseEvent(string productId, string currencyCode, double unitPrice, string transactionId = null, string payload = null, bool isRestored = false)
        {
            // NO-OP
        }

        public void TrackResourceEvent(ResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
        {
            // NO-OP
        }

        public void TrackProgressionEvent(ProgressionStatus progressionStatus, string progression01, int score = 0)
        {
            // NO-OP
        }

        public void TrackProgressionEvent(ProgressionStatus progressionStatus, string progression01, string progression02, int score = 0)
        {
            // NO-OP
        }

        public void TrackProgressionEvent(ProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score = 0)
        {
            // NO-OP
        }

        public void TrackErrorEvent(ErrorSeverity severity, string message)
        {
            // NO-OP
        }

        public void TrackDesignEvent(string eventName, float eventValue = 0)
        {
            // NO-OP
        }

        public void TrackAdEvent(AdAction adAction, AdType adType, string adNetwork, string adPlacementId)
        {
            // NO-OP
        }
    }
}

