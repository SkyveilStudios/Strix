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

        private static AttributeType _selectedAttribute = AttributeType.ImagePreview;

        public static void DrawAttributesTab() {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(150), GUILayout.ExpandHeight(true));
            DrawAttributeList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawSectionHeader("Description");
            DrawDescription();

            DrawSeparator();

            DrawSectionHeader("Preview");
            DrawPreview();

            DrawSeparator();

            DrawSectionHeader("Code Example");
            DrawCodeBlock();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawAttributeList() {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);

            if (GUILayout.Button("ImagePreview"))
                _selectedAttribute = AttributeType.ImagePreview;
            
            if (GUILayout.Button("Required"))
                _selectedAttribute = AttributeType.Required;

            EditorGUILayout.EndVertical();
        }

        private static void DrawSectionHeader(string title) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private static void DrawSeparator() {
            var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
            GUILayout.Space(4);
        }

        private static void DrawDescription() {
            string description = _selectedAttribute switch {
                AttributeType.ImagePreview =>
                    "Allows you to embed a custom image directly above a field in the inspector.\n\n" +
                    "<b>Parameters:</b>\n" +
                    "• <b>path</b> (string): Path to the image within the Assets folder.\n" +
                    "• <b>width</b> (float): Width of the image (ignored if fullWidth is true).\n" +
                    "• <b>fullWidth</b> (bool): If true, stretches image to full inspector width.\n" +
                    "• <b>alignment</b> (ImageAlignment): If not full width, controls image alignment: Left, Center, or Right.",
                AttributeType.Required =>
                    "Displays a warning that a GameObject is missing/null",
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
                    DrawImagePreview();
                    break;
                case AttributeType.Required:
                    DrawRequired();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DrawImagePreview() {
            EnsureImagePreviewInstance();

            if (!_imagePreviewInstance) {
                EditorGUILayout.HelpBox("Unable to create preview instance.", MessageType.Error);
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(_imagePreviewInstance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }
        
        private static void DrawRequired() {
            EnsureRequiredInstance();

            if (!_requiredInstance) {
                EditorGUILayout.HelpBox("Unable to create preview instance.", MessageType.Error);
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(_requiredInstance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }
        
        private static void EnsureRequiredInstance() {
            if (_requiredInstance) return;

            var go = GameObject.Find("StrixPreview") ?? new GameObject("StrixPreview") {
                hideFlags = HideFlags.HideAndDontSave
            };

            if (!go.TryGetComponent(out _requiredInstance))
                _requiredInstance = go.AddComponent<HubRequired>();
        }

        private static void EnsureImagePreviewInstance() {
            if (_imagePreviewInstance) return;

            var go = GameObject.Find("StrixPreview") ?? new GameObject("StrixPreview") {
                hideFlags = HideFlags.HideAndDontSave
            };

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