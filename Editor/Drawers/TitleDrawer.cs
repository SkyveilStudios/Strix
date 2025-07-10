#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Strix.Runtime.Attributes;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDrawer : DecoratorDrawer {
        #region Constants
        private const float DefaultSpace = 10f;
        private const float LineOffsetFromTitle = 8f;
        private const float MinLineWidth = 1f;
        #endregion
        
        #region Cache
        private static readonly Dictionary<string, GUIContent> ContentCache = new();
        private static readonly Dictionary<TitleColor, Color> ColorCache = new();
        private GUIStyle _cachedTitleStyle;
        private bool _styleInitialized;
        #endregion
        
        #region Properties
        private TitleAttribute Title => (TitleAttribute)attribute;
        #endregion

        #region Public Methods
        public override void OnGUI(Rect position) {
            if (!_styleInitialized) {
                InitializeStyle();
            }

            var titleContent = GetCachedContent();
            var titleSize = _cachedTitleStyle.CalcSize(titleContent);
            var layoutData = CalculateLayout(position, titleSize);

            DrawTitle(layoutData.TitleRect, titleContent);
            DrawLines(layoutData);
        }

        public override float GetHeight() {
            return EditorGUIUtility.singleLineHeight + Title.Spacing;
        }
        #endregion

        #region Private Methods
        private void InitializeStyle() {
            _cachedTitleStyle = new GUIStyle(EditorStyles.boldLabel) {
                alignment = Title.AlignTitleLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter,
                richText = true,
                fontSize = Title.FontSize > 0 ? Title.FontSize : EditorStyles.boldLabel.fontSize,
                fontStyle = Title.IsBold ? FontStyle.Bold : FontStyle.Normal
            };
            _styleInitialized = true;
        }

        private GUIContent GetCachedContent() {
            var titleText = $"<color=#{Title.TitleColorString}>{Title.Title}</color>";
            
            if (!ContentCache.TryGetValue(titleText, out var content)) {
                content = new GUIContent(titleText);
                ContentCache[titleText] = content;
            }
            
            return content;
        }

        private LayoutData CalculateLayout(Rect position, Vector2 titleSize) {
            var titleY = position.y;
            var lineY = titleY + EditorGUIUtility.singleLineHeight * 0.5f;
            
            var titleX = Title.AlignTitleLeft
                ? position.x
                : position.x + (position.width - titleSize.x) * 0.5f;

            var titleRect = new Rect(titleX, titleY, titleSize.x, EditorGUIUtility.singleLineHeight);
            var lineColor = GetCachedColor(Title.LineColor);

            return new LayoutData {
                TitleRect = titleRect,
                LineY = lineY,
                LineColor = lineColor,
                Position = position
            };
        }

        private void DrawTitle(Rect titleRect, GUIContent titleContent) {
            // Add shadow effect if enabled
            if (Title.HasShadow) {
                var shadowRect = new Rect(titleRect.x + 1, titleRect.y + 1, titleRect.width, titleRect.height);
                var shadowStyle = new GUIStyle(_cachedTitleStyle) {
                    normal = { textColor = Color.black * 0.5f }
                };
                EditorGUI.LabelField(shadowRect, titleContent, shadowStyle);
            }

            EditorGUI.LabelField(titleRect, titleContent, _cachedTitleStyle);
        }

        private void DrawLines(LayoutData layoutData) {
            if (!Title.ShowLine) return;

            var lineHeight = Title.LineHeight;
            var lineColor = layoutData.LineColor;

            if (Title.AlignTitleLeft) {
                DrawSingleLine(layoutData, lineHeight, lineColor);
            } else {
                DrawCenteredLines(layoutData, lineHeight, lineColor);
            }
        }

        private void DrawSingleLine(LayoutData layoutData, float lineHeight, Color lineColor) {
            var lineStartX = layoutData.TitleRect.xMax + LineOffsetFromTitle;
            var lineWidth = layoutData.Position.xMax - lineStartX;

            if (lineWidth > MinLineWidth) {
                var lineRect = new Rect(lineStartX, layoutData.LineY, lineWidth, lineHeight);
                DrawStyledLine(lineRect, lineColor);
            }
        }

        private void DrawCenteredLines(LayoutData layoutData, float lineHeight, Color lineColor) {
            var space = Title.LineSpacing;
            
            // Left line
            var leftLineStart = layoutData.Position.x;
            var leftLineEnd = layoutData.TitleRect.xMin - space;
            if (leftLineEnd > leftLineStart) {
                var leftLineRect = new Rect(leftLineStart, layoutData.LineY, leftLineEnd - leftLineStart, lineHeight);
                DrawStyledLine(leftLineRect, lineColor);
            }

            // Right line
            var rightLineStart = layoutData.TitleRect.xMax + space;
            var rightLineEnd = layoutData.Position.x + layoutData.Position.width;
            if (rightLineEnd > rightLineStart) {
                var rightLineRect = new Rect(rightLineStart, layoutData.LineY, rightLineEnd - rightLineStart, lineHeight);
                DrawStyledLine(rightLineRect, lineColor);
            }
        }

        private void DrawStyledLine(Rect lineRect, Color lineColor) {
            switch (Title.LineStyle) {
                case LineStyle.Solid:
                    EditorGUI.DrawRect(lineRect, lineColor);
                    break;
                case LineStyle.Dashed:
                    DrawDashedLine(lineRect, lineColor);
                    break;
                case LineStyle.Dotted:
                    DrawDottedLine(lineRect, lineColor);
                    break;
                case LineStyle.Gradient:
                    DrawGradientLine(lineRect, lineColor);
                    break;
            }
        }

        private static void DrawDashedLine(Rect lineRect, Color lineColor) {
            const float dashWidth = 4f;
            const float gapWidth = 2f;
            const float totalWidth = dashWidth + gapWidth;
            var dashCount = Mathf.FloorToInt(lineRect.width / totalWidth);

            for (var i = 0; i < dashCount; i++) {
                var dashRect = new Rect(
                    lineRect.x + i * totalWidth,
                    lineRect.y,
                    dashWidth,
                    lineRect.height
                );
                EditorGUI.DrawRect(dashRect, lineColor);
            }
        }

        private static void DrawDottedLine(Rect lineRect, Color lineColor) {
            const float dotSize = 2f;
            const float gapSize = 3f;
            const float totalWidth = dotSize + gapSize;
            var dotCount = Mathf.FloorToInt(lineRect.width / totalWidth);

            for (var i = 0; i < dotCount; i++) {
                var dotRect = new Rect(
                    lineRect.x + i * totalWidth,
                    lineRect.y + (lineRect.height - dotSize) * 0.5f,
                    dotSize,
                    dotSize
                );
                EditorGUI.DrawRect(dotRect, lineColor);
            }
        }

        private static void DrawGradientLine(Rect lineRect, Color lineColor) {
            var transparent = new Color(lineColor.r, lineColor.g, lineColor.b, 0f);
            var texture = CreateGradientTexture(lineColor, transparent);
            
            var oldColor = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(lineRect, texture);
            GUI.color = oldColor;
        }

        private static Texture2D CreateGradientTexture(Color startColor, Color endColor) {
            var texture = new Texture2D(256, 1);
            var colors = new Color[256];
            
            for (var i = 0; i < 256; i++) {
                var t = i / 255f;
                colors[i] = Color.Lerp(startColor, endColor, t);
            }
            
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }

        private static Color GetCachedColor(TitleColor colorEnum) {
            if (!ColorCache.TryGetValue(colorEnum, out var color)) {
                color = TitleAttribute.GetColor(colorEnum);
                ColorCache[colorEnum] = color;
            }
            return color;
        }
        #endregion
        
        #region Nested Types
        private struct LayoutData {
            public Rect TitleRect;
            public float LineY;
            public Color LineColor;
            public Rect Position;
        }
        #endregion
    }
}
#endif