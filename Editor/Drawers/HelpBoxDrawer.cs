using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var helpBox = (HelpBoxAttribute)attribute;
            var helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(helpBox.Message), EditorGUIUtility.currentViewWidth) + 8f;
            return helpBoxHeight + EditorGUI.GetPropertyHeight(property, label, true);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var helpBox = (HelpBoxAttribute)attribute;
            
            var helpBoxRect = new Rect(position.x, position.y, position.width, 
                EditorStyles.helpBox.CalcHeight(new GUIContent(helpBox.Message), position.width));
            EditorGUI.HelpBox(helpBoxRect, helpBox.Message, helpBox.Type);
            
            var fieldRect = new Rect(position.x, position.y + helpBoxRect.height + 4, position.width,
                EditorGUI.GetPropertyHeight(property, label, true));
            EditorGUI.PropertyField(fieldRect, property, label, true);
        }
    }
}