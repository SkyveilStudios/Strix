using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    public enum ImageAlignment {
        Left,
        Center,
        Right
    }

    public enum ClickAction {
        None,
        SelectInProject,
        OpenInEditor,
        ShowInExplorer
    }

    /// <summary>
    /// Displays an image preview in the Inspector with extensive customization options
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ImagePreviewAttribute : PropertyAttribute {
        
        #region Properties
        public string Path { get; }
        public float Width { get; }
        public bool FullWidth { get; }
        public ImageAlignment Alignment { get; }
        public float Padding { get; }
        public bool ShowBorder { get; }
        public Color BorderColor { get; }
        public bool ShowShadow { get; }
        public bool ShowBackground { get; }
        public Color BackgroundColor { get; }
        public Color Tint { get; }
        public ScaleMode ScaleMode { get; }
        public bool ShowTooltip { get; }
        public string TooltipText { get; }
        public ClickAction ClickAction { get; }
        public bool ShowOverlay { get; }
        #endregion

        #region Constructors
        public ImagePreviewAttribute(string path, 
            float width = 200f, 
            bool fullWidth = false, 
            ImageAlignment alignment = ImageAlignment.Center,
            float padding = 4f,
            bool showBorder = false,
            float borderColorR = 0.7f, float borderColorG = 0.7f, float borderColorB = 0.7f,
            bool showShadow = false,
            bool showBackground = false,
            float backgroundColorR = 0.9f, float backgroundColorG = 0.9f, float backgroundColorB = 0.9f,
            float tintR = 1f, float tintG = 1f, float tintB = 1f, float tintA = 1f,
            ScaleMode scaleMode = ScaleMode.ScaleToFit,
            bool showTooltip = true,
            string tooltipText = null,
            ClickAction clickAction = ClickAction.SelectInProject,
            bool showOverlay = true) {
            
            Path = path;
            Width = width;
            FullWidth = fullWidth;
            Alignment = alignment;
            Padding = padding;
            ShowBorder = showBorder;
            BorderColor = new Color(borderColorR, borderColorG, borderColorB, 1f);
            ShowShadow = showShadow;
            ShowBackground = showBackground;
            BackgroundColor = new Color(backgroundColorR, backgroundColorG, backgroundColorB, 1f);
            Tint = new Color(tintR, tintG, tintB, tintA);
            ScaleMode = scaleMode;
            ShowTooltip = showTooltip;
            TooltipText = tooltipText;
            ClickAction = clickAction;
            ShowOverlay = showOverlay;
        }

        // Simplified constructor for common use cases
        public ImagePreviewAttribute(string path, float width = 200f, bool fullWidth = false) 
            : this(path, width, fullWidth, ImageAlignment.Center) { }

        // Constructor with basic styling
        public ImagePreviewAttribute(string path, float width, ImageAlignment alignment, bool showBorder = false) 
            : this(path, width, false, alignment, 4f, showBorder) { }
        #endregion
    }
}