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
    }
}
#endif