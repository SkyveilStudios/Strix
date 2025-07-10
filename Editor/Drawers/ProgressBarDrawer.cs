using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var attr = (ProgressBarAttribute)attribute;
            var value = GetNumericValue(property);
            var min = attr.Min;
            var max = attr.Max;
            var percent = Mathf.InverseLerp(min, max, value);
            
            Rect barRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var fillWidth = Mathf.Clamp01(percent) * barRect.width;
            
            Color borderColor = new(0.35f, 0.35f, 0.35f);
            Color bgColor = new(0.15f, 0.15f, 0.15f);
            var fgColor = GetColor(attr.BarColor);
            var textColor = Color.white;
            
            EditorGUI.DrawRect(barRect, bgColor);
            
            var fillRect = new Rect(barRect.x, barRect.y, fillWidth, barRect.height);
            EditorGUI.DrawRect(fillRect, fgColor);

            Handles.color = borderColor;
            Handles.DrawSolidRectangleWithOutline(barRect, Color.clear, borderColor);

            var displayLabel = $"{attr.Label}: {value:0.##} / {max:0.##}";
            var style = new GUIStyle(EditorStyles.label) {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = textColor },
                fontStyle = FontStyle.Bold,
            };
            DrawOutlinedLabel(barRect, displayLabel, style, Color.black, textColor);

            if (!attr.IsInteractable) return;

            Rect sliderRect = new(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(sliderRect, label, property);
            EditorGUI.BeginChangeCheck();

            var newValue = EditorGUI.Slider(sliderRect, GUIContent.none, value, min, max);

            if (EditorGUI.EndChangeCheck()) {
                SetNumericValue(property, newValue);
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var attr = (ProgressBarAttribute)attribute;
            var height = EditorGUIUtility.singleLineHeight;
            if (attr.IsInteractable) height += EditorGUIUtility.singleLineHeight + 4;
            return height;
        }

        private static float GetNumericValue(SerializedProperty property) {
            return property.propertyType switch {
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Generic when property.type == "double" => (float)property.doubleValue,
                _ => 0f
            };
        }

        private static void SetNumericValue(SerializedProperty property, float value) {
            switch (property.propertyType) {
                case SerializedPropertyType.Float:
                    property.floatValue = value;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = Mathf.RoundToInt(value);
                    break;
                case SerializedPropertyType.Generic when property.type == "double":
                    property.doubleValue = value;
                    break;
            }
        }
        
        private static void DrawOutlinedLabel(Rect position, string text, GUIStyle style, Color outlineColor, Color textColor) {
            var originalColor = style.normal.textColor;
            style.normal.textColor = outlineColor;
            
            EditorGUI.LabelField(new Rect(position.x - 1, position.y, position.width, position.height), text, style);
            EditorGUI.LabelField(new Rect(position.x + 1, position.y, position.width, position.height), text, style);
            EditorGUI.LabelField(new Rect(position.x, position.y - 1, position.width, position.height), text, style);
            EditorGUI.LabelField(new Rect(position.x, position.y + 1, position.width, position.height), text, style);
            
            style.normal.textColor = textColor;
            EditorGUI.LabelField(position, text, style);
            style.normal.textColor = originalColor;
        }
        
        private static Color GetColor(ProgressBarColor barColor) => barColor switch {
            ProgressBarColor.Red => Color.red,
            ProgressBarColor.Green => Color.green,
            ProgressBarColor.Blue => Color.blue,
            ProgressBarColor.Yellow => Color.yellow,
            ProgressBarColor.Cyan => Color.cyan,
            ProgressBarColor.Magenta => Color.magenta,
            ProgressBarColor.Gray => Color.gray,
            ProgressBarColor.White => Color.white,
            ProgressBarColor.Orange => new Color(1f, 0.5f, 0f),
            ProgressBarColor.Purple => new Color(0.5f, 0f, 0.5f),
            ProgressBarColor.Pink => new Color(1f, 0.4f, 0.7f),
            ProgressBarColor.Teal => new Color(0f, 0.5f, 0.5f),
            ProgressBarColor.Lime => new Color(0.7f, 1f, 0f),
            ProgressBarColor.Indigo => new Color(0.29f, 0f, 0.51f),
            ProgressBarColor.Gold => new Color(1f, 0.84f, 0f),
            ProgressBarColor.Aquamarine => new Color(0.5f, 1f, 0.83f),
            ProgressBarColor.Beige => new Color(0.96f, 0.96f, 0.86f),
            ProgressBarColor.Brown => new Color(0.59f, 0.29f, 0f),
            ProgressBarColor.Coral => new Color(1f, 0.5f, 0.31f),
            ProgressBarColor.Crimson => new Color(0.86f, 0.08f, 0.24f),
            ProgressBarColor.DarkBlue => new Color(0f, 0f, 0.55f),
            ProgressBarColor.DarkGreen => new Color(0f, 0.39f, 0f),
            ProgressBarColor.DarkRed => new Color(0.55f, 0f, 0f),
            ProgressBarColor.Ivory => new Color(1f, 1f, 0.94f),
            ProgressBarColor.Lavender => new Color(0.9f, 0.9f, 0.98f),
            ProgressBarColor.LightBlue => new Color(0.68f, 0.85f, 0.9f),
            ProgressBarColor.LightGreen => new Color(0.56f, 0.93f, 0.56f),
            ProgressBarColor.Navy => new Color(0f, 0f, 0.5f),
            ProgressBarColor.Olive => new Color(0.5f, 0.5f, 0f),
            ProgressBarColor.Peach => new Color(1f, 0.9f, 0.71f),
            ProgressBarColor.Salmon => new Color(0.98f, 0.5f, 0.45f),
            ProgressBarColor.Silver => new Color(0.75f, 0.75f, 0.75f),
            ProgressBarColor.Tan => new Color(0.82f, 0.71f, 0.55f),
            ProgressBarColor.Turquoise => new Color(0.25f, 0.88f, 0.82f),
            ProgressBarColor.Violet => new Color(0.93f, 0.51f, 0.93f),
            _ => Color.green
        };
    }
}