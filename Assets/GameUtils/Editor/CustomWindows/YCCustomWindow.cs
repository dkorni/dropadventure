using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCCustomWindow : EditorWindow {

            public enum WindowPosition {
                UpperLeft,
                UpperCenter,
                UpperRight,
                MiddleLeft,
                MiddleCenter,
                MiddleRight,
                LowerLeft,
                LowerCenter,
                LowerRight
            }

            private Rect GetMainWindowRect() {
#if UNITY_2020_1_OR_NEWER
                return EditorGUIUtility.GetMainWindowPosition();
#else
                return new Rect(0, 0, 1920, 1080);
#endif
            }

            public void SetMinSize(float width, float height) {
                width = Mathf.Min(width, this.GetMainWindowRect().width * 0.9f);
                height = Mathf.Min(height, this.GetMainWindowRect().height * 0.9f);
                this.minSize = new Vector2(width, height);
            }

            public void SetMaxSize(float width, float height) {
                width = Mathf.Min(width, this.GetMainWindowRect().width * 0.9f);
                height = Mathf.Min(height, this.GetMainWindowRect().height * 0.9f);
                this.maxSize = new Vector2(width, height);
            }

            public void SetSize(float width, float height) {
                this.SetMinSize(width, height);
                this.SetMaxSize(width, height);
            }

            public void SetPosition(int posX, int posY) {
                Rect r = this.position;
                r.x = posX;
                r.y = posY;
                this.position = r;
            }

            public void SetPosition(WindowPosition windowPosition) {
                Rect r = this.position;
                float centerX = this.GetMainWindowRect().center.x - this.position.width / 2;
                float centerY = this.GetMainWindowRect().center.y - this.position.height / 2;
                float rightX = this.GetMainWindowRect().width - this.position.width;
                float downY = this.GetMainWindowRect().height - this.position.height;
                switch (windowPosition) {
                    case WindowPosition.UpperLeft:
                        r.x = 0;
                        r.y = 0;
                        break;

                    case WindowPosition.UpperCenter:
                        r.x = centerX;
                        r.y = 0;
                        break;

                    case WindowPosition.UpperRight:
                        r.x = rightX;
                        r.y = 0;
                        break;

                    case WindowPosition.MiddleLeft:
                        r.x = 0;
                        r.y = centerY;
                        break;

                    case WindowPosition.MiddleCenter:
                        r.x = centerX;
                        r.y = centerY;
                        break;

                    case WindowPosition.MiddleRight:
                        r.x = rightX;
                        r.y = centerY;
                        break;

                    case WindowPosition.LowerLeft:
                        r.x = 0;
                        r.y = downY;
                        break;

                    case WindowPosition.LowerCenter:
                        r.x = centerX;
                        r.y = downY;
                        break;

                    case WindowPosition.LowerRight:
                        r.x = rightX;
                        r.y = downY;
                        break;
                }
                this.position = r;
            }

            protected void AddEmptyLine(int amount = 0) {
                string spaces = "";
                for (int i = 0; i < amount; i++) {
                    spaces += "\n";
                }
                GUILayout.Label(spaces);
            }

            protected void AddLabel(string text, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = alignment;
                GUILayout.Label(text, style);
            }

            protected void AddLabel(string text, Color color, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = alignment;
                style.normal.textColor = color;
                GUILayout.Label(text, style);
            }

            protected void AddSelectableLabel(string text, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = alignment;
                GUILayout.BeginVertical("box");
                EditorGUILayout.SelectableLabel(text, style);
                GUILayout.EndVertical();
            }

            protected void AddSelectableLabel(string text, Color color, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = alignment;
                style.normal.textColor = color;
                GUILayout.BeginVertical("box");
                EditorGUILayout.SelectableLabel(text, style);
                GUILayout.EndVertical();
            }

            protected string AddTextInputField(string title, string defaultValue) {
                GUILayout.BeginHorizontal();
                this.AddLabel(title);
                string result = EditorGUILayout.TextField(defaultValue);
                GUILayout.EndHorizontal();
                return result;
            }

            protected bool AddToggle(string text = "", bool defaultValue = false, TextAnchor alignment = TextAnchor.MiddleLeft) {
                GUIStyle style = new GUIStyle(GUI.skin.toggle);
                style.alignment = alignment;
                return GUILayout.Toggle(defaultValue, text, style);
            }

            protected void AddButton(string text, Action action) {
                if (GUILayout.Button(text)) {
                    action();
                }
            }

            protected void AddButtonValidation(string text, Action action) {
                if (GUILayout.Button(text) || (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))) {
                    action();
                }
            }

            protected void AddButtonClose(string text = "") {
                if (text == "") {
                    text = "Close";
                }
                if (GUILayout.Button(text)) {
                    this.Close();
                }
            }

            protected void AddTable(int column, int row, Action[] actions) {
                if (actions.Length <= column * row) {
                    GUILayout.BeginVertical("box");
                    for (int i = 0; i < row; i++) {
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < column; j++) {
                            GUILayout.BeginHorizontal("box");
                            actions[i * column + j]();
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                } else {
                    Debug.LogError("Actions Array too large for the set size");
                }
            }

            protected void AddCancelOk(string cancel, string ok, Action cancelAction, Action okAction) {
                GUILayout.BeginHorizontal();
                this.AddButton(cancel, cancelAction);
                this.AddButtonValidation(ok, okAction);
                GUILayout.EndHorizontal();
            }

            protected T AddObjectField<T>(string text, T obj, bool allowSceneObjects) where T : UnityEngine.Object {
                return EditorGUILayout.ObjectField(text, obj, typeof(T), allowSceneObjects) as T;
            }
        }
    }
}
