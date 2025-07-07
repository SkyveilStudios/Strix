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

        public static void DrawComponentsTab() {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));
            DrawComponentList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            HubTabUtils.DrawHeader("Description");
            DrawDescription();

            HubTabUtils.DrawSeparator();
            HubTabUtils.DrawHeader("Preview");
            DrawPreview();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawComponentList() {
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
                _selectedComponent = type;
            }
        }

        private static void DrawDescription() {
            var description = _selectedComponent switch {
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
                    EnsureAudioSourcePreview();
                    DrawEditorPreview(_audioSourceInstance);
                    break;
                case ComponentType.SceneNote:
                    EnsureSceneNotePreview();
                    DrawEditorPreview(_sceneNoteInstance);
                    break;
                case ComponentType.TransformLock:
                    EnsureTransformLockPreview();
                    DrawEditorPreview(_transformLockInstance);
                    break;
            }
        }

        private static void DrawEditorPreview(Object instance) {
            if (!instance) {
                EditorGUILayout.HelpBox("Unable to create preview", MessageType.Error);
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(instance, null, ref _previewEditor);
            _previewEditor?.OnInspectorGUI();
        }

        private static void EnsureAudioSourcePreview() {
            if (_audioSourceInstance) return;
            var go = HubTabUtils.CreateOrGetPreviewGo();
            if (!go.TryGetComponent(out _audioSourceInstance))
                _audioSourceInstance = go.AddComponent<AudioSourcePreview>();

            if (!go.TryGetComponent(out AudioSource source))
                source = go.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.volume = 0.3f;

            const string path = "Assets/Strix/Editor/Components/AudioSourcePreview/Real Pianos - Defeat.wav";
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip) source.clip = clip;
        }

        private static void EnsureSceneNotePreview() {
            if (_sceneNoteInstance) return;
            var go = HubTabUtils.CreateOrGetPreviewGo();
            if (!go.TryGetComponent(out _sceneNoteInstance))
                _sceneNoteInstance = go.AddComponent<SceneNote>();
        }

        private static void EnsureTransformLockPreview() {
            if (_transformLockInstance) return;
            var go = HubTabUtils.CreateOrGetPreviewGo();
            if (!go.TryGetComponent(out _transformLockInstance))
                _transformLockInstance = go.AddComponent<TransformLock>();
        }

        [InitializeOnLoadMethod]
        private static void CleanupPreviewObjects() {
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
