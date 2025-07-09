using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class HelpBoxAttribute : PropertyAttribute {
        public string Message { get; }
        public HelpBoxType Type { get; }

        public HelpBoxAttribute(string message, HelpBoxType type = HelpBoxType.Info) {
            Message = message;
            Type = type;
        }
    }

    public enum HelpBoxType {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}