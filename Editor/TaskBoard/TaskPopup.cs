#if UNITY_EDITOR
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Strix.Editor.TaskBoard.Data;
using Strix.Editor.TaskBoard.Drawers;

namespace Strix.Editor.TaskBoard
{
    public class TaskPopup : EditorWindow
    {
        private static TaskItem _taskItem;
        private static Action<TaskItem> _onUpdate;

        private string _description;
        private string _dueDate;
        private string _tags;

        public static void Show(TaskItem task, Action<TaskItem> onUpdate)
        {
            _taskItem = task;
            _onUpdate = onUpdate;

            var window = CreateInstance<TaskPopup>();
            window.titleContent = new GUIContent("Edit Task");
            window.position = new Rect(Screen.width / 2f - 230f, Screen.height / 2f - 150f, 460f, 300f);
            window.minSize = new Vector2(460, 300);
            window.ShowUtility();
        }

        private void OnEnable()
        {
            _description = _taskItem.description;
            _dueDate = new DateTime(_taskItem.dueDateTicks).ToString("yyyy-MM-dd");
            _tags = _taskItem.tags;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Edit Task", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical("box");
            _description = EditorGUILayout.TextField("Description", _description);
            _dueDate = EditorGUILayout.TextField("Due Date (yyyy-MM-dd)", _dueDate);

            EditorGUILayout.LabelField("Tags");
            _tags = TagSelectorDrawer.Draw(_tags, position.width - 40);
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save", GUILayout.Height(28)))
            {
                if (!DateTime.TryParseExact(_dueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                    parsedDate = DateTime.Today;

                _taskItem.description = _description.Trim();
                _taskItem.dueDateTicks = parsedDate.Ticks;
                _taskItem.tags = _tags.Trim();

                _onUpdate?.Invoke(_taskItem);
                Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Height(28)))
                Close();

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
