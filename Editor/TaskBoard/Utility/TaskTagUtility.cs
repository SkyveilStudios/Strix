using UnityEngine;

namespace Strix.Editor.TaskBoard.Utility {
    public static class TaskTagUtility {
        public static string GetTagIcon(string tag) {
            tag = tag.ToLowerInvariant();
            return tag switch {
                "bug" => "🐞", "task" => "📝", "feature" => "✨", "polish" => "🧼",
                "design" => "🎨", "art" => "🖌️", "audio" => "🎧", "high priority" => "🚨",
                "low priority" => "⬇️", "blocked" => "⛔", "review" => "🔍", "in progress" => "🔄",
                "done" => "✅", _ => "🏷"
            };
        }

        public static Color GetTagColor(string tag) {
            tag = tag.ToLowerInvariant();
            return tag switch {
                "bug" => new Color(0.8f, 0.2f, 0.2f),
                "task" => new Color(0.75f, 0.75f, 0.75f),
                "feature" => new Color(0.2f, 0.6f, 1f),
                "polish" => new Color(1f, 0.84f, 0f),
                "design" => new Color(0.6f, 0.4f, 1f),
                "art" => new Color(1f, 0.5f, 0.8f),
                "audio" => new Color(0.3f, 0.9f, 0.5f),
                "high priority" => new Color(1f, 0.4f, 0.2f),
                "low priority" => Color.gray,
                "blocked" => new Color(0.5f, 0.1f, 0.1f),
                "review" => new Color(0.9f, 0.7f, 0.2f),
                "in progress" => new Color(0.3f, 0.6f, 1f),
                "done" => new Color(0.3f, 0.8f, 0.3f),
                _ => new Color(0.5f, 0.5f, 0.5f)
            };
        }
    }
}