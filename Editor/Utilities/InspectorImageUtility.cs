using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Utilities {
    public enum ImageAlignment {
        Left,
        Center,
        Right
    }
    
    public static class InspectorImageUtility {
        private static Texture2D _cachedTexture;
        private static string _cachedPath;

        public static void DrawImage(string assetPath, float width, bool fullWidth = false, ImageAlignment alignment = ImageAlignment.Center, float padding = 4f) {
            if (string.IsNullOrEmpty(assetPath)) {
                EditorGUILayout.LabelField("Invalid image path");
                return;
            }

            if (_cachedTexture == null || _cachedPath != assetPath) {
                _cachedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                _cachedPath = assetPath;
            }
            
            var texture = _cachedTexture;
            if (texture == null) {
                EditorGUILayout.LabelField($"Invalid image at: {assetPath}");
                return;
            }
            
            var maxWidth = fullWidth ? EditorGUIUtility.currentViewWidth - 40f : width;
            var aspect = (float)texture.width / texture.height;
            var height = maxWidth / aspect;
            
            var rect = GUILayoutUtility.GetRect(maxWidth, height,  GUILayout.ExpandWidth(false));

            switch (alignment) {
                case ImageAlignment.Center:
                    rect.x = (EditorGUIUtility.currentViewWidth - maxWidth) / 2f;
                    break;
                case ImageAlignment.Right:
                    rect.x = EditorGUIUtility.currentViewWidth - maxWidth - padding;
                    break;
                case ImageAlignment.Left:
                default:
                    rect.x += padding;
                    break;
            }
            EditorGUI.DrawPreviewTexture(rect, texture, null, ScaleMode.ScaleToFit);
        }
    }
}