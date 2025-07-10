using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_2020_1_OR_NEWER
using Unity.Profiling;
#endif

namespace Strix.Editor.Hub {
    public static class HubMainTab {
        #region Constants
        private const float HALF_WIDTH_RATIO = 1.7f;
        private const float SECTION_SPACING = 8f;
        private const float SUBSECTION_SPACING = 4f;
        private const float SEPARATOR_HEIGHT = 1f;
        private const float BUTTON_WIDTH = 80f;
        private const float LABEL_WIDTH = 146f;
        
        // Preference Keys
        private const string CHECK_UPDATES_KEY = "Strix.Hub.CheckForUpdates";
        private const string HIERARCHY_LINES_KEY = "Strix.Hierarchy.ShowLines";
        private const string HIERARCHY_BUTTONS_KEY = "Strix.Hierarchy.ShowButtons";
        private const string LOGGER_ENABLED_KEY = "Strix.Logging.Enabled";
        
        // Icon Keys
        private const string ICONS_ENABLED_KEY = "StrixHierarchy.Enabled";
        private const string CUSTOM_ICONS_KEY = "StrixHierarchy.ShowCustomIcons";
        private const string BUILTIN_ICONS_KEY = "StrixHierarchy.ShowBuiltInIcons";
        
        // Style Keys
        private const string LINE_STYLE_KEY = "Strix.Hierarchy.LineStyle";
        private const string LINE_OPACITY_KEY = "Strix.Hierarchy.LineOpacity";
        #endregion

        #region Cached Values
        private static int? _cachedLockedCount;
        private static double _lastLockedCountCheck;
        private const double LOCKED_COUNT_CACHE_DURATION = 0.5; // seconds
        
        private static GUIStyle _sectionHeaderStyle;
        private static GUIStyle _miniLabelCentered;
        private static Color _separatorColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        #endregion

        #region Profiler Markers
        #if UNITY_2020_1_OR_NEWER
        private static readonly ProfilerMarker DrawMainTabMarker = new ProfilerMarker("HubMainTab.DrawMainTab");
        private static readonly ProfilerMarker DrawSettingsMarker = new ProfilerMarker("HubMainTab.DrawSettings");
        #endif
        #endregion

