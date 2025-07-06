using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Strix.Editor.Common {
    /// <summary>
    /// Formatted logging utility for debug output.
    /// Provides methods for logging information, warning, and server messages
    /// </summary>
    public static class StrixLogger {
        #if UNITY_EDITOR
        /// <summary>
        /// Logs an information message to the console
        /// </summary>
        public static void Log(string message) {
            Debug.Log(Format(message));
        }

        /// <summary>
        /// Logs a warning message to the console
        /// </summary>
        public static void LogWarning(string message) {
            Debug.LogWarning(Format(message));
        }

        /// <summary>
        /// Logs an error message to the console
        /// </summary>
        public static void LogError(string message) {
            Debug.LogError(Format(message));
        }

        /// <summary>
        /// Formats the message by adding the calling class and method name.
        /// </summary>
        /// <param name="message">The log message content.</param>
        /// <returns>A formatted string including the script and method as context.</returns>
        private static string Format(string message) {
            var frame = new StackTrace().GetFrame(2);
            var method = frame?.GetMethod();
            var className = method?.DeclaringType?.Name ?? "UnknownClass";
            var methodName = method?.Name ?? "UnknownMethod";
            return $"<b>[{className}::{methodName}]</b> {message}";
        }
        #endif
    }
}