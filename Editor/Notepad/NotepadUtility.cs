#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Strix.Editor.Notepad {
    public static class NotepadUtility {
        public static Texture2D CreateSolidColorTexture(Color color) {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        public static string CombinePath(params string[] parts) {
            return Path.Combine(parts);
        }
    }
}
#endif