        #region Main Drawing Methods
        public static void DrawMainTab() {
            #if UNITY_2020_1_OR_NEWER
            using (DrawMainTabMarker.Auto())
            #endif
            {
                InitializeStyles();
                
                EditorGUILayout.BeginHorizontal();
                
                var halfWidth = EditorGUIUtility.currentViewWidth / HALF_WIDTH_RATIO;
                
                // Tools Panel
                EditorGUILayout.BeginVertical("box", GUILayout.Width(halfWidth), GUILayout.ExpandHeight(true));
                DrawToolShortcuts();
                EditorGUILayout.EndVertical();

                DrawVerticalSeparator();
                
                // Settings Panel
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
                DrawSettings();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private static void DrawToolShortcuts() {
            DrawSectionHeader("🛠 Tools");
            
            var tools = new[] {
                ("Project Stats", "Strix/Project Stats"),
                ("Notepad", "Strix/Notepad/Window"),
                ("Task Board", "Strix/Task Board"),
                ("Missing Script Finder", "Strix/Missing Script Finder"),
                ("Icon Browser", "Strix/Icon Browser")
            };
            
            foreach (var (name, menuPath) in tools) {
                if (GUILayout.Button($"Open {name}")) {
                    EditorApplication.ExecuteMenuItem(menuPath);
                }
            }
            
            GUILayout.FlexibleSpace();
        }

        private static void DrawSettings() {
            #if UNITY_2020_1_OR_NEWER
            using (DrawSettingsMarker.Auto())
            #endif
            {
                DrawSectionHeader("⚙️ Settings");
                
                // General Settings
                DrawGeneralSettings();
                
                EditorGUILayout.Space(SECTION_SPACING);
                
                // Hierarchy Settings
                DrawHierarchySection();
                
                EditorGUILayout.Space(SECTION_SPACING);
                
                // Logging Settings
                DrawLoggingSection();
                
                GUILayout.FlexibleSpace();
            }
        }
        #endregion

        #region Settings Sections
        private static void DrawGeneralSettings() {
            // Show on startup
            DrawToggleSetting("Show Hub On Startup", StrixHub.AutoOpenKey, true);
            
            EditorGUILayout.Space(2);

            // Version Checking
            DrawToggleSetting("Check for Updates On Startup", CHECK_UPDATES_KEY, true);

            EditorGUI.indentLevel++;
            if (GUILayout.Button("Check For Updates Now")) {
                StrixVersionChecker.CheckForUpdateFromHub(showIfUpToDate: true);
            }
            EditorGUI.indentLevel--;
        }

        private static void DrawHierarchySection() {
            DrawSectionHeader("🌳 Hierarchy");
            DrawSeparator();
            EditorGUILayout.Space(SUBSECTION_SPACING);
            
            // Icons
            DrawHierarchyIconsSettings();
            
            EditorGUILayout.Space(SUBSECTION_SPACING);
            
            // Tree Lines
            DrawHierarchyTreeSettings();
            
            EditorGUILayout.Space(SUBSECTION_SPACING);
            
            // Buttons
            DrawHierarchyButtonsSettings();
            
            EditorGUILayout.Space(SUBSECTION_SPACING);
            
            // Global Actions
            DrawHierarchyActions();
        }

        private static void DrawLoggingSection() {
            DrawSectionHeader("📝 Logs");
            DrawSeparator();
            EditorGUILayout.Space(SUBSECTION_SPACING);
            
            // Main toggle with custom layout
            var loggerEnabled = EditorPrefs.GetBool(LOGGER_ENABLED_KEY, true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Enable StrixLogger", GUILayout.Width(LABEL_WIDTH));
            var newLoggerEnabled = GUILayout.Toggle(loggerEnabled, "", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            
            if (newLoggerEnabled != loggerEnabled) {
                EditorPrefs.SetBool(LOGGER_ENABLED_KEY, newLoggerEnabled);
            }

            if (!newLoggerEnabled) return;
            
            EditorGUI.indentLevel++;
            DrawLogToggle("Info Logs", "Strix.Logging.Info");
            DrawLogToggle("Warning Logs", "Strix.Logging.Warn");
            DrawLogToggle("Error Logs", "Strix.Logging.Error");
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Hierarchy Settings
        private static void DrawHierarchyIconsSettings() {
            var iconsEnabled = DrawToggleSetting("Show Icons", ICONS_ENABLED_KEY, true);
            
            if (!iconsEnabled) return;
            
            EditorGUI.indentLevel++;
            DrawToggle("Custom Component Icons", CUSTOM_ICONS_KEY, true);
            DrawToggle("Built-in Component Icons", BUILTIN_ICONS_KEY, true);
            EditorGUI.indentLevel--;
        }

        private static void DrawHierarchyTreeSettings() {
            var showLines = DrawToggleSetting("Show Tree Lines", HIERARCHY_LINES_KEY, true);
            
            if (!showLines) return;
            
            EditorGUI.indentLevel++;
            
            // Line Style
            var currentStyle = EditorPrefs.GetInt(LINE_STYLE_KEY, 0);
            var styleOptions = new[] { 
                "Solid", 
                "Dotted", 
                "Dashed", 
                "Double Line",
                "Minimal",
                "Heavy",
                "Zigzag"
            };
            var selected = EditorGUILayout.Popup("Line Style", currentStyle, styleOptions);
            if (selected != currentStyle) {
                EditorPrefs.SetInt(LINE_STYLE_KEY, selected);
                EditorApplication.RepaintHierarchyWindow();
            }
            
            // Line Opacity with live preview
            var currentOpacity = EditorPrefs.GetFloat(LINE_OPACITY_KEY, 0.25f);
            EditorGUILayout.BeginHorizontal();
            var newOpacity = EditorGUILayout.Slider("Line Opacity", currentOpacity, 0.1f, 1f);
            
            // Preview rect
            var previewRect = GUILayoutUtility.GetRect(40, EditorGUIUtility.singleLineHeight);
            DrawLinePreview(previewRect, (Hierarchy.StrixHierarchyConnector.LineStyle)selected, newOpacity);
            EditorGUILayout.EndHorizontal();
            
            if (Mathf.Abs(newOpacity - currentOpacity) > 0.01f) {
                EditorPrefs.SetFloat(LINE_OPACITY_KEY, newOpacity);
                EditorApplication.RepaintHierarchyWindow();
            }
            
            EditorGUI.indentLevel--;
        }

        private static void DrawHierarchyButtonsSettings() {
            var showButtons = DrawToggleSetting("Show Action Buttons", HIERARCHY_BUTTONS_KEY, true);
            
            if (!showButtons) return;
            
            EditorGUI.indentLevel++;
            
            // Description
            EditorGUILayout.LabelField("Adds lock/visibility buttons to hierarchy items", _miniLabelCentered);
            
            // Locked objects count with caching
            var lockedCount = GetCachedLockedObjectsCount();
            if (lockedCount > 0) {
                var message = $"🔒 {lockedCount} object{(lockedCount > 1 ? "s" : "")} locked";
                EditorGUILayout.LabelField(message, EditorStyles.miniLabel);
                
                if (GUILayout.Button("Unlock All", GUILayout.Width(BUTTON_WIDTH))) {
                    #if UNITY_EDITOR
                    Hierarchy.StrixHierarchyButtons.UnlockAll();
                    InvalidateLockedCountCache();
                    #endif
                }
            }
            
            EditorGUI.indentLevel--;
        }

        private static void DrawHierarchyActions() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Refresh Hierarchy", GUILayout.Width(120))) {
                #if UNITY_EDITOR
                Hierarchy.StrixHierarchyIcons.ClearAllCaches();
                Hierarchy.StrixHierarchyButtons.ClearAllCaches();
                Hierarchy.StrixHierarchyConnector.ClearAllCaches();
                InvalidateLockedCountCache();
                EditorApplication.RepaintHierarchyWindow();
                SceneView.RepaintAll();
                #endif
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Helper Methods
        private static void InitializeStyles() {
            if (_sectionHeaderStyle == null) {
                _sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel) {
                    fontSize = 12,
                    fixedHeight = 20
                };
            }
            
            if (_miniLabelCentered == null) {
                _miniLabelCentered = new GUIStyle(EditorStyles.miniLabel) {
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }

        private static void DrawSectionHeader(string title) {
            EditorGUILayout.LabelField(title, _sectionHeaderStyle);
        }

        private static void DrawSeparator() {
            var rect = GUILayoutUtility.GetRect(1, SEPARATOR_HEIGHT, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, _separatorColor);
        }

        private static void DrawVerticalSeparator() {
            var rect = GUILayoutUtility.GetRect(4, 4, GUILayout.Width(4), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(rect, _separatorColor);
        }

        private static bool DrawToggleSetting(string label, string prefKey, bool defaultValue) {
            var current = EditorPrefs.GetBool(prefKey, defaultValue);
            var newValue = EditorGUILayout.ToggleLeft(label, current);
            if (newValue != current) {
                EditorPrefs.SetBool(prefKey, newValue);
            }
            return newValue;
        }

        private static void DrawToggle(string label, string prefKey, bool defaultValue) {
            var current = EditorPrefs.GetBool(prefKey, defaultValue);
            var updated = EditorGUILayout.Toggle(label, current);
            if (updated != current) {
                EditorPrefs.SetBool(prefKey, updated);
            }
        }

        private static void DrawLogToggle(string label, string prefKey) {
            DrawToggle(label, prefKey, true);
        }

        private static void DrawLinePreview(Rect rect, Hierarchy.StrixHierarchyConnector.LineStyle style, float opacity) {
            // Draw background
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.3f));
            
            var color = new Color(1f, 1f, 1f, opacity);
            var oldColor = Handles.color;
            Handles.color = color;
            
            var padding = 4f;
            var start = new Vector3(rect.x + padding, rect.center.y);
            var end = new Vector3(rect.xMax - padding, rect.center.y);
            
            // Draw preview based on style
            switch (style) {
                case Hierarchy.StrixHierarchyConnector.LineStyle.Solid:
                    Handles.DrawLine(start, end);
                    break;
                    
                case Hierarchy.StrixHierarchyConnector.LineStyle.Dotted:
                    // Draw actual dots
                    var dotCount = 5;
                    for (int i = 0; i < dotCount; i++) {
                        var t = i / (float)(dotCount - 1);
                        var pos = Vector3.Lerp(start, end, t);
                        var dotRect = new Rect(pos.x - 1, pos.y - 1, 2, 2);
                        EditorGUI.DrawRect(dotRect, color);
                    }
                    break;
                    
                case Hierarchy.StrixHierarchyConnector.LineStyle.Dashed:
                    Handles.DrawDottedLine(start, end, 6f);
                    break;
                    
                case Hierarchy.StrixHierarchyConnector.LineStyle.DoubleLine:
                    var offset = new Vector3(0, 2);
                    Handles.DrawLine(start - offset, end - offset);
                    Handles.DrawLine(start + offset, end + offset);
                    break;
                    
                case Hierarchy.StrixHierarchyConnector.LineStyle.Minimal:
                    // Just a thin line
                    Handles.DrawLine(start, end);
                    break;
                    
                case Hierarchy.StrixHierarchyConnector.LineStyle.Heavy:
                    // Draw multiple lines for thickness
                    for (int i = -2; i <= 2; i++) {
                        Handles.DrawLine(start + new Vector3(0, i * 0.5f), end + new Vector3(0, i * 0.5f));
                    }
                    break;
                    
                case Hierarchy.StrixHierarchyConnector.LineStyle.Zigzag:
                    // Simple zigzag preview
                    var points = new Vector3[5];
                    for (int i = 0; i < 5; i++) {
                        var t = i / 4f;
                        var x = Mathf.Lerp(start.x, end.x, t);
                        var y = rect.center.y + (i % 2 == 0 ? -2 : 2);
                        points[i] = new Vector3(x, y);
                    }
                    for (int i = 0; i < 4; i++) {
                        Handles.DrawLine(points[i], points[i + 1]);
                    }
                    break;
            }
            
            Handles.color = oldColor;
        }

        private static int GetCachedLockedObjectsCount() {
            var currentTime = EditorApplication.timeSinceStartup;
            
            if (_cachedLockedCount.HasValue && 
                currentTime - _lastLockedCountCheck < LOCKED_COUNT_CACHE_DURATION) {
                return _cachedLockedCount.Value;
            }
            
            #if UNITY_EDITOR
            var count = 0;
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in allObjects) {
                if (obj != null && Hierarchy.StrixHierarchyButtons.IsLocked(obj)) {
                    count++;
                }
            }
            
            _cachedLockedCount = count;
            _lastLockedCountCheck = currentTime;
            return count;
            #else
            return 0;
            #endif
        }

        private static void InvalidateLockedCountCache() {
            _cachedLockedCount = null;
        }
        #endregion
    }
}