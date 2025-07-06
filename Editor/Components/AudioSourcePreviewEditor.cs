using Strix.Runtime.Components;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Components {
    [CustomEditor(typeof(AudioSourcePreview))]
    public class AudioSourcePreviewEditor : UnityEditor.Editor {
        private AudioSourcePreview _preview;
        private AudioSource _source;

        private void OnEnable() {
            _preview = (AudioSourcePreview)target;
            _source = _preview.GetComponent<AudioSource>();
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable() {
            EditorApplication.update -= OnEditorUpdate;
            if (_preview.stopOnDeselect && !Application.isPlaying) StopAudio();
        }

        private void OnEditorUpdate() {
            if (_source != null && _source.isPlaying) Repaint();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            DrawDescriptionBox();
            DrawToggleOptions();
            DrawAudioControls();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDescriptionBox() {
            EditorGUILayout.HelpBox("Preview AudioSource in the Editor without entering Play Mode.\n" +
                                    "If Pitch is negative with Loop turned off the script will not work", MessageType.Info);
            EditorGUILayout.Space(6);
        }

        private void DrawToggleOptions() {
            _preview.stopOnDeselect = EditorGUILayout.Toggle("Stop On Deselect", _preview.stopOnDeselect);
            EditorGUILayout.Space(10);
        }

        private void DrawAudioControls() {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Audio Preview", EditorStyles.boldLabel);

            var hasClip = _source && _source.clip;
            EditorGUI.BeginDisabledGroup(!hasClip);

            DrawPlayStopButton();

            EditorGUI.EndDisabledGroup();

            if (hasClip) {
                DrawClipInfo();
                DrawProgressBar();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPlayStopButton() {
            var playIcon = EditorGUIUtility.IconContent("d_PlayButton");
            var stopIcon = EditorGUIUtility.IconContent("d_PreMatQuad");

            var buttonContent = _preview.isPreviewPlaying && _source.isPlaying ? stopIcon : playIcon;
            buttonContent.text = _preview.isPreviewPlaying ? " Stop Audio" : " Play Audio";

            if (!GUILayout.Button(buttonContent, GUILayout.Height(24))) return;
            if (_preview.isPreviewPlaying && _source.isPlaying) StopAudio();
            else PlayAudio();
        }

        private void DrawClipInfo() {
            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                $"Clip: {_source.clip.name}\nDuration: {_source.clip.length:F2} seconds",
                MessageType.None);
        }

        private void DrawProgressBar() {
            if (!_source.isPlaying) return;
            var progress = Mathf.Clamp01(_source.time / _source.clip.length);
            var rect = GUILayoutUtility.GetRect(128, 16, GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(rect, progress, $"{_source.time:F1} / {_source.clip.length:F1} sec");
        }

        private void PlayAudio() {
            if (!_preview || !_source || !_source.clip) return;
            _source.Play();
            _preview.isPreviewPlaying = true;
            SceneView.RepaintAll();
        }

        private void StopAudio() {
            if (_source && _source.isPlaying) _source.Stop();
            _preview.isPreviewPlaying = false;
            SceneView.RepaintAll();
        }
    }
}