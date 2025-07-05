#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Strix.Editor.TaskBoard.Data;
using Strix.Editor.TaskBoard.Core;
using Strix.Editor.TaskBoard.Drawers;

namespace Strix.Editor.TaskBoard
{
    public class TaskBoardWindow : EditorWindow
    {
        public static readonly string[] DefaultTags = {
            "Bug", "Task", "Feature", "Polish", "Design", "Art", "Audio",
            "High Priority", "Low Priority", "Blocked", "Review", "In Progress", "Done"
        };

        private readonly List<TaskItem> _activeTasks = new();
        private readonly List<TaskItem> _doneTasks = new();
        private readonly HashSet<string> _newTags = new();

        private string _newTaskText = "";
        private DateTime _newDueDate = DateTime.Today;
        private Vector2 _scrollActive, _scrollDone;
        private int _activeTabIndex;
        private bool _hideDueDates = true, _hideTags;
        private readonly string[] _tabLabels = { "Active", "Completed" };

        [MenuItem("Strix/Task Board")]
        public static void ShowWindow()
        {
            var window = GetWindow<TaskBoardWindow>();
            window.titleContent = new GUIContent("Task Board", EditorGUIUtility.IconContent("d_InputField Icon").image);
        }

        private void OnEnable() => TaskBoardStorage.Load(_activeTasks, _doneTasks);
        private void OnDisable() => TaskBoardStorage.Save(_activeTasks, _doneTasks);

        private void OnGUI()
        {
            DrawHeader();
            GUILayout.Space(10);
            DrawAddTaskUI();
            GUILayout.Space(10);
            DrawTabView();
        }

        private void DrawHeader()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 40);
            var drawTitle = new GUIContent("Task Board", EditorGUIUtility.IconContent("d_InputField Icon").image);
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            GUI.Label(rect, drawTitle, style);

            var iconContent = EditorGUIUtility.IconContent("_Popup", "Options");
            var buttonRect = new Rect(rect.xMax - 28, rect.y + 8, 24, 24);
            if (!GUI.Button(buttonRect, iconContent)) return;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Hide Due Dates"), _hideDueDates, () => { _hideDueDates = !_hideDueDates; Repaint(); });
            menu.AddItem(new GUIContent("Hide Tags"), _hideTags, () => { _hideTags = !_hideTags; Repaint(); });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Export JSON"), false, Export);
            menu.AddItem(new GUIContent("Import JSON"), false, Import);
            menu.DropDown(buttonRect);
        }

        private void DrawAddTaskUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Add New Task", EditorStyles.boldLabel);
            _newTaskText = EditorGUILayout.TextField("Description", _newTaskText);
            var dateStr = EditorGUILayout.TextField("Due Date (yyyy-MM-dd)", _newDueDate.ToString("yyyy-MM-dd"));
            if (DateTime.TryParse(dateStr, out var parsed)) _newDueDate = parsed;

            EditorGUILayout.LabelField("Tags");
            TagSelectorDrawer.Draw(_newTags, position.width - 40);

            if (GUILayout.Button("Add Task", GUILayout.Height(28)))
            {
                var trimmed = _newTaskText.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    _activeTasks.Add(new TaskItem
                    {
                        description = trimmed,
                        dueDateTicks = _newDueDate.Ticks,
                        tags = string.Join(",", _newTags)
                    });
                    _newTaskText = "";
                    _newDueDate = DateTime.Today;
                    _newTags.Clear();
                    TaskBoardStorage.Save(_activeTasks, _doneTasks);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTabView()
        {
            _activeTabIndex = GUILayout.Toolbar(_activeTabIndex, _tabLabels);
            GUILayout.Space(5);

            if (_activeTabIndex == 0)
                _scrollActive = DrawTaskList(_activeTasks, true, _scrollActive);
            else
                _scrollDone = DrawTaskList(_doneTasks, false, _scrollDone);
        }

        private Vector2 DrawTaskList(List<TaskItem> tasks, bool isActive, Vector2 scroll)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var task in tasks.ToList())
            {
                TaskCardDrawer.Draw(task, isActive,
                    onComplete: t => { _doneTasks.Add(t); _activeTasks.Remove(t); },
                    onReturn: t => { _activeTasks.Add(t); _doneTasks.Remove(t); },
                    onEdit: t =>
                    {
                        TaskPopup.Show(t, updated =>
                        {
                            var list = isActive ? _activeTasks : _doneTasks;
                            var index = list.IndexOf(t);
                            if (index >= 0) list[index] = updated;
                            TaskBoardStorage.Save(_activeTasks, _doneTasks);
                            Repaint();
                        });
                    },
                    onDelete: t =>
                    {
                        _activeTasks.Remove(t);
                        _doneTasks.Remove(t);
                        TaskBoardStorage.Save(_activeTasks, _doneTasks);
                    }
                );
            }

            EditorGUILayout.EndScrollView();
            return scroll;
        }

        private void Export()
        {
            var path = EditorUtility.SaveFilePanel("Export Task List", Application.dataPath, "TaskBoard.json", "json");
            if (string.IsNullOrEmpty(path)) return;
            var data = new TaskListData { active = _activeTasks.ToArray(), done = _doneTasks.ToArray() };
            var json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(path, json);
            Debug.Log("Tasks exported to: " + path);
        }

        private void Import()
        {
            var path = EditorUtility.OpenFilePanel("Import Task List", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) return;
            var json = System.IO.File.ReadAllText(path);
            var data = JsonUtility.FromJson<TaskListData>(json);
            if (data == null) return;

            if (!EditorUtility.DisplayDialog("Import Tasks", "Overwrite current task list?", "Yes", "Cancel")) return;
            _activeTasks.Clear();
            _doneTasks.Clear();
            _activeTasks.AddRange(data.active);
            _doneTasks.AddRange(data.done);
            TaskBoardStorage.Save(_activeTasks, _doneTasks);
        }
    }
}
#endif
