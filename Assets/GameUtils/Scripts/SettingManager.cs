using UnityEngine;
using UnityEngine.UI;

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-11)]
        public class SettingManager : BaseManager {

            private static Color COLOR_ON = Color.white;
            private static Color COLOR_OFF = new Color(0.7f, 0.7f, 0.7f, 1f);

            public Button bClose;
            public Button bRestorePurchase;
            public Button bDataPrivacy;
            public GameObject panelBts;
            public GameObject content;
            public Button bLang;
            public Text tVersion;
            public Button bVersion;

            private float _versionLastClick;

            protected override void Awake() {
                base.Awake();
                this.bRestorePurchase.gameObject.SetActive(this.ycManager.ycConfig.HasInApps());
#if UNITY_ANDROID && !UNITY_EDITOR
                this.bRestorePurchase.gameObject.SetActive(false);
#endif
                this.bLang.gameObject.SetActive(false);
                this.tVersion.text = "v" + Application.version + "  sdk" + YCConfig.VERSION;
                if (this.ycManager.abTestingManager.GetPlayerSample() != "") {
                    this.tVersion.text += " (" + this.ycManager.abTestingManager.GetPlayerSample() + ")";
                }
                this.bVersion.onClick.AddListener(() => {
                    if (Time.time - this._versionLastClick < 0.3f) {
                        MaxSdk.ShowMediationDebugger();
                    }
                    this._versionLastClick = Time.time;
                });
            }

            private void Start() {
                if (this.ycManager.i18nManager.i18NResourcesManager.i18ns.Count > 1) {
                    this.bLang.gameObject.SetActive(true);
                    this.bLang.image.sprite = this.ycManager.i18nManager.GetCurrentStrite();
                    this.bLang.onClick.AddListener(() => {
                        this.ycManager.i18nManager.NextLanguages();
                        this.bLang.image.sprite = this.ycManager.i18nManager.GetCurrentStrite();
                    });
                }
                this.panelBts.gameObject.SetActive(
                    this.bLang.gameObject.activeSelf
                );
                this.bClose.onClick.AddListener(() => {
                    this.gameObject.SetActive(false);
                });
                this.bRestorePurchase.onClick.AddListener(() => {
                    this.ycManager.inAppManager.RestorePurchases();
                });
                this.bDataPrivacy.onClick.AddListener(() => {
                    this.ycManager.adsManager.DisplayGDPR();
                });
            }

            public void Show() {
                this.gameObject.SetActive(true);
            }

        }
    }
}