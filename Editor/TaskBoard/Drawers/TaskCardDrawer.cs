using System;
using UnityEditor;
using UnityEngine;
using Strix.Editor.TaskBoard.Data;
using Strix.Editor.TaskBoard.Utility;

namespace Strix.Editor.TaskBoard.Drawers {
    public static class TaskCardDrawer {
        public static void Draw(TaskItem task, bool isActive, Action<TaskItem> onComplete, Action<TaskItem> onReturn, Action<TaskItem> onEdit, Action<TaskItem> onDelete) {
            var due = new DateTime(task.dueDateTicks);
            var dueString = due.ToString("yyyy-MM-dd");
            var daysLeft = (due - DateTime.Today).Days;

            var dueLabel = daysLeft switch {
                < 0 => $"Overdue by {Math.Abs(daysLeft)} day(s)",
                0 => "Due Today",
                1 => "Due Tomorrow",
                _ => $"Due in {daysLeft} days"
            };

            var dueColor = isActive switch {
                true when due < DateTime.Today => Color.red,
                true when due == DateTime.Today => new Color(1f, 0.65f, 0f),
                _ => GUI.skin.label.normal.textColor
            };

            EditorGUILayout.BeginVertical("box");
            DrawTagBadges(task.tags);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"• {task.description}", EditorStyles.label);

            var oldColor = GUI.contentColor;
            GUI.contentColor = dueColor;
            EditorGUILayout.LabelField($"🗓 {dueString} – {dueLabel}", EditorStyles.miniLabel);
            GUI.contentColor = oldColor;

            EditorGUILayout.EndVertical();

            if (isActive) {
                if (GUILayout.Button("✔", GUILayout.Width(24))) onComplete?.Invoke(task);
                if (GUILayout.Button("✎", GUILayout.Width(24))) onEdit?.Invoke(task);
            } else {
                if (GUILayout.Button("↩", GUILayout.Width(24))) onReturn?.Invoke(task);
            }

            if (GUILayout.Button("🗑", GUILayout.Width(24))) onDelete?.Invoke(task);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private static void DrawTagBadges(string tagsCsv) {
            if (string.IsNullOrWhiteSpace(tagsCsv)) return;
            var tags = tagsCsv.Split(',');
            EditorGUILayout.BeginHorizontal();
            foreach (var raw in tags) {
                var tag = raw.Trim();
                if (string.IsNullOrEmpty(tag)) continue;
                var style = new GUIStyle(EditorStyles.miniButton) {
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = TaskTagUtility.GetTagColor(tag) },
                    padding = new RectOffset(6, 6, 2, 2),
                    fixedHeight = 18
                };
                var label = $"{TaskTagUtility.GetTagIcon(tag)} {tag}";
                GUILayout.Button(label, style, GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}