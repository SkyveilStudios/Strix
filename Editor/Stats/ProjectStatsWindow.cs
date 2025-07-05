#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Stats {
    public class ProjectStatsWindow : EditorWindow {
        [MenuItem("Strix/Project Stats")]
        public static void ShowWindow() {
            var window = GetWindow<ProjectStatsWindow>();
            window.titleContent = new GUIContent("Project Stats", EditorGUIUtility.IconContent("Project").image);
        }

        private void OnGUI() {
            GUILayout.Label("Project Stats Tool", EditorStyles.boldLabel);
        }
    }
}
#endif