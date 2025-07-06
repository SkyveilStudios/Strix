#if UNITY_EDITOR
using Strix.Runtime.Components;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Components {
    /// <summary>
    /// Editor only component showing axis toggles for position, rotation, and scale.
    /// </summary>
    [CustomEditor(typeof(TransformLock))]
    public class TransformLockEditor : UnityEditor.Editor {
        private static Texture2D _lockIcon;
        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox(
                "Locks the transform of this GameObject in the Editor to prevent accidental edits.\n" +
                "This component is ignored at runtime.",
                MessageType.Info);

            serializedObject.Update();

            DrawAxisSection("Position", "lockPositionX", "lockPositionY", "lockPositionZ");
            DrawAxisSection("Rotation", "lockRotationX", "lockRotationY", "lockRotationZ");
            DrawAxisSection("Scale", "lockScaleX", "lockScaleY", "lockScaleZ");

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAxisSection(string label, string xProp, string yProp, string zProp) {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            DrawToggle("X", serializedObject.FindProperty(xProp));
            DrawToggle("Y", serializedObject.FindProperty(yProp));
            DrawToggle("Z", serializedObject.FindProperty(zProp));

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawToggle(string axisLabel, SerializedProperty property) {
            property.boolValue = GUILayout.Toggle(
                property.boolValue,
                axisLabel,
                "Button",
                GUILayout.Width(30));
        }
        
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawLockIconGizmo(TransformLock target, GizmoType gizmoType) {
            if (!AnyAxisLocked(target)) return;

            if (_lockIcon == null)
                _lockIcon = EditorGUIUtility.IconContent("AssemblyLock").image as Texture2D;

            if (_lockIcon == null) return;

            var worldPos = target.transform.position + Vector3.up * 2f;
            var guiPoint = HandleUtility.WorldToGUIPoint(worldPos);

            Handles.BeginGUI();
            const float size = 24f;
            GUI.DrawTexture(
                new Rect(guiPoint.x - size * 0.5f, guiPoint.y - size * 0.5f, size, size),
                _lockIcon);
            Handles.EndGUI();
        }

        private static bool AnyAxisLocked(TransformLock t) {
            return t.lockPositionX || t.lockPositionY || t.lockPositionZ ||
                   t.lockRotationX || t.lockRotationY || t.lockRotationZ ||
                   t.lockScaleX    || t.lockScaleY    || t.lockScaleZ;
        }
    }
}
#endif