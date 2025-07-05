using System;

namespace Strix.Editor.TaskBoard.Data {
    [Serializable]
    public struct TaskItem : IEquatable<TaskItem> {
        public string description;
        public long dueDateTicks;
        public string tags;

        public bool Equals(TaskItem other) {
            return description == other.description && dueDateTicks == other.dueDateTicks && tags == other.tags;
        }

        public override bool Equals(object obj) {
            return obj is TaskItem other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(description, dueDateTicks, tags);
        }
    }

    [Serializable]
    public class TaskListData {
        public TaskItem[] active;
        public TaskItem[] done;
    }
}