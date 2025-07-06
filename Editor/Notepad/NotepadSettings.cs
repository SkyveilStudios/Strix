#if UNITY_EDITOR
using UnityEngine;

namespace Strix.Editor.Notepad
{
    [CreateAssetMenu(fileName = "NotepadSettings", menuName = "Strix/Notepad/Settings")]
    public class NotepadSettings : ScriptableObject
    {
        public Color textColor = new Color32(0x26, 0xAB, 0x2E, 0xFF);
        public Color backgroundColor = new Color32(0x2A, 0x2A, 0x2A, 0xFF);
        public string selectedFont = "CourierPrime";
        public bool useCustomFont = true;
    }
}
#endif