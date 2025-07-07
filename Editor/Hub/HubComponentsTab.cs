using System;
using Strix.Runtime.Components;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Strix.Editor.Hub {
    public static class HubComponentsTab {
        private enum ComponentType {
            AudioSourcePreview,
            SceneNote,
            TransformLock
        }

        private static AudioSourcePreview _audioSourceInstance;
        private static SceneNote _sceneNoteInstance;
        private static TransformLock _transformLockInstance;
        private static UnityEditor.Editor _previewEditor;
        private static Vector2 _scroll;

        private static ComponentType _selectedComponent = ComponentType.AudioSourcePreview;
        
        public static void DrawComponentsTab()
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
            DrawComponentList();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawSectionHeader("Description");
            DrawDescription();

            DrawSeparator();
            
            DrawPreview();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        
        private static void DrawComponentList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
            
            GUILayout.Space(0);
            var bgRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(new Rect(bgRect.x, bgRect.y, 200, Screen.height), new Color(0.20f, 0.20f, 0.20f));

            _scroll = GUILayout.BeginScrollView(
                _scroll,
                GUIStyle.none,
                GUI.skin.verticalScrollbar
            );

            
            var labelStyle = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 32,
                padding = new RectOffset(0, 0, -15, 0)
            };
            
            EditorGUILayout.LabelField("Components", labelStyle, GUILayout.ExpandWidth(true));
            var lineRect = GUILayoutUtility.GetRect(1, 2, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(lineRect, new Color(0.7f, 0.7f, 0.7f));

            DrawComponentButton("AudioSourcePreview", ComponentType.AudioSourcePreview);
            DrawComponentButton("SceneNote", ComponentType.SceneNote);
            DrawComponentButton("TransformLock", ComponentType.TransformLock);

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        
        private static void DrawComponentButton(string label, ComponentType type) {
            var isSelected = _selectedComponent == type;

            var style = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 30,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(10, 10, 0, 0),
                border = new RectOffset(0, 0, 0, 0),
                normal = {
                    background = MakeTex(1, 1, isSelected ? new Color(0.30f, 0.48f, 0.55f) : new Color(0.15f, 0.15f, 0.15f)),
                    textColor = isSelected ? Color.white : Color.gray
                },
                hover = {
                    background = MakeTex(1, 1, isSelected ? new Color(0.30f, 0.48f, 0.55f) : new Color(0.25f, 0.25f, 0.25f)),
                    textColor = Color.white
                },
                active = {
                    background = MakeTex(1, 1, new Color(0.30f, 0.48f, 0.55f)),
                    textColor = Color.white
                }
            };

            if (GUILayout.Button(label, style, GUILayout.ExpandWidth(true), GUILayout.Width(200))) {
                _selectedComponent = type;
            }
        }

        private static Texture2D MakeTex(int width, int height, Color col) {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; ++i)
                pix[i] = col;

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        private static void DrawSectionHeader(string title) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }
        
        private static void DrawSeparator()
        {
            var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
            GUILayout.Space(4);
        }
        
        private static void DrawDescription()
        {
            var description = _selectedComponent switch
            {
                ComponentType.AudioSourcePreview => "Allows you to preview audio clips directly in the editor without entering Play Mode.",
                ComponentType.SceneNote => "Adds developer notes directly into your scene view to help communicate intent or reminders.",
                ComponentType.TransformLock => "Locks transform changes to prevent accidental editing in the Unity Editor.",
                _ => "No description available."
            };

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(description);
            EditorGUILayout.EndVertical();
        }

        private static void DrawPreview() {
            switch (_selectedComponent) {
                case ComponentType.AudioSourcePreview:
                    DrawAudioSourcePreview();
                    break;
                case ComponentType.SceneNote:
                    DrawSceneNotePreview();
                    break;
                case ComponentType.TransformLock:
                    DrawTransformLockPreview();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DrawAudioSourcePreview() {
            CheckAudioPreview();
            if (!_audioSourceInstance) {
                EditorGUILayout.HelpBox("Unable to create preview", MessageType.Error);
                return;
            }
            
            UnityEditor.Editor.CreateCachedEditor(_audioSourceInstance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }

        private static void DrawSceneNotePreview() {
            CheckSceneNotePreview();
            if (!_sceneNoteInstance) {
                EditorGUILayout.HelpBox("Unable to create preview", MessageType.Error);
                return;
            }
            
            UnityEditor.Editor.CreateCachedEditor(_sceneNoteInstance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }
        
        private static void DrawTransformLockPreview() {
            CheckTransformLockPreview();
            if (!_transformLockInstance) {
                EditorGUILayout.HelpBox("Unable to create preview", MessageType.Error);
                return;
            }
            
            UnityEditor.Editor.CreateCachedEditor(_transformLockInstance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }
        
        private static void CheckTransformLockPreview() {
            if (_transformLockInstance) return;
            
            var go = GameObject.Find("StrixPreview") ?? new GameObject("StrixPreview") {
                hideFlags = HideFlags.HideAndDontSave
            };

            if (!go.TryGetComponent(out _transformLockInstance)) {
                _transformLockInstance = go.AddComponent<TransformLock>();
            }
        }

        private static void CheckSceneNotePreview() {
            if (_sceneNoteInstance) return;
            
            var go = GameObject.Find("StrixPreview") ?? new GameObject("StrixPreview") {
                hideFlags = HideFlags.HideAndDontSave
            };

            if (!go.TryGetComponent(out _sceneNoteInstance)) {
                _sceneNoteInstance = go.AddComponent<SceneNote>();
            }
        }

        private static void CheckAudioPreview() {
            if (_audioSourceInstance) return;

            var go = GameObject.Find("StrixPreview") ?? new GameObject("StrixPreview") {
                hideFlags = HideFlags.HideAndDontSave
            };
            
            if (!go.TryGetComponent(out _audioSourceInstance)) {
                _audioSourceInstance = go.AddComponent<AudioSourcePreview>();
            }

            if (!go.TryGetComponent(out AudioSource source)) {
                source = go.AddComponent<AudioSource>();
            }
            
            source.playOnAwake = false;
            source.volume = 0.3f;

            const string audioClipPath = "Assets/Strix/Editor/Components/AudioSourcePreview/Real Pianos - Defeat.wav";
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);
            if (clip) {
                source.clip = clip;
            }
        }

        [InitializeOnLoadMethod]
        private static void DeletePreview() {
            EditorApplication.quitting += () => {
                if (_audioSourceInstance)
                    Object.DestroyImmediate(_audioSourceInstance.gameObject);
                if (_sceneNoteInstance)
                    Object.DestroyImmediate(_sceneNoteInstance.gameObject);
                if (_transformLockInstance)
                    Object.DestroyImmediate(_transformLockInstance.gameObject);
            };
        }
    }
}