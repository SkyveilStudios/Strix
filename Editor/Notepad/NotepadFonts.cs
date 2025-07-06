#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Notepad
{
    public static class NotepadFonts
    {
        private static readonly string[] SearchPaths = { "Assets/SkyveilStudios/Notepad/Fonts" };

        public static Font LoadFont(string fontName)
        {
            var path = AssetDatabase.FindAssets("t:Font", SearchPaths)
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == fontName);

            if (!string.IsNullOrEmpty(path))
                return AssetDatabase.LoadAssetAtPath<Font>(path);

            Debug.LogWarning($"Font '{fontName}' not found in NotepadFonts.");
            return null;
        }

        public static string[] GetFontNames()
        {
            return AssetDatabase.FindAssets("t:Font", SearchPaths)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
    }
}
#endif