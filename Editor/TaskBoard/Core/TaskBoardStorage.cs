using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Strix.Editor.TaskBoard.Data;

namespace Strix.Editor.TaskBoard.Core {
    public static class TaskBoardStorage {
        private const string EditorPrefsKey = "TaskBoard.TasksData";

        public static void Save(List<TaskItem> active, List<TaskItem> done) {
            var data = new TaskListData { active = active.ToArray(), done = done.ToArray() };
            EditorPrefs.SetString(EditorPrefsKey, JsonUtility.ToJson(data));
        }

        public static void Load(List<TaskItem> active, List<TaskItem> done) {
            active.Clear();
            done.Clear();
            var json = EditorPrefs.GetString(EditorPrefsKey, "");
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<TaskListData>(json);
            if (data?.active != null) active.AddRange(data.active);
            if (data?.done != null) done.AddRange(data.done);
        }
    }
}