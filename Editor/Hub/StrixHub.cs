using System;
using Strix.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Hub {
    public class StrixHub : EditorWindow {
        private enum Tab {
            Main,
            Attributes,
            Components
        }

        public const string AutoOpenKey = "Strix.Hub.AutoOpen";
        private Tab _currentTab =  Tab.Main;
        private Vector2 _scroll;

        [MenuItem("Strix/Strix Hub", priority = 1)]
        public static void ShowWindow() {
            var window = CreateInstance<StrixHub>();
            window.titleContent = new GUIContent("Strix Hub");
            window.minSize = new Vector2(700, 600);
            window.ShowUtility();
            
            const string checkUpdatesKey = "Strix.Hub.CheckForUpdates";
            if (EditorPrefs.GetBool(checkUpdatesKey, true)) {
                StrixVersionChecker.CheckForUpdateFromHub(showIfUpToDate: false);
            }
        }

        [InitializeOnLoadMethod]
        private static void AutoShow() {
            if (EditorPrefs.GetBool(AutoOpenKey, true)) {
                EditorApplication.update += OpenOnStartup;
            }
        }

        private static void OpenOnStartup() {
            EditorApplication.update -= OpenOnStartup;
            if (!Application.isPlaying) ShowWindow();
        }

        private void OnGUI() {
            DrawHeaderBar();
            InspectorImageUtility.DrawImage(
                assetPath: "Assets/Strix/Banners/StrixBanner.jpg",
                width: 1080f,
                fullWidth: true,
                alignment: ImageAlignment.Center
            );
            DrawDescriptionBar();
            DrawTabs();
            
            EditorGUILayout.Space();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            switch (_currentTab) {
                case Tab.Main:
                    HubMainTab.DrawMainTab();
                    break;
                case Tab.Attributes:
                    HubAttributesTab.DrawAttributesTab();
                    break;
                case Tab.Components:
                    HubComponentsTab.DrawComponentsTab();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EditorGUILayout.EndScrollView();
            DrawFooterBar();
        }
        
        private static void DrawHeaderBar()
        {
            var rect = GUILayoutUtility.GetRect(0, 24, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            const float padding = 10f;

            var labelStyle = EditorStyles.boldLabel;
            var labelHeight = labelStyle.CalcHeight(new GUIContent("Test"), 1000f);
            var verticalOffset = (rect.height - labelHeight) * 0.5f;
            
            var leftSize = labelStyle.CalcSize(new GUIContent("Copyright © 2025 SkyveilStudios. All rights reserved."));
            var leftRect = new Rect(rect.x + padding, rect.y + verticalOffset, leftSize.x, labelHeight);
            GUI.Label(leftRect, "Copyright © 2025 SkyveilStudios. All rights reserved.", labelStyle);
            
            var versionSize = labelStyle.CalcSize(new GUIContent("Version " + StrixVersionInfo.CurrentVersion));
            var rightRect = new Rect(rect.xMax - versionSize.x - padding, rect.y + verticalOffset, versionSize.x, labelHeight);
            GUI.Label(rightRect, $"Version {StrixVersionInfo.CurrentVersion}", labelStyle);
        }

        private static void DrawDescriptionBar()
        {
            var rect = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
            
            var titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true
            };

            var bodyStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
            };
            
            const string title = "👋 Welcome to Strix by SkyveilStudios!";
            const string body = "This is your central hub for accessing Strix Editor Tools.";

            const float padding = 10f;

            var titleRect = new Rect(rect.x + padding, rect.y + 8, rect.width - 2 * padding, 20);
            var bodyRect = new Rect(rect.x + padding, rect.y + 30, rect.width - 2 * padding, 48);

            GUI.Label(titleRect, title, titleStyle);
            GUI.Label(bodyRect, body, bodyStyle);
        }
        
        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Toggle(_currentTab == Tab.Main, "Main", EditorStyles.toolbarButton))
                _currentTab = Tab.Main;

            if (GUILayout.Toggle(_currentTab == Tab.Attributes, "Attributes", EditorStyles.toolbarButton))
                _currentTab = Tab.Attributes;
            
            if (GUILayout.Toggle(_currentTab == Tab.Components, "Components", EditorStyles.toolbarButton))
                _currentTab = Tab.Components;

            EditorGUILayout.EndHorizontal();
        }
        
        private static void DrawFooterBar()
        {
            var rect = GUILayoutUtility.GetRect(0, 32, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            const float padding = 10f;
            const float buttonWidth = 110f;
            const float buttonHeight = 20f;
            const float spacing = 8f;

            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = buttonHeight
            };

            const float totalWidth = buttonWidth * 2 + spacing;
            var startX = rect.x + padding + (rect.width - totalWidth - 2 * padding) / 2f;
            var y = rect.y + (rect.height - buttonHeight) / 2f;

            var docRect = new Rect(startX, y, buttonWidth, buttonHeight);
            var discordRect = new Rect(startX + buttonWidth + spacing, y, buttonWidth, buttonHeight);

            if (GUI.Button(docRect, "📖 Github", buttonStyle))
                Application.OpenURL("https://github.com/SkyveilStudios/Strix/");

            if (GUI.Button(discordRect, "💬 Discord", buttonStyle))
                Application.OpenURL("https://discord.gg/XkRh7CEccy");
        }
    }
}