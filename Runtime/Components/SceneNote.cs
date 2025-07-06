#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Strix.Runtime.Components {
    /// <summary>
    /// Displays text and an optional marker to an object in scene view
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class SceneNote : MonoBehaviour {
        public enum NoteVisibility { Always, SelectedOnly, Hidden }
        public enum NoteCategory { Info, Warning, Design, Bug, Custom}

        [Header("Note Settings")] 
        [Tooltip("Should the title display in the scene view.")]
        public bool showTitle = true;
        
        [Tooltip("Title displayed in the scene view")] 
        public string title = "Scene Note";

        [Range(8, 32), Tooltip("Font size for the Title")] 
        public int titleSize = 12;

        [Tooltip("Offset from the GameObject for displaying the title.")]
        public Vector3 worldOffset = new(0, 1f, 0);

        [Tooltip("Display condition for the note.")]
        public NoteVisibility visibility = NoteVisibility.Always;

        [Tooltip("Categorization tag for organization and color.")]
        public NoteCategory category = NoteCategory.Info;

        [Tooltip("Only visible when 'Custom' category is selected.")]
        public Color customColor = Color.white;
        
        [TextArea(3, 10), Tooltip("Description for the scene note. Not shown in scene view")]
        public string description;

        [Header("Marker Settings")] 
        public bool showMarker = true;
        [Range(0.05f, 1.0f)] 
        public float markerRadius = 0.25f;
        
        #if UNITY_EDITOR
        public bool ShouldDisplayInEditor =>
            visibility switch {
                NoteVisibility.Always => true,
                NoteVisibility.SelectedOnly => Selection.Contains(gameObject),
                _ => false
            };

        public Color CategoryColor =>
            category switch {
                NoteCategory.Info => Color.cyan,
                NoteCategory.Warning => Color.yellow,
                NoteCategory.Design => Color.green,
                NoteCategory.Bug => Color.red,
                NoteCategory.Custom => customColor,
                _ => Color.white
            };
        #endif
    }
}