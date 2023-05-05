#if ZIBRA_LIQUID_PRO_VERSION
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using com.zibra.liquid.Editor.SDFObjects;

namespace com.zibra.liquid.Editor
{
    internal class ZibraLiquidProOnbording : EditorWindow
    {
        private const string REGISTRATION_GUID = "39f6c1ca9194bcb4d896f42e94b38b88";
        private const string AUTHORIZATION_GUID = "c70f412c74090c8488e3e2d7ec4a6101";

        private string CurrentUXMLGUID = REGISTRATION_GUID;
        private bool TriedToVerify = false;

        private TextField AuthKeyInputField;
        private Label AuthMessage;
        private Button ActivateButton;
        private Button GetStartedButton;

        private ZibraServerAuthenticationManager.Status LatestStatus =
    ZibraServerAuthenticationManager.Status.NotInitialized;

        private ZibraServerAuthenticationManager ServerAuthenticationInstance;

        public static GUIContent WindowTitle => new GUIContent("Zibra Liquids Onbording Screen");

        internal static void ShowWindowDelayed()
        {
            ShowWindow();
            EditorApplication.update -= ShowWindowDelayed;
        }

        [InitializeOnLoadMethod]
        internal static void InitializeOnLoad()
        {
            // Don't automatically open any windows in batch mode
            if (Application.isBatchMode)
            {
                return;
            }

            if (EditorPrefs.GetBool("ZibraLiquidsProOnbordingShown", true))
            {
                EditorPrefs.SetBool("ZibraLiquidsProOnbordingShown", false);
                EditorApplication.update += ShowWindowDelayed;
            }
        }

        [MenuItem("Zibra AI/Zibra Liquids/Open Onbording", false, 1)]
        private static void ShowWindow()
        {
            ZibraLiquidProOnbording window = (ZibraLiquidProOnbording)GetWindow(typeof(ZibraLiquidProOnbording));
            window.titleContent = WindowTitle;
            window.Show();
        }

        private static void CloseWindow()
        {
            ZibraLiquidProOnbording window = (ZibraLiquidProOnbording)GetWindow(typeof(ZibraLiquidProOnbording));
            window.Close();
        }

        private void OnEnable()
        {
            var root = rootVisualElement;
            root.Clear();

            int width = 456;
            int height = 442;

            minSize = maxSize = new Vector2(width, height);

            var uxmlAssetPath = AssetDatabase.GUIDToAssetPath(CurrentUXMLGUID);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlAssetPath);
            visualTree.CloneTree(root);

            if (CurrentUXMLGUID == REGISTRATION_GUID)
            {
                root.Q<Button>("GetKey").clicked += GetKeyClick;
                root.Q<Button>("HaveKey").clicked += HaveKeyClick;
                root.Q<Button>("PrivacyPolicy").clicked += PrivacyPolicyClick;
                root.Q<Button>("TermsAndConditions").clicked += TermsAndConditionsClick;
            }
            if (CurrentUXMLGUID == AUTHORIZATION_GUID)
            {
                ServerAuthenticationInstance = ZibraServerAuthenticationManager.GetInstance();
                root.Q<Button>("BackToRegistration").clicked += BackToRegistrationClick;

                AuthMessage = root.Q<Label>("AuthorizationMessage");
                ActivateButton = root.Q<Button>("Activate");
                GetStartedButton = root.Q<Button>("GetStart");
                AuthKeyInputField = root.Q<TextField>("ActivationField");

                AuthKeyInputField.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Return)
                    {
                        ActivateClick();
                        evt.StopPropagation();
                    }
                 });
                ActivateButton.clicked += ActivateClick;
                GetStartedButton.clicked += GetStartedClick;


                if (ServerAuthenticationInstance.IsLicenseVerified())
                {
                    AuthMessageStyle(AuthMessage, Color.green);
                    AuthMessage.text = "Your license validation is Success";
                    AuthKeyInputField.style.display = DisplayStyle.None;
                    ActivateButton.style.display = DisplayStyle.None;
                    GetStartedButton.style.display = DisplayStyle.Flex;
                }
            }
        }

        private void GetKeyClick()
        {
            Application.OpenURL("https://license.zibra.ai/api/stripeTrial?source=plugin");
            CurrentUXMLGUID = AUTHORIZATION_GUID;
            OnEnable();
        }

        private void HaveKeyClick()
        {
            CurrentUXMLGUID = AUTHORIZATION_GUID;
            OnEnable();
        }

        private void PrivacyPolicyClick()
        {
            Application.OpenURL("https://zibra.ai/privacy-policy/");
        }

        private void TermsAndConditionsClick()
        {
            Application.OpenURL("https://zibra.ai/terms-of-service/");
        }

        private void ActivateClick()
        {
            string key = AuthKeyInputField.text.Trim();

            if (key.Length == ZibraServerAuthenticationManager.KEY_LENGTH)
            {
                TriedToVerify = true;
                ServerAuthenticationInstance.RegisterKey(key);
                AuthKeyInputField.style.display = DisplayStyle.None;
                ActivateButton.style.display = DisplayStyle.None;
                AuthMessage.text = ServerAuthenticationInstance.GetErrorMessage();
            }
            else
            {
                AuthMessageStyle(AuthMessage, Color.red);
                AuthMessage.text = ("Zibra Liquid Key Error, Incorrect key format.");
            }
        }

        private void GetStartedClick()
        {
            ZibraWelcomeWindow.OpenWelcomeWindow();
            CloseWindow();
        }

        private void BackToRegistrationClick()
        {
            CurrentUXMLGUID = REGISTRATION_GUID;
            OnEnable();
        }

        private void AuthMessageStyle(Label authMessage, Color color)
        {
            authMessage.style.marginTop = 58;
            authMessage.style.color = new StyleColor(color);
            authMessage.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        private void Update()
        {
            if (TriedToVerify)
            {
                if (ServerAuthenticationInstance.GetStatus() == LatestStatus)
                    return;

                LatestStatus = ZibraServerAuthenticationManager.GetInstance().GetStatus();

                if (ServerAuthenticationInstance.IsLicenseVerified())
                {
                    AuthMessageStyle(AuthMessage, Color.green);
                    AuthMessage.text = "Your license validation is Success";
                    GetStartedButton.style.display = DisplayStyle.Flex;
                }
                else
                {
                    if (ServerAuthenticationInstance.GetStatus() != ZibraServerAuthenticationManager.Status.NotRegistered && 
                        ServerAuthenticationInstance.GetStatus() != ZibraServerAuthenticationManager.Status.KeyValidationInProgress)
                    {
                        AuthMessageStyle(AuthMessage, Color.red);
                        AuthMessage.text = ServerAuthenticationInstance.GetErrorMessage();
                        AuthKeyInputField.style.display = DisplayStyle.Flex;
                        ActivateButton.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        if (ServerAuthenticationInstance.GetErrorMessage() != "")
                        {
                            AuthMessageStyle(AuthMessage, Color.white);
                            AuthMessage.text = ServerAuthenticationInstance.GetErrorMessage();
                        }
                    }
                }
            }
        }
    }
}
#endif
