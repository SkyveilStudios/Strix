using System;
using Strix.Runtime.Hub.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Strix.Editor.Hub {
    public static class HubAttributesTab {
        private enum AttributeType {
            ImagePreview,
            Required
        }

        private static HubImagePreview _imagePreviewInstance;
        private static HubRequired _requiredInstance;
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

            DrawAttributeButton("ImagePreview", AttributeType.ImagePreview);
            DrawAttributeButton("Required", AttributeType.Required);

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
            var description = _selectedAttribute switch {
                AttributeType.ImagePreview =>
                    "Allows you to embed a custom image directly above a field in the inspector.\n\n" +
                    "<b>Parameters:</b>\n" +
                    "• <b>path</b> (string): Path to the image within the Assets folder.\n" +
                    "• <b>width</b> (float): Width of the image (ignored if fullWidth is true).\n" +
                    "• <b>fullWidth</b> (bool): If true, stretches image to full inspector width.\n" +
                    "• <b>alignment</b> (ImageAlignment): If not full width, controls image alignment: Left, Center, or Right.",
                AttributeType.Required => "Displays a warning that a GameObject is missing/null",
                _ => "Preview"
            };

            var style = new GUIStyle(EditorStyles.label) {
                wordWrap = true,
                richText = true
            };

            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(description, style);
            EditorGUILayout.EndVertical();
        }

        private static void DrawPreview() {
            switch (_selectedAttribute) {
                case AttributeType.ImagePreview:
                    EnsureImagePreviewInstance();
                    DrawEditorPreview(_imagePreviewInstance);
                    break;
                case AttributeType.Required:
                    EnsureRequiredInstance();
                    DrawEditorPreview(_requiredInstance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DrawEditorPreview(Object instance) {
            if (!instance) {
                EditorGUILayout.HelpBox("Unable to create preview instance.", MessageType.Error);
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(instance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }

        private static void EnsureRequiredInstance() {
            if (_requiredInstance) return;
            var go = HubTabUtils.CreateOrGetPreviewGo();
            if (!go.TryGetComponent(out _requiredInstance))
                _requiredInstance = go.AddComponent<HubRequired>();
        }

        private static void EnsureImagePreviewInstance() {
            if (_imagePreviewInstance) return;
            var go = HubTabUtils.CreateOrGetPreviewGo();
            if (!go.TryGetComponent(out _imagePreviewInstance))
                _imagePreviewInstance = go.AddComponent<HubImagePreview>();
        }

        private static void DrawCodeBlock() {
            var code = _selectedAttribute switch {
                AttributeType.ImagePreview => "[ImagePreview(\"Assets/Strix/Banners/StrixBanner.jpg\", 400f)]\n[SerializeField] private GameObject obj;",
                AttributeType.Required => "[Required]\n[SerializeField] public GameObject obj;",
                _ => throw new ArgumentOutOfRangeException()
            };

            var style = new GUIStyle(EditorStyles.textArea) {
                font = Font.CreateDynamicFontFromOSFont("Courier New", 12),
                wordWrap = false,
                richText = false
            };

            EditorGUILayout.SelectableLabel(code, style, GUILayout.ExpandHeight(true));
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
