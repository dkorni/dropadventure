using UnityEngine;
using System;

namespace YsoCorp {
    namespace GameUtils {
        [DefaultExecutionOrder(-20)]
        public class YCManager : BaseManager {

            public static YCManager instance;

            #region Static Field
            public static string VERSION = "0.1.0";

            private static string PLAYER_LAUNCH_COUNT = "YC_PLAYER_LAUNCH_COUNT";
            private static string PLAYER_GAME_COUNT = "YC_PLAYER_GAME_COUNT";
            private static string PLAYER_FIRST_DAY = "YC_PLAYER_FIRST_DAY";
            private static string PLAYER_RETENTION_D = "YC_PLAYER_RETENTION_D";

            public bool HasGameStarted { get; private set; } = false;
            #endregion

            public YCConfig ycConfig;

            public ABTestingManager abTestingManager { get; set; }
            public AdsManager adsManager { get; set; }
            public AnalyticsManager analyticsManager { get; set; }
            public FBManager fbManager { get; set; }
            public GdprManager gdprManager { get; set; }
            public I18nManager i18nManager { get; set; }
            public InAppManager inAppManager { get; set; }
            public MmpManager mmpManager { get; set; }
            public NoInternetManager noInternetManager { get; set; }
            public RequestManager requestManager { get; set; }
            public SettingManager settingManager { get; set; }
            public YCDataManager dataManager { get; set; }

            protected override void Awake() {
                if (instance != null) {
                    DestroyImmediate(this.gameObject);
                } else {
                    instance = this;
                    base.Awake();
                    DontDestroyOnLoad(this.gameObject);
                    this.dataManager = this.GetComponentInChildren<YCDataManager>();
                    this.abTestingManager = this.GetComponentInChildren<ABTestingManager>();
                    this.adsManager = this.GetComponentInChildren<AdsManager>();
                    this.analyticsManager = this.GetComponentInChildren<AnalyticsManager>();
                    this.fbManager = this.GetComponentInChildren<FBManager>();
                    this.gdprManager = this.GetComponentInChildren<GdprManager>();
                    this.i18nManager = this.GetComponentInChildren<I18nManager>();
                    this.inAppManager = this.GetComponentInChildren<InAppManager>();
                    this.mmpManager = this.GetComponentInChildren<MmpManager>();
                    this.noInternetManager = this.GetComponentInChildren<NoInternetManager>();
                    this.requestManager = this.GetComponentInChildren<RequestManager>();
                    this.settingManager = this.GetComponentInChildren<SettingManager>(true);
                    PlayerPrefs.SetInt(PLAYER_LAUNCH_COUNT, PlayerPrefs.GetInt(PLAYER_LAUNCH_COUNT, 0) + 1);
                    this.CheckMmpLaunchCountEvent();
                    this.CheckMmpRetentionEvent();
                    if (Application.installMode != ApplicationInstallMode.Editor && Application.installMode != ApplicationInstallMode.DeveloperBuild) {
                        this.ycConfig.activeLogs = 0;
                        this.ycConfig.ABDebugLog = false;
                        this.ycConfig.InappDebug = false;
                    }
                    Debug.Log("[GameUtils] YCManager : Initialize !");
                }
            }

            public bool IsFirstTimeAppLaunched() {
                return PlayerPrefs.GetInt(PLAYER_LAUNCH_COUNT, 0) == 1;
            }

            public int GetPlayerLaunchCount() {
                return PlayerPrefs.GetInt(PLAYER_LAUNCH_COUNT, 0);
            }

            private void CheckMmpLaunchCountEvent() {
                if (this.GetPlayerLaunchCount() == 20) {
                    this.mmpManager.OnMmpsInit.AddListener(() => this.mmpManager.SendEvent("20_game_launch"));
                }
            }

            private void CheckMmpRetentionEvent() {
                int day = 1;
                if (this.IsTodayEventSent(day) == false && this.IsPlayerRetentionDay(day)) {
                    this.mmpManager.OnMmpsInit.AddListener(() => this.mmpManager.SendEvent("retention_d" + day));
                    PlayerPrefs.SetInt(PLAYER_RETENTION_D + day, 1);
                }
            }

            private bool IsPlayerRetentionDay(int day) {
                DateTime firstDay = DateTime.Parse(this.GetPlayerFirstDay());
                DateTime testDay = firstDay.AddDays(day);
                return DateTime.UtcNow.ToString("d") == testDay.ToString("d");
            }

            private string GetPlayerFirstDay() {
                string date = PlayerPrefs.GetString(PLAYER_FIRST_DAY, "NoDate");
                if (date == "NoDate") {
                    date = DateTime.UtcNow.ToString("d");
                    PlayerPrefs.SetString(PLAYER_FIRST_DAY, date);
                }
                return date;
            }

            private bool IsTodayEventSent(int day) {
                return PlayerPrefs.GetInt(PLAYER_RETENTION_D + day, 0) == 1;
            }

            public void OnGameStarted(int level) {
                if (this.HasGameStarted == false) {
                    this.HasGameStarted = true;
                    this.analyticsManager.OnGameStarted(level);
                    PlayerPrefs.SetInt(PLAYER_GAME_COUNT, PlayerPrefs.GetInt(PLAYER_GAME_COUNT, 0) + 1);
                }
            }

            public void OnGameFinished(bool levelComplete, float score = 0f) {
                if (this.HasGameStarted == true) {
                    this.HasGameStarted = false;
                    this.analyticsManager.OnGameFinished(levelComplete, score);
                    this.dataManager.IncrementLevelPlayed(levelComplete);
                }
            }

        }
    }
}
