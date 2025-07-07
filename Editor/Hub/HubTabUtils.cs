using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Hub {
    public static class HubTabUtils {
        public static void DrawHeader(string title) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }
        
        public static void DrawSeparator() {
            var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
            GUILayout.Space(4);
        }
        
        public static void DrawSidePanelBackground(float width) {
            GUILayout.Space(0);
            var bgRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.Repaint) {
                EditorGUI.DrawRect(new Rect(bgRect.x, bgRect.y, width, Screen.height), new Color(0.20f, 0.20f, 0.20f));
            }
        }
        
        public static Texture2D MakeTex(int width, int height, Color col) {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; i++) pix[i] = col;

            var tex = new Texture2D(width, height);
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
        
        public static GameObject CreateOrGetPreviewGo(string name = "StrixPreview") {
            var go = GameObject.Find(name) ?? new GameObject(name) {
                hideFlags = HideFlags.HideAndDontSave
            };
            return go;
        }
    }
}