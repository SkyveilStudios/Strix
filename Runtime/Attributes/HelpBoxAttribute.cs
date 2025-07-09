using System;
using UnityEditor;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class HelpBoxAttribute : PropertyAttribute {
        public string Message { get; }
        public MessageType Type { get; }

        public HelpBoxAttribute(string message, MessageType type = MessageType.Info) {
            Message = message;
            Type = type;
        }
    }
}