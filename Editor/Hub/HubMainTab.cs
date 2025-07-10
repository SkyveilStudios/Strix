using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Hub {
    public static class HubMainTab {
        public static void DrawMainTab()
        {
            EditorGUILayout.BeginHorizontal();
            
            var halfWidth = EditorGUIUtility.currentViewWidth / 1.7f;
            
            EditorGUILayout.BeginVertical("box", GUILayout.Width(halfWidth), GUILayout.ExpandHeight(true));
            DrawToolShortcuts();
            EditorGUILayout.EndVertical();

            DrawVerticalSeparator();
            
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            DrawSettings();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        
        private static void DrawToolShortcuts()
        {
            EditorGUILayout.LabelField("🛠 Tools", EditorStyles.boldLabel);
            if (GUILayout.Button("Open Project Stats")) EditorApplication.ExecuteMenuItem("Strix/Project Stats");
            if (GUILayout.Button("Open Notepad")) EditorApplication.ExecuteMenuItem("Strix/Notepad/Window");
            if (GUILayout.Button("Open Task Board")) EditorApplication.ExecuteMenuItem("Strix/Task Board");
            if (GUILayout.Button("Open Missing Script Finder")) EditorApplication.ExecuteMenuItem("Strix/Missing Script Finder");
            if (GUILayout.Button("Open Icon Browser")) EditorApplication.ExecuteMenuItem("Strix/Icon Browser");
        }

        private static void DrawSettings() {
            EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
            
            // Show on startup
            var autoOpen = EditorPrefs.GetBool(StrixHub.AutoOpenKey, true);
            var newAutoOpen = EditorGUILayout.ToggleLeft("Show Hub On Startup", autoOpen);
            if (newAutoOpen != autoOpen) EditorPrefs.SetBool(StrixHub.AutoOpenKey, newAutoOpen);
            
            EditorGUILayout.Space(1);

            // Version Checking
            const string checkUpdatesKey = "Strix.Hub.CheckForUpdates";
            var checkUpdates = EditorPrefs.GetBool(checkUpdatesKey, true);
            var newCheckUpdates = EditorGUILayout.ToggleLeft("Check for Updates On Startup", checkUpdates);
            if (newCheckUpdates != checkUpdates) EditorPrefs.SetBool(checkUpdatesKey, newCheckUpdates);

            EditorGUI.indentLevel++;
            if (GUILayout.Button("Check For Updates")) {
                StrixVersionChecker.CheckForUpdateFromHub(showIfUpToDate: true);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(1);
            
            // Hierarchy Trees
            EditorGUILayout.LabelField("Hierarchy:", EditorStyles.boldLabel);
            var hierarchyLineRect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(hierarchyLineRect, new Color(0.3f, 0.3f, 0.3f, 1f));
            EditorGUILayout.Space(4);
            
            const string hierarchyLinesKey = "Strix.Hierarchy.ShowLines";
            var showLines = EditorPrefs.GetBool(hierarchyLinesKey, true);
            var newShowLines = EditorGUILayout.ToggleLeft("Show Hierarchy Lines", showLines);
            if (newShowLines != showLines) EditorPrefs.SetBool(hierarchyLinesKey, newShowLines);
            
            EditorGUILayout.Space(1);
            
            EditorGUI.BeginDisabledGroup(!newShowLines);
            var currentStyle = EditorPrefs.GetInt("Strix.Hierarchy.LineStyle", 0);
            var styleOptions = new[] { "Solid", "Dotted", "Dashed" };
            var selected = EditorGUILayout.Popup("Line Style", currentStyle, styleOptions);

            if (selected != currentStyle)
                EditorPrefs.SetInt("Strix.Hierarchy.LineStyle", selected);
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space(1);


            // Logging
            EditorGUILayout.LabelField("Logs:", EditorStyles.boldLabel);
            var lineRect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(lineRect, new Color(0.3f, 0.3f, 0.3f, 1f));
            EditorGUILayout.Space(4);
            
            const string loggerPrefKey = "Strix.Logging.Enabled";
            var loggerEnabled = EditorPrefs.GetBool(loggerPrefKey, true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Enable StrixLogger", GUILayout.Width(146));
            var newLoggerEnabled = GUILayout.Toggle(loggerEnabled, "", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            if (newLoggerEnabled != loggerEnabled) EditorPrefs.SetBool(loggerPrefKey, newLoggerEnabled);

            if (!newLoggerEnabled) return;
            EditorGUI.indentLevel++;
                
            DrawLogToggle("Info Logs", "Strix.Logging.Info");
            DrawLogToggle("Warning Logs", "Strix.Logging.Warn");
            DrawLogToggle("Error Logs", "Strix.Logging.Error");
                
            EditorGUI.indentLevel--;
        }

        private static void DrawLogToggle(string label, string prefKey) {
            var current = EditorPrefs.GetBool(prefKey, true);
            var updated = EditorGUILayout.Toggle(label, current);
            if (updated != current) EditorPrefs.SetBool(prefKey, updated);
        }
        
        private static void DrawVerticalSeparator()
        {
            var rect = GUILayoutUtility.GetRect(4,4, GUILayout.Width(4), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
        }
    }
}