using Strix.Editor.Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.IconBrowser {
    /// <summary>
    /// Editor window that displays Unity's built-in icons in a searchable grid.
    /// Allows developers to preview and copy icon names for use in custom tooling.
    /// </summary>
    public class IconBrowser : EditorWindow {
        private const string WindowTitle = "Icon Browser";
        private const float IconSize = 64f;
        private const float IconLabelWidth = 96f;
        private const float Padding = 12f;

        private Vector2 _scroll;
        private List<(string name, Texture icon)> _icons;
        private bool _scanned;
        private string _search = "";

        /// <summary>
        /// Opens the Icon Browser window from the Unity menu.
        /// </summary>
        [MenuItem("Strix/Icon Browser")]
        public static void ShowWindow() {
            GetWindow<IconBrowser>(WindowTitle);
        }

        /// <summary>
        /// Draws the main GUI for the icon window.
        /// Initializes icon scanning on first render and draws all UI elements.
        /// </summary>
        private void OnGUI() {
            if (!_scanned) {
                ScanAllIcons();
                _scanned = true;
            }
            StrixEditorUIUtils.DrawTitle(WindowTitle);
            DrawSearchField();
            DrawIconGrid();
        }

        /// <summary>
        /// Scans Unity's built-in icon names and caches unique icon textures with their identifiers.
        /// </summary>
        private void ScanAllIcons() {
            _icons = new List<(string, Texture)>();

            foreach (var iconName in BuiltInIcons.Names) {
                var content = EditorGUIUtility.IconContent(iconName);
                if (content?.image && !_icons.Exists(x => x.icon == content.image))
                    _icons.Add((iconName, content.image));
            }
        }

        /// <summary>
        /// Draws a search bar UI allowing users to filter icons by name.
        /// Includes a search icon and clear button.
        /// </summary>
        private void DrawSearchField() {
            EditorGUILayout.Space(5);

            using (new GUILayout.HorizontalScope()) {
                var searchIcon = EditorGUIUtility.IconContent("Search Icon");
                GUILayout.Label(searchIcon, GUILayout.Width(20), GUILayout.Height(20));

                GUI.SetNextControlName("SearchField");
                _search = EditorGUILayout.TextField(_search, GUILayout.MinWidth(200));

                if (!string.IsNullOrEmpty(_search)) {
                    if (GUILayout.Button("x", GUILayout.Width(20)))
                        _search = "";
                }
            }
            EditorGUILayout.Space(5);
        }

        /// <summary>
        /// Draws a scrollable grid of icons, filtered by search text.
        /// Icons are laid out in rows and columns based on window width.
        /// </summary>
        private void DrawIconGrid() {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var columns = Mathf.Max(1, Mathf.FloorToInt((position.width - Padding) / IconLabelWidth));
            var shown = 0;
            var inRow = false;

            foreach (var (iconName, texture) in _icons) {
                if (!string.IsNullOrEmpty(_search) && !iconName.ToLower().Contains(_search.ToLower()))
                    continue;

                if (shown % columns == 0) {
                    EditorGUILayout.BeginHorizontal();
                    inRow = true;
                }

                DrawIconItem(iconName, texture);
                shown++;

                if (shown % columns != 0) continue;
                EditorGUILayout.EndHorizontal();
                inRow = false;
            }
            if (inRow) EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws an individual icon preview with label and a "Copy" button to copy the icon name to clipboard.
        /// </summary>
        /// <param name="iconName">The name identifier of the icon.</param>
        /// <param name="texture">The icon texture to display.</param>
        private static void DrawIconItem(string iconName, Texture texture) {
            GUILayout.BeginVertical(GUILayout.Width(IconLabelWidth));
            GUILayout.Label(new GUIContent(texture), GUILayout.Width(IconSize), GUILayout.Height(IconSize));
            GUILayout.Label(iconName, EditorStyles.miniLabel, GUILayout.Width(IconLabelWidth));

            if (GUILayout.Button("Copy", GUILayout.Width(IconSize)))
                EditorGUIUtility.systemCopyBuffer = iconName;

            GUILayout.EndVertical();
        }
    }
}