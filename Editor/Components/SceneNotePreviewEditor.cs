﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Strix.Runtime.Components;

namespace Strix.Editor.Components {
    /// <summary>
    /// Handles editor rendering of SceneNote titles and markers in the Scene view
    /// </summary>
    [InitializeOnLoad]
    public static class SceneNotePreviewEditor {
        static SceneNotePreviewEditor() { SceneView.duringSceneGui += OnSceneGUI; }

        private static void OnSceneGUI(SceneView sceneView) {
            var notes = Object.FindObjectsByType<SceneNote>(FindObjectsSortMode.None);

            foreach (var note in notes) {
                if (note == null || !note.isActiveAndEnabled || !note.ShouldDisplayInEditor) continue;
                var color = note.CategoryColor;
                if (!note.showMarker) continue;
                Handles.color = color;
                Handles.DrawWireDisc(note.transform.position, Vector3.up, note.markerRadius);
            }
            Handles.BeginGUI();

            foreach (var note in notes) {
                if (note == null || !note.isActiveAndEnabled || !note.ShouldDisplayInEditor) continue;

                if (!note.showTitle || string.IsNullOrWhiteSpace(note.title)) continue;
                var textColor = note.CategoryColor;
                var style = new GUIStyle(EditorStyles.boldLabel) {
                    fontSize = note.titleSize,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState { textColor = textColor }
                };
                DrawLabelWithOutline(note.transform.position + note.worldOffset, note.title, style, Color.black);
            }
            Handles.EndGUI();
        }

        private static void DrawLabelWithOutline(Vector3 worldPos, string text, GUIStyle style, Color outlineColor) {
            var directions = new[] {
                Vector3.up * 0.01f,
                Vector3.down * 0.01f,
                Vector3.left * 0.01f,
                Vector3.right * 0.01f
            };

            var shadowStyle = new GUIStyle(style) { normal = new GUIStyleState { textColor = outlineColor } };
            foreach (var dir in directions) Handles.Label(worldPos + dir, text, shadowStyle);
            Handles.Label(worldPos, text, style);
        }
    }
}
#endif
