#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Strix.Editor.Common {
    /// <summary>
    /// Formatted logging utility for debug output.
    /// Provides methods for logging information, warning, and server messages
    /// </summary>
    public static class StrixLogger {
        private const string EditorPrefKey = "Strix.Logging.Enabled";

        /// <summary>
        /// Global toggle to enable or disable all logging
        /// </summary>
        private static bool Enabled { get; set; }

        static StrixLogger() {
            Enabled = EditorPrefs.GetBool(EditorPrefKey, true);
        }
        
        /// <summary>
        /// Logs an information message to the console
        /// </summary>
        public static void Log(string message, Object context = null) {
            if (!Enabled) return;
            Debug.Log(Format(message, "white"), context);
        }

        /// <summary>
        /// Logs a warning message to the console
        /// </summary>
        public static void LogWarning(string message, Object context = null) {
            if (!Enabled) return;
            Debug.LogWarning(Format(message, "yellow"), context);
        }

        /// <summary>
        /// Logs an error message to the console
        /// </summary>
        public static void LogError(string message, Object context = null) {
            if (!Enabled) return;
            Debug.LogError(Format(message, "red"), context);
        }

        /// <summary>
        /// Formats the message by adding the calling class and method name.
        /// </summary>
        /// <param name="message">The log message content.</param>
        /// <returns>A formatted string including the script and method as context.</returns>
        private static string Format(string message, string color) {
            var frame = new StackTrace().GetFrame(2);
            var method = frame?.GetMethod();
            var className = method?.DeclaringType?.Name ?? "UnknownClass";
            var methodName = method?.Name ?? "UnknownMethod";
            return $"<color={color}><b>[{className}::{methodName}]</b> {message}</color>";
        }

        /// <summary>
        /// Unity Menu Integration
        /// </summary>
        [MenuItem("Strix/Logging/Enable Logging", priority = 100)]
        private static void ToggleLogging() {
            Enabled = !Enabled;
            EditorPrefs.SetBool(EditorPrefKey, Enabled);
        }

        [MenuItem("Strix/Logging/Enable Logging", true)]
        private static bool ToggleLoggingValidate() {
            Menu.SetChecked("Strix/Logging/Enable Logging", Enabled);
            return true;
        }
    }
}
#endif