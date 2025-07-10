using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    public enum TitleColor {
        Gray, Red, Green, Blue, Yellow,
        Cyan, Magenta, White, Black,
        Orange, Purple, Pink, Brown,
        LightGray, DarkGray, LightBlue,
        DarkBlue, LightGreen, DarkGreen,
        LightRed, DarkRed
    }
    
    public enum LineStyle {
        Solid,
        Dashed,
        Dotted,
        Gradient
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class TitleAttribute : PropertyAttribute {
        #region Constants
        private const TitleColor DefaultTitleColor = TitleColor.Cyan;
        private const TitleColor DefaultLineColor = TitleColor.Blue;
        private const float DefaultLineHeight = 2f;
        private const float DefaultSpacing = 1f;
        private const float DefaultLineSpacing = 10f;
        private const int DefaultFontSize = 0;
        #endregion
        
        #region Properties
        public string Title { get; }
        public TitleColor TitleColor { get; }
        public TitleColor LineColor { get; }
        public string TitleColorString { get; }
        public string LineColorString { get; }
        public float LineHeight { get; }
        public float Spacing { get; }
        public float LineSpacing { get; }
        public bool AlignTitleLeft { get; }
        public bool ShowLine { get; }
        public LineStyle LineStyle { get; }
        public int FontSize { get; }
        public bool IsBold { get; }
        public bool HasShadow { get; }
        #endregion

        #region Constructors
        public TitleAttribute(string title = "",
            TitleColor titleColor = DefaultTitleColor,
            TitleColor lineColor = DefaultLineColor,
            float lineHeight = DefaultLineHeight,
            float spacing = DefaultSpacing,
            bool alignTitleLeft = false,
            bool showLine = true,
            LineStyle lineStyle = LineStyle.Solid,
            int fontSize = DefaultFontSize,
            bool isBold = true,
            bool hasShadow = false,
            float lineSpacing = DefaultLineSpacing) {
            
            Title = title;
            TitleColor = titleColor;
            LineColor = lineColor;
            TitleColorString = ColorUtility.ToHtmlStringRGB(GetColor(TitleColor));
            LineColorString = ColorUtility.ToHtmlStringRGB(GetColor(LineColor));
            LineHeight = Mathf.Max(1f, lineHeight);
            Spacing = spacing;
            LineSpacing = lineSpacing;
            AlignTitleLeft = alignTitleLeft;
            ShowLine = showLine;
            LineStyle = lineStyle;
            FontSize = fontSize;
            IsBold = isBold;
            HasShadow = hasShadow;
        }
        #endregion

        #region Public Methods
        public static Color GetColor(TitleColor color) {
            return color switch {
                TitleColor.Gray => Color.gray,
                TitleColor.Red => Color.red,
                TitleColor.Green => Color.green,
                TitleColor.Blue => Color.blue,
                TitleColor.Yellow => Color.yellow,
                TitleColor.Cyan => Color.cyan,
                TitleColor.Magenta => Color.magenta,
                TitleColor.White => Color.white,
                TitleColor.Black => Color.black,
                TitleColor.Orange => new Color(1f, 0.5f, 0f),
                TitleColor.Purple => new Color(0.5f, 0f, 1f),
                TitleColor.Pink => new Color(1f, 0.75f, 0.8f),
                TitleColor.Brown => new Color(0.6f, 0.3f, 0f),
                TitleColor.LightGray => new Color(0.8f, 0.8f, 0.8f),
                TitleColor.DarkGray => new Color(0.3f, 0.3f, 0.3f),
                TitleColor.LightBlue => new Color(0.5f, 0.8f, 1f),
                TitleColor.DarkBlue => new Color(0f, 0f, 0.5f),
                TitleColor.LightGreen => new Color(0.5f, 1f, 0.5f),
                TitleColor.DarkGreen => new Color(0f, 0.5f, 0f),
                TitleColor.LightRed => new Color(1f, 0.5f, 0.5f),
                TitleColor.DarkRed => new Color(0.5f, 0f, 0f),
                _ => Color.white
            };
        }
        #endregion
    }
}