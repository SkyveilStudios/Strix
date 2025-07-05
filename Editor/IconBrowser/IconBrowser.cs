using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.IconBrowser {
    public class IconBrowser : EditorWindow {
        private const int IconColumns = 6;
        private const float IconSize = 64.0f;
        private const float IconLabelWidth = 96.0f;
        
        private Vector2 _scrollPos;
        private List<(string name, Texture icon)> _icons;
        private bool _scanned;
        private string _search = "";
        
        [MenuItem("Strix/Icon Browser")]
        public static void ShowWindow() {
            GetWindow<IconBrowser>("Icon Browser");
        }

        private void OnGUI() {
            if (!_scanned) {
                ScanAllIcons();
                _scanned = true;
            }
            
            DrawSearchField();
            DrawIconGrid();
        }

        private void ScanAllIcons() {
            _icons = new List<(string, Texture)>();

            foreach (var iconName in BuiltInIcons.Names) {
                var content = EditorGUIUtility.IconContent(iconName);
                if (content?.image && !_icons.Exists(x => x.icon == content.image))
                    _icons.Add((iconName, content.image));
            }
        }

        private void DrawSearchField() {
            EditorGUILayout.Space(5);

            var iconLabel = EditorGUIUtility.IconContent("Search Icon");
            iconLabel.text = "Search";
            EditorGUILayout.LabelField(iconLabel, EditorStyles.boldLabel);

            _search = EditorGUILayout.TextField(_search);
            EditorGUILayout.Space(5);
        }

        private void DrawIconGrid() {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var shown = 0;
            var inRow = false;

            foreach (var (iconName, texture) in _icons) {
                if (!string.IsNullOrEmpty(_search) && !iconName.ToLower().Contains(_search.ToLower()))
                    continue;

                if (shown % IconColumns == 0) {
                    EditorGUILayout.BeginHorizontal();
                    inRow = true;
                }

                DrawIconItem(iconName, texture);
                shown++;

                if (shown % IconColumns != 0) continue;
                EditorGUILayout.EndHorizontal();
                inRow = false;
            }

            if (inRow) EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        
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