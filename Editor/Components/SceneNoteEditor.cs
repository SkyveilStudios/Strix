#if UNITY_EDITOR
using Strix.Runtime.Components;
using UnityEditor;

namespace Strix.Editor.Components {
    /// <summary>
    /// Inspector component for SceneNote
    /// </summary>
    [CustomEditor(typeof(SceneNote))]
    public class SceneNoteEditor : UnityEditor.Editor {
        private SerializedProperty _title, _description, _titleSize, _worldOffset, _visibility, _showTitle;
        private SerializedProperty _category, _customColor, _showMarker, _markerRadius;

        private void OnEnable() {
            _showTitle = serializedObject.FindProperty("showTitle");
            _title = serializedObject.FindProperty("title");
            _titleSize = serializedObject.FindProperty("titleSize");
            _worldOffset = serializedObject.FindProperty("worldOffset");
            _visibility = serializedObject.FindProperty("visibility");
            _category = serializedObject.FindProperty("category");
            _customColor = serializedObject.FindProperty("customColor");
            _description = serializedObject.FindProperty("description");
            _showMarker = serializedObject.FindProperty("showMarker");
            _markerRadius = serializedObject.FindProperty("markerRadius");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox(
                "Displays a floating note and optional marker in the Scene view." +
                "\nColor is determined by category unless 'Custom' is selected.",
                MessageType.Info);

            serializedObject.Update();

            EditorGUILayout.PropertyField(_showTitle);
            EditorGUILayout.PropertyField(_title);
            EditorGUILayout.PropertyField(_titleSize);
            EditorGUILayout.PropertyField(_worldOffset);
            EditorGUILayout.PropertyField(_visibility);
            EditorGUILayout.PropertyField(_category);

            if ((SceneNote.NoteCategory)_category.enumValueIndex == SceneNote.NoteCategory.Custom) {
                EditorGUILayout.PropertyField(_customColor);
            }
            
            EditorGUILayout.PropertyField(_description);

            EditorGUILayout.Space(6);
            EditorGUILayout.PropertyField(_showMarker);
            if (_showMarker.boolValue)
                EditorGUILayout.PropertyField(_markerRadius);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif