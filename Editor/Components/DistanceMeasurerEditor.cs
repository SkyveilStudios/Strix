#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DistanceMeasurer))]
public class DistanceMeasurerEditor : Editor {
    private DistanceMeasurer _distanceMeasurer;
    private bool _showTargetsFoldout = true;
    private bool _showDisplayFoldout = true;
    private bool _showTextFoldout = true;
    private bool _showAdvancedFoldout;
    private bool _showUtilityFoldout;

    private string _tagToMeasure = "Player";
    private Color _tagGizmoColor = Color.cyan;

    private void OnEnable() {
        _distanceMeasurer = (DistanceMeasurer)target;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Distance Measurer", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Measures and displays distances between this object and target objects with visual gizmos.", MessageType.Info);

        // Targets section
        _showTargetsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_showTargetsFoldout, "Measurement Targets");
        if (_showTargetsFoldout) {
            EditorGUI.indentLevel++;
            DrawTargetsSection();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Display Settings
        _showDisplayFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_showDisplayFoldout, "Display Settings");
        if (_showDisplayFoldout) {
            EditorGUI.indentLevel++;
            DrawDisplaySection();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Text Settings
        _showTextFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_showTextFoldout, "Text Settings");
        if (_showTextFoldout) {
            EditorGUI.indentLevel++;
            DrawTextSection();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Advanced Options
        _showAdvancedFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_showAdvancedFoldout, "Advanced Options");
        if (_showAdvancedFoldout) {
            EditorGUI.indentLevel++;
            DrawAdvancedSection();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Utility Functions
        _showUtilityFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_showUtilityFoldout, "Utility Functions");
        if (_showUtilityFoldout) {
            EditorGUI.indentLevel++;
            DrawUtilitySection();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        // Real-time distance display
        if (Application.isPlaying) {
            EditorGUILayout.Space();
            DrawRealTimeDistances();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTargetsSection() {
        var targetsProperty = serializedObject.FindProperty("targets");

        // Draw array size field
        EditorGUILayout.PropertyField(targetsProperty.FindPropertyRelative("Array.size"), new GUIContent("Size"));

        // Draw each target manually to avoid nested foldouts
        for (var i = 0; i < targetsProperty.arraySize; i++) {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            var targetElement = targetsProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.LabelField($"Target {i + 1}", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetElement, GUIContent.none);

            // Remove button for this target
            if (GUILayout.Button("Remove", GUILayout.Width(60))) {
                targetsProperty.DeleteArrayElementAtIndex(i);
                EditorUtility.SetDirty(_distanceMeasurer);
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();

        // Add new target button
        if (GUILayout.Button("Add New Target")) {
            targetsProperty.arraySize++;
            EditorUtility.SetDirty(_distanceMeasurer);
        }

        // Quick add target from selection
        if (Selection.activeTransform != null && Selection.activeTransform != _distanceMeasurer.transform) {
            if (GUILayout.Button($"Add Selected: {Selection.activeTransform.name}")) {
                _distanceMeasurer.AddTarget(Selection.activeTransform);
                EditorUtility.SetDirty(_distanceMeasurer);
            }
        }

        // Clear all targets
        if (GUILayout.Button("Clear All Targets")) {
            if (EditorUtility.DisplayDialog("Clear Targets", "Are you sure you want to clear all targets?", "Yes", "No")) {
                targetsProperty.ClearArray();
                EditorUtility.SetDirty(_distanceMeasurer);
            }
        }
    }

    private void DrawDisplaySection() {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showInPlayMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lineWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showSelfPosition"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("selfPositionColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sphereRadius"));
    }

    private void DrawTextSection() {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showUnitSuffix"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("decimalPlaces"));
    }

    private void DrawAdvancedSection() {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useWorldSpace"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showMidpoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("midpointColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("midpointSize"));
    }

    private void DrawUtilitySection() {
        EditorGUILayout.LabelField("Batch Operations", EditorStyles.miniBoldLabel);

        // Measure to objects with tag
        EditorGUILayout.BeginHorizontal();
        _tagToMeasure = EditorGUILayout.TextField("Tag:", _tagToMeasure);
        _tagGizmoColor = EditorGUILayout.ColorField(_tagGizmoColor);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add All Objects with Tag")) {
            _distanceMeasurer.MeasureToObjectsWithTag(_tagToMeasure, _tagGizmoColor);
            EditorUtility.SetDirty(_distanceMeasurer);
        }

        EditorGUILayout.Space();

        // Quick distance check
        if (Selection.activeTransform != null && Selection.activeTransform != _distanceMeasurer.transform) {
            float distance = Vector3.Distance(_distanceMeasurer.transform.position, Selection.activeTransform.position);
            EditorGUILayout.HelpBox($"Distance to {Selection.activeTransform.name}: {distance:F2} units", MessageType.Info);
        }
    }

    private void DrawRealTimeDistances() {
        EditorGUILayout.LabelField("Real-time Distances", EditorStyles.boldLabel);

        var distances = _distanceMeasurer.GetAllDistances();

        if (distances.Count == 0) {
            EditorGUILayout.HelpBox("No targets to measure", MessageType.Info);
            return;
        }

        foreach (var kvp in distances) {
            if (kvp.Key != null) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(kvp.Key.name, GUILayout.Width(150));
                EditorGUILayout.LabelField($"{kvp.Value:F2} units", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void OnSceneGUI() {
        if (_distanceMeasurer == null) return;

        // Draw enhanced gizmos in scene view
        Handles.color = Color.white;

        // Draw a label at the object's position
        var position = _distanceMeasurer.transform.position;
        Handles.Label(position + Vector3.up * 0.5f, $"Distance Measurer\n{_distanceMeasurer.name}", EditorStyles.whiteLabel);

        // Allow dragging targets in scene view
        var distances = _distanceMeasurer.GetAllDistances();
        foreach (var kvp in distances) {
            if (kvp.Key != null) {
                var targetPos = kvp.Key.position;
                var midpoint = (position + targetPos) * 0.5f;

                // Draw enhanced distance label
                var distanceLabel = $"{kvp.Key.name}\n{kvp.Value:F2}u";
                Handles.Label(midpoint, distanceLabel, EditorStyles.whiteLabel);

                // Draw clickable handle at midpoint
                Handles.color = Color.yellow;
                if (Handles.Button(midpoint, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap)) {
                    Selection.activeTransform = kvp.Key;
                }
            }
        }
    }
}

// Custom property drawer for MeasurementTarget
[CustomPropertyDrawer(typeof(MeasurementTarget))]
public class MeasurementTargetDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        var singleLineHeight = EditorGUIUtility.singleLineHeight;
        var spacing = EditorGUIUtility.standardVerticalSpacing;

        var targetRect = new Rect(position.x, position.y, position.width, singleLineHeight);
        var colorRect = new Rect(position.x, position.y + singleLineHeight + spacing, position.width * 0.5f, singleLineHeight);
        var showDistanceRect = new Rect(position.x + position.width * 0.5f, position.y + singleLineHeight + spacing, position.width * 0.5f, singleLineHeight);

        var showLineRect = new Rect(position.x, position.y + (singleLineHeight + spacing) * 2, position.width * 0.33f, singleLineHeight);
        var show3DRect = new Rect(position.x + position.width * 0.33f, position.y + (singleLineHeight + spacing) * 2, position.width * 0.33f, singleLineHeight);
        var showHorizontalRect = new Rect(position.x + position.width * 0.66f, position.y + (singleLineHeight + spacing) * 2, position.width * 0.34f, singleLineHeight);

        var showVerticalRect = new Rect(position.x, position.y + (singleLineHeight + spacing) * 3, position.width, singleLineHeight);

        // Draw properties
        EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("target"));
        EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("gizmoColor"), new GUIContent("Color"));
        EditorGUI.PropertyField(showDistanceRect, property.FindPropertyRelative("showDistance"), new GUIContent("Show Distance"));

        EditorGUI.PropertyField(showLineRect, property.FindPropertyRelative("showLine"), new GUIContent("Line"));
        EditorGUI.PropertyField(show3DRect, property.FindPropertyRelative("show3DDistance"), new GUIContent("3D"));
        EditorGUI.PropertyField(showHorizontalRect, property.FindPropertyRelative("showHorizontalDistance"), new GUIContent("Horizontal"));
        EditorGUI.PropertyField(showVerticalRect, property.FindPropertyRelative("showVerticalDistance"), new GUIContent("Show Vertical Distance"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
    }
}
#endif