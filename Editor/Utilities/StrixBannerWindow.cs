using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Utilities {
    public class StrixBannerWindow : EditorWindow {
        [MenuItem("Strix/Show Banner Window")]
        public static void ShowWindow() {
            var window = GetWindow<StrixBannerWindow>();
            window.titleContent = new GUIContent("Strix Banner");
            window.Show();
        }

        private void OnGUI() {
            InspectorImageUtility.DrawImage(
                assetPath: "Assets/Strix/Editor/Banners/StrixBanner.jpg",
                width: 1080f,
                fullWidth: true,
                alignment: ImageAlignment.Center
                );
            
            GUILayout.Space(10);
        }
    }
}