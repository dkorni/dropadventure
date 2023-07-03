using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HomaGames.HomaBelly.Installer
{
    public class LogComponent
    {
        public event Action<bool> OnToggled;
        
        // Templates
        private VisualTreeAsset logElement;
        private VisualTreeAsset debugLogElement;
        private VisualTreeAsset warningLogElement;
        
        // Images
        private Texture2D warningLogImage;
        private Texture2D debugLogImage;
        private Texture2D errorLogImage;
        
        // Cache
        private VisualElement logsWindowInstance;
        private VisualElement logsText;

        private VisualElement rootPlacement;
        private VisualElement replacedElement;

        private Queue<KeyValuePair<HomaBellyEditorLog.Level, string>> logsToAdd = new Queue<KeyValuePair<HomaBellyEditorLog.Level, string>>();
        
        private bool isOpen;
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;
                OnToggled?.Invoke(value);
                if (isOpen)
                {
                    replacedElement.RemoveFromHierarchy();
                    rootPlacement.Add(logsWindowInstance);
                }
                else
                {
                    logsWindowInstance.RemoveFromHierarchy();
                    rootPlacement.Add(replacedElement);
                }
            }
        }

        
        public LogComponent(VisualElement root,VisualElement replaced)
        {
            rootPlacement = root;
            replacedElement = replaced;
            var logWindowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{HomaBellyEditorWindow.RESOURCES_PATH}/Logs.uxml");
            logsWindowInstance = logWindowTemplate.Instantiate();
            logsText = logsWindowInstance.Query<VisualElement>("LogsList").First();
            logElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{HomaBellyEditorWindow.RESOURCES_PATH}/LogElement.uxml");
            warningLogImage = AssetDatabase.LoadAssetAtPath<Texture2D>($"{HomaBellyEditorWindow.RESOURCES_PATH}/warning_log.png");
            debugLogImage = AssetDatabase.LoadAssetAtPath<Texture2D>($"{HomaBellyEditorWindow.RESOURCES_PATH}/debug_log.png");
            errorLogImage = AssetDatabase.LoadAssetAtPath<Texture2D>($"{HomaBellyEditorWindow.RESOURCES_PATH}/error_log.png");
            logsWindowInstance.Query<Button>("LogsBackButton").First().RegisterCallback<ClickEvent>(BackButtonClicked);
            logsWindowInstance.Query<Button>("CopyLogsButton").First().RegisterCallback<ClickEvent>(OnCopyLogsClicked);
            HomaBellyEditorLog.OnLogsAdded += OnNewLogs;
        }

        public void UpdateLogsOnMainThread()
        {
            while (logsToAdd.Count>0)
            {
                var obj = logsToAdd.Dequeue();
                var log = logElement.Instantiate();
                log.Query<Label>().First().text = obj.Value;
                var icon = log.Query<VisualElement>("Icon").First();
                switch (obj.Key)
                {
                    case HomaBellyEditorLog.Level.DEBUG:
                        icon.style.backgroundImage = new StyleBackground(debugLogImage);
                        break;
                    case HomaBellyEditorLog.Level.WARNING:
                        icon.style.backgroundImage = new StyleBackground(warningLogImage);
                        break;
                    case HomaBellyEditorLog.Level.ERROR:
                        icon.style.backgroundImage = new StyleBackground(errorLogImage);
                        break;
                }
                logsText.Add(log);
            }
        }
        
        private void OnNewLogs(KeyValuePair<HomaBellyEditorLog.Level, string> obj)
        {
            logsToAdd.Enqueue(obj);
        }

        private void BackButtonClicked(ClickEvent evt)
        {
            IsOpen = false;
        }

        private void OnCopyLogsClicked(ClickEvent evt)
        {
            GUIUtility.systemCopyBuffer = HomaBellyEditorLog.GetCopiableLogTrace();
        }
    }
}
