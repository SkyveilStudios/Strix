#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Strix.Runtime.Components {
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class SceneNote : MonoBehaviour {
        public enum NoteVisibility {
            Always,
            SelectedOnly,
            Hidden
        }

        public enum NoteCategory {
            Info,
            Warning,
            Design,
            Bug,
            TODO,
            Custom
        }

        [Header("Note Settings")] [TextArea(2, 10)]
        public string note = "Scene Note";

        [Tooltip("Font size for the scene note.")] [Range(8, 32)]
        public int fontSize = 12;

        [Tooltip("Offset from the GameObject for displaying the note.")]
        public Vector3 worldOffset = new(0, 1f, 0);

        [Tooltip("Display condition for the note.")]
        public NoteVisibility visibility = NoteVisibility.Always;

        [Tooltip("Categorization tag for organization and color.")]
        public NoteCategory category = NoteCategory.Info;

        [Tooltip("Only visible when 'Custom' category is selected.")]
        public Color customColor = Color.white;

        [Header("Marker Settings")] public bool showMarker = true;

        [Range(0.05f, 0.5f)] public float markerRadius = 0.25f;
        
        private void OnDrawGizmos() {
            if (!ShouldDisplay()) return;

            var displayColor = GetCategoryColor();

            if (showMarker) {
                Gizmos.color = displayColor;
                Gizmos.DrawSphere(transform.position, markerRadius);
            }

            var style = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = fontSize,
                normal = new GUIStyleState { textColor = displayColor }
            };

            Handles.Label(transform.position + worldOffset, note, style);
        }

        private bool ShouldDisplay() {
            return visibility switch {
                NoteVisibility.Always => true,
                NoteVisibility.SelectedOnly => Selection.Contains(gameObject),
                _ => false,
            };
        }

        private Color GetCategoryColor() {
            return category switch {
                NoteCategory.Info => Color.cyan,
                NoteCategory.Warning => Color.yellow,
                NoteCategory.Design => Color.green,
                NoteCategory.Bug => Color.red,
                NoteCategory.TODO => Color.magenta,
                NoteCategory.Custom => customColor,
                _ => Color.white
            };
        }
    }
}
#endif