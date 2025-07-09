using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(DisableInEditModeAttribute))]
    [CustomPropertyDrawer(typeof(DisableInPlayModeAttribute))]
    public class DisableModeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var isPlayMode = Application.isPlaying;

            var shouldDisable = attribute switch {
                DisableInPlayModeAttribute => isPlayMode,
                DisableInEditModeAttribute => !isPlayMode,
                _ => false
            };

            var prevGUIState =  GUI.enabled;
            GUI.enabled = !shouldDisable;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = prevGUIState;
        }
    }
}