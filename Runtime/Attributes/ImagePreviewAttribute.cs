using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    public enum ImageAlignment {
        Left,
        Center,
        Right
    }
    
    /// <summary>
    /// Displays an image on the component using the file path and size
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ImagePreviewAttribute : PropertyAttribute {
        public string Path { get; }
        public float Width { get; }
        public bool FullWidth { get; }
        public ImageAlignment Alignment { get; }

        public ImagePreviewAttribute(string path, float width, bool fullWidth = false, ImageAlignment alignment = ImageAlignment.Center) {
            Path = path;
            Width = width;
            FullWidth = fullWidth;
            Alignment = alignment;
        }
    }
}