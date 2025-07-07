#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Strix.Editor.Common {
    /// <summary>
    /// Formatted logging utility for debug output.
    /// Provides methods for logging information, warning, and server messages
    /// Supports highlighting the context, colors, and global toggle, and optional file logging
    /// </summary>
    public static class StrixLogger {
        private const string LoggerPrefKey = "Strix.Logging.Enabled";
        private const string LogInfoPrefKey = "Strix.Logging.Info";
        private const string LogWarnPrefKey = "Strix.Logging.Warn";
        private const string LogErrorPrefKey = "Strix.Logging.Error";
        private const string LogFilePath = "Assets/StrixLog.txt";

        /// <summary>
        /// Global toggle to enable or disable all logging
        /// </summary>
        private static bool LoggerEnabled { get; set; }
        private static bool LogInfoEnabled { get; set; }
        private static bool LogWarnEnabled { get; set; }
        private static bool LogErrorEnabled { get; set; }
        

        static StrixLogger() {
            LoggerEnabled = EditorPrefs.GetBool(LoggerPrefKey, true);
            LogInfoEnabled = EditorPrefs.GetBool(LogInfoPrefKey, true);
            LogWarnEnabled = EditorPrefs.GetBool(LogWarnPrefKey, true);
            LogErrorEnabled = EditorPrefs.GetBool(LogErrorPrefKey, true);
        }
        
        /// <summary>
        /// Logs an information message to the console with optional context
        /// </summary>
        public static void Log(string message, Object context = null) {
            if (!LoggerEnabled) return;
            if (!LogInfoEnabled) return;
            Debug.Log(Format(message, "white"), context);
        }

        /// <summary>
        /// Logs a warning message to the console with optional context
        /// </summary>
        public static void LogWarning(string message, Object context = null) {
            if (!LoggerEnabled) return;
            if (!LogWarnEnabled) return;
            Debug.LogWarning(Format(message, "yellow"), context);
        }

        /// <summary>
        /// Logs an error message to the console with optional context
        /// </summary>
        public static void LogError(string message, Object context = null) {
            if (!LoggerEnabled) return;
            if (!LogErrorEnabled) return;
            Debug.LogError(Format(message, "red"), context);
        }

        /// <summary>
        /// Logs a message to both the console and a file with timestamps
        /// </summary>
        public static void LogToFile(string message, LogType type = LogType.Log, Object context = null) {
            if (!LoggerEnabled) return;

            var color = type switch {
                LogType.Error => "red",
                LogType.Warning => "yellow",
                _ => "white"
            };

            var consoleMessage = Format(message, color);
            var fileMessage = FormatPlain(message, type);

            switch (type) {
                case LogType.Error:
                    Debug.LogError(consoleMessage, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(consoleMessage, context);
                    break;
                case LogType.Assert:
                case LogType.Log:
                case LogType.Exception:
                default:
                    Debug.Log(consoleMessage, context);
                    break;
            }

            WriteToFile(fileMessage);
        }

        /// <summary>
        /// Formats to the console with context and color
        /// </summary>
        private static string Format(string message, string color) {
            var frame = new StackTrace().GetFrame(2);
            var method = frame?.GetMethod();
            var className = method?.DeclaringType?.Name ?? "UnknownClass";
            var methodName = method?.Name ?? "UnknownMethod";
            var timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            
            return $"<color={color}>[{timestamp}] <b>[{className}::{methodName}]</b> {message}</color>";
        }
        
        /// <summary>
        /// Formats a text file for logging
        /// </summary>
        private static string FormatPlain(string message, LogType type) {
            var frame = new StackTrace().GetFrame(2);
            var method = frame?.GetMethod();
            var className = method?.DeclaringType?.Name ?? "UnknownClass";
            var methodName = method?.Name ?? "UnknownMethod";
            var timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");

            return $"[{timestamp}] [{type}] [{className}::{methodName}] {message}";
        }
        
        /// <summary>
        /// Writes a line to the Strix log file.
        /// </summary>
        private static void WriteToFile(string line) {
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);
                File.AppendAllText(LogFilePath, line + "\n");
            } catch (IOException ex) {
                Debug.LogError($"<b>[StrixLogger]</b> Failed to write to log file: {ex.Message}");
            }
        }
    }
}
#endif