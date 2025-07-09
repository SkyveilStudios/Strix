using System;
using System.Collections.Generic;
using Strix.Runtime.Hub.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Strix.Editor.Hub {
    public static class HubAttributesTab {
        private class AttributeData {
            public string Label;
            public string Description;
            public string CodeExample;
            public Func<Object> EnsureInstance;
        }

        private static readonly Dictionary<AttributeType, AttributeData> Attributes = new() {
            {
                AttributeType.ImagePreview, new AttributeData {
                    Label = "ImagePreview",
                    Description = "Allows you to embed a custom image above a field.\n\n" +
                                  "<b>Parameters:</b>\n" +
                                  "• <b>path</b>: Path to the image in Assets.\n" +
                                  "• <b>width</b>: Image width.\n" +
                                  "• <b>fullWidth</b>: Stretch image to inspector width.\n" +
                                  "• <b>alignment</b>: Image alignment: Left, Center, Right.",
                    CodeExample = "[ImagePreview(\"Assets/Strix/Banners/StrixBanner.jpg\", 400f)]\n[SerializeField] private GameObject obj;",
                    EnsureInstance = () => EnsureInstance(ref _imagePreviewInstance, typeof(HubImagePreview))
                }
            }, {
                AttributeType.Required, new AttributeData {
                    Label = "Required",
                    Description = "Displays a warning when a GameObject is missing or null.",
                    CodeExample = "[Required]\n[SerializeField] public GameObject obj;",
                    EnsureInstance = () => EnsureInstance(ref _requiredInstance, typeof(HubRequired))
                }
            }, {
                AttributeType.ReadOnly, new AttributeData {
                    Label = "ReadOnly",
                    Description = "Displays fields grayed out and uneditable in the inspector.",
                    CodeExample = "[ReadOnly]\n[SerializeField] public float speed;",
                    EnsureInstance = () => EnsureInstance(ref _readOnlyInstance, typeof(HubReadOnly))
                }
            }
        };

        private enum AttributeType {
            ImagePreview,
            Required,
            ReadOnly
        }

        private static HubImagePreview _imagePreviewInstance;
        private static HubRequired _requiredInstance;
        private static HubReadOnly _readOnlyInstance;
        private static UnityEditor.Editor _previewEditor;
        private static Vector2 _scroll;
        private static AttributeType _selectedAttribute = AttributeType.ImagePreview;

        public static void DrawAttributesTab() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
            DrawAttributeList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            HubTabUtils.DrawHeader("Description");
            DrawDescription();
            HubTabUtils.DrawSeparator();

            HubTabUtils.DrawHeader("Preview");
            DrawPreview();
            HubTabUtils.DrawSeparator();

            HubTabUtils.DrawHeader("Code Example");
            DrawCodeBlock();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawAttributeList() {
            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
            HubTabUtils.DrawSidePanelBackground(200);
            _scroll = GUILayout.BeginScrollView(_scroll, GUIStyle.none, GUI.skin.verticalScrollbar);

            var labelStyle = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 32,
                padding = new RectOffset(0, 0, -15, 0)
            };
            EditorGUILayout.LabelField("Attributes", labelStyle, GUILayout.ExpandWidth(true));

            var lineRect = GUILayoutUtility.GetRect(1, 2, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(lineRect, new Color(0.7f, 0.7f, 0.7f));

            foreach (var kvp in Attributes) {
                DrawAttributeButton(kvp.Value.Label, kvp.Key);
            }

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private static void DrawAttributeButton(string label, AttributeType type) {
            var isSelected = _selectedAttribute == type;

            var style = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 30,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(10, 10, 0, 0),
                border = new RectOffset(0, 0, 0, 0),
                normal = {
                    background = HubTabUtils.MakeTex(1, 1, isSelected ? new Color(0.30f, 0.48f, 0.55f) : new Color(0.15f, 0.15f, 0.15f)),
                    textColor = isSelected ? Color.white : Color.gray
                },
                hover = {
                    background = HubTabUtils.MakeTex(1, 1, isSelected ? new Color(0.30f, 0.48f, 0.55f) : new Color(0.25f, 0.25f, 0.25f)),
                    textColor = Color.white
                },
                active = {
                    background = HubTabUtils.MakeTex(1, 1, new Color(0.30f, 0.48f, 0.55f)),
                    textColor = Color.white
                }
            };

            if (GUILayout.Button(label, style, GUILayout.ExpandWidth(true), GUILayout.Width(200))) {
                _selectedAttribute = type;
            }
        }

        private static void DrawDescription() {
            var data = Attributes[_selectedAttribute];
            var style = new GUIStyle(EditorStyles.label) {
                wordWrap = true,
                richText = true
            };

            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(data.Description, style);
            EditorGUILayout.EndVertical();
        }

        private static void DrawPreview() {
            var instance = Attributes[_selectedAttribute].EnsureInstance();
            DrawEditorPreview(instance);
        }

        private static void DrawEditorPreview(Object instance) {
            if (!instance) {
                EditorGUILayout.HelpBox("Unable to create preview instance.", MessageType.Error);
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(instance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }

        private static void DrawCodeBlock() {
            var code = Attributes[_selectedAttribute].CodeExample;
            var style = new GUIStyle(EditorStyles.textArea) {
                font = Font.CreateDynamicFontFromOSFont("Courier New", 12),
                wordWrap = false,
                richText = false
            };

            EditorGUILayout.SelectableLabel(code, style, GUILayout.ExpandHeight(true));
        }

        private static Object EnsureInstance<T>(ref T field, Type type) where T : Component {
            if (field) return field;
            var go = HubTabUtils.CreateOrGetPreviewGo();
            field = go.GetComponent(type) as T;
            if (!field)
                field = go.AddComponent(type) as T;
            return field;
        }

        [InitializeOnLoadMethod]
        private static void CleanupPreviewObject() {
            EditorApplication.quitting += () => {
                if (_imagePreviewInstance)
                    Object.DestroyImmediate(_imagePreviewInstance.gameObject);
            };
        }
    }
}