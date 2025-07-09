using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    public enum ProgressBarColor {
        Red, Green, Blue, Yellow, Cyan, Magenta, Gray, White
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class ProgressBarAttribute : PropertyAttribute {
        public string Label { get; }
        public float Min { get; }
        public float Max { get; }
        public ProgressBarColor BarColor { get; }
        public bool IsInteractable { get; set; }

        public ProgressBarAttribute(string label, float min, float max, ProgressBarColor color = ProgressBarColor.Green) {
            Label = label;
            Min = min;
            Max = max;
            BarColor = color;
            IsInteractable = false;
        }
    }
}