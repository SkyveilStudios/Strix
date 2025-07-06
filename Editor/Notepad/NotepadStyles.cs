#if UNITY_EDITOR
using UnityEngine;

namespace Strix.Editor.Notepad
{
    public static class NotepadStyles
    {
        public static GUIStyle CreateTextAreaStyle(Font font, int fontSize, Color textColor, Color backgroundColor)
        {
            return new GUIStyle(GUI.skin.textArea)
            {
                font = font,
                fontSize = fontSize,
                normal =
                {
                    textColor = textColor,
                    background = MakeTexture(backgroundColor)
                }
            };
        }

        public static GUIStyle CreateMiniButtonStyle()
        {
            return new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 24,
                fixedHeight = 24,
                padding = new RectOffset(0, 0, 0, 0)
            };
        }

        private static Texture2D MakeTexture(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
    }
}
#endif