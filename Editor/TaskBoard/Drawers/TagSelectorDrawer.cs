using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Strix.Editor.TaskBoard.Utility;

namespace Strix.Editor.TaskBoard.Drawers {
    public static class TagSelectorDrawer {
        public static void Draw(HashSet<string> selectedTags, float maxWidth) {
            float totalWidth = 0;
            EditorGUILayout.BeginHorizontal();

            foreach (var tag in TaskBoardWindow.DefaultTags) {
                var icon = TaskTagUtility.GetTagIcon(tag);
                var label = $"{icon} {tag}";
                var isSelected = selectedTags.Contains(tag);

                var style = new GUIStyle(EditorStyles.miniButton) {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 22,
                    padding = new RectOffset(10, 10, 4, 4),
                    normal = { textColor = Color.white }
                };

                var size = style.CalcSize(new GUIContent(label));
                var tagWidth = size.x + 8;

                if (totalWidth + tagWidth > maxWidth) {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    totalWidth = 0;
                }

                var prevColor = GUI.backgroundColor;
                GUI.backgroundColor = isSelected ? TaskTagUtility.GetTagColor(tag) : new Color(0.25f, 0.25f, 0.25f);

                var toggled = GUILayout.Toggle(isSelected, label, style, GUILayout.Width(tagWidth));
                if (toggled) selectedTags.Add(tag);
                else selectedTags.Remove(tag);

                GUI.backgroundColor = prevColor;
                totalWidth += tagWidth;
            }

            EditorGUILayout.EndHorizontal();
        }

        public static string Draw(string csvTags, float maxWidth) {
            var selected = csvTags.Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToHashSet(System.StringComparer.OrdinalIgnoreCase);

            Draw(selected, maxWidth);
            return string.Join(",", selected);
        }
    }
}