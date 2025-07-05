using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Common {
    public static class StrixEditorUIUtils {
        /// <summary>
        /// Draws a button with a text label and an icon, adjusting size for layout.
        /// </summary>
        public static void DrawResponsiveButton(string label, string iconName, System.Action onClick, float minWidth = 120f) {
            GUIContent content = new(label, EditorGUIUtility.IconContent(iconName).image);

            if (GUILayout.Button(content, GUILayout.Height(30), GUILayout.MinWidth(minWidth))) {
                onClick?.Invoke();
            }
        }

        /// <summary>
        /// Draws a button with an icon, label, tooltip, and interactable state.
        /// </summary>
        public static void DrawResponsiveButton(
            string label,
            string iconName,
            System.Action onClick,
            bool enabled,
            string tooltip,
            float minWidth = 120f,
            float height = 30f
        ) {
            using (new EditorGUI.DisabledScope(!enabled)) {
                GUIContent content = new(label, EditorGUIUtility.IconContent(iconName).image, tooltip);

                if (GUILayout.Button(content, GUILayout.Height(height), GUILayout.MinWidth(minWidth))) {
                    onClick?.Invoke();
                }
            }
        }
    }
}