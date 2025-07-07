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

        private static ComponentType _selectedComponent = ComponentType.AudioSourcePreview;
        
        public static void DrawComponentsTab()
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical(GUILayout.Width(150), GUILayout.ExpandHeight(true));
            DrawComponentList();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawDescription();

            DrawSeparator();
            
            DrawPreview();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        
        private static void DrawComponentList()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);

            if (GUILayout.Button("AudioSourcePreview"))
                _selectedComponent = ComponentType.AudioSourcePreview;

            if (GUILayout.Button("SceneNote"))
                _selectedComponent = ComponentType.SceneNote;

            if (GUILayout.Button("TransformLock"))
                _selectedComponent = ComponentType.TransformLock;

            EditorGUILayout.EndVertical();
        }
        
        private static void DrawSeparator()
        {
            var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
            GUILayout.Space(4);
        }
        
        private static void DrawDescription()
        {
            string title = _selectedComponent switch
            {
                ComponentType.AudioSourcePreview => "AudioSource Preview:",
                ComponentType.SceneNote => "Scene Note:",
                ComponentType.TransformLock => "TransformLock:",
                _ => "No title available:"
            };
            string description = _selectedComponent switch
            {
                ComponentType.AudioSourcePreview => "Allows you to preview audio clips directly in the editor without entering Play Mode.",
                ComponentType.SceneNote => "Adds developer notes directly into your scene view to help communicate intent or reminders.",
                ComponentType.TransformLock => "Locks transform changes to prevent accidental editing in the Unity Editor.",
                _ => "No description available."
            };

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
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