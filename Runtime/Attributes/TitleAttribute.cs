#if  UNITY_EDITOR
using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    public enum TitleColor {
        Gray, Red, Green, Blue, Yellow, 
        Cyan, Magenta, White, Black
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class TitleAttribute : PropertyAttribute {
        private const TitleColor DefaultTitleColor = TitleColor.Cyan;
        private const TitleColor DefaultLineColor = TitleColor.Blue;
        private const float DefaultLineHeight = 2f;

        public string Title { get; }
        private TitleColor TitleColor { get; }
        private TitleColor LineColor { get; }
        public string TitleColorString { get; }
        public string LineColorString { get; }
        public float LineHeight { get; }
        public float Spacing { get; }
        public bool AlignTitleLeft { get; }

        public TitleAttribute(string title = "",
            TitleColor titleColor = DefaultTitleColor,
            TitleColor lineColor = DefaultLineColor,
            float lineHeight = DefaultLineHeight,
            float spacing = 1f,
            bool alignTitleLeft = false) {
            Title = title;
            TitleColor = titleColor;
            LineColor = lineColor;
            TitleColorString = ColorUtility.ToHtmlStringRGB(GetColor(TitleColor));
            LineColorString = ColorUtility.ToHtmlStringRGB(GetColor(LineColor));
            LineHeight = Mathf.Max(1f, lineHeight);
            Spacing = spacing;
            AlignTitleLeft = alignTitleLeft;
        }

        private static Color GetColor(TitleColor color) {
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
                _ => Color.white
            };
        }
    }
}
#endif