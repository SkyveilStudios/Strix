﻿using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var helpBox = (HelpBoxAttribute)attribute;
            var helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(helpBox.Message), EditorGUIUtility.currentViewWidth) + 8f;
            var propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            return helpBoxHeight + propertyHeight + 4f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var helpBox = (HelpBoxAttribute)attribute;

            var helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(helpBox.Message), position.width);
            var helpBoxRect = new Rect(position.x, position.y, position.width, helpBoxHeight);
            EditorGUI.HelpBox(helpBoxRect, helpBox.Message, (MessageType)helpBox.Type);

            var fieldRect = new Rect(position.x, position.y + helpBoxRect.height + 4f, position.width,
                EditorGUI.GetPropertyHeight(property, label, true));
            EditorGUI.PropertyField(fieldRect, property, label, true);
        }
    }
}