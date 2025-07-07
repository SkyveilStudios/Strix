#if UNITY_EDITOR
using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            var isMissing = IsMissing(property);
            var warningHeight = isMissing? EditorGUIUtility.singleLineHeight * 1.5f + 4f: 0f;
            
            if (isMissing) {
                var warningRect = new Rect(position.x, position.y, position.width, warningHeight);
                EditorGUI.HelpBox(warningRect, $"{property.displayName} is required!", MessageType.Error);
            }
            
            var fieldRect = new Rect(position.x, position.y + warningHeight, position.width, EditorGUI.GetPropertyHeight(property, label, true));
            EditorGUI.PropertyField(fieldRect, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);
            return fieldHeight + (IsMissing(property) ? EditorGUIUtility.singleLineHeight * 1.5f + 4f: 0f);
        }

        private static bool IsMissing(SerializedProperty property) {
            return property.propertyType switch {
                SerializedPropertyType.ObjectReference => property.objectReferenceValue == null,
                SerializedPropertyType.String => string.IsNullOrEmpty(property.stringValue),
                _ => false,
            };
        }
    }
}
#endif