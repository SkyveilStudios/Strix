#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Strix.Runtime.Attributes;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDrawer : DecoratorDrawer {
        private TitleAttribute Title => (TitleAttribute)attribute;

        public override void OnGUI(Rect position) {
            var titleText = $"<color=#{Title.TitleColorString}>{Title.Title}</color>";
            GUIStyle titleStyle = new(EditorStyles.boldLabel) {
                alignment = Title.AlignTitleLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter,
                richText = true
            };

            var titleSize = titleStyle.CalcSize(new GUIContent(titleText));
            var titleY = position.y;
            var lineY = titleY + EditorGUIUtility.singleLineHeight / 2f;
            var lineHeight = Title.LineHeight;

            var titleX = Title.AlignTitleLeft
                ? position.x
                : position.x + (position.width - titleSize.x) / 2f;

            Rect titleRect = new(titleX, titleY, titleSize.x, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(titleRect, titleText, titleStyle);

            var lineColor = HexToColor(Title.LineColorString);

            if (Title.AlignTitleLeft) {
                var lineStartX = titleRect.xMax + 8f;
                var lineWidth = position.xMax - lineStartX;

                if (!(lineWidth > 0)) return;
                Rect lineRect = new(lineStartX, lineY, lineWidth, lineHeight);
                EditorGUI.DrawRect(lineRect, lineColor);
            }
            else {
                const float space = 10f;

                var leftLineStart = position.x;
                var leftLineEnd = titleRect.xMin - space;
                if (leftLineEnd > leftLineStart) {
                    Rect leftLineRect = new(leftLineStart, lineY, leftLineEnd - leftLineStart, lineHeight);
                    EditorGUI.DrawRect(leftLineRect, lineColor);
                }

                var rightLineStart = titleRect.xMax + space;
                var rightLineEnd = position.x + position.width;
                if (!(rightLineEnd > rightLineStart)) return;
                Rect rightLineRect = new(rightLineStart, lineY, rightLineEnd - rightLineStart, lineHeight);
                EditorGUI.DrawRect(rightLineRect, lineColor);
            }
        }

        public override float GetHeight() {
            return EditorGUIUtility.singleLineHeight + Title.Spacing;
        }

        private static Color HexToColor(string hex) {
            ColorUtility.TryParseHtmlString($"#{hex}", out var color);
            return color;
        }
    }
}
#endif