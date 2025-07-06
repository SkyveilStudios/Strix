#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Notepad
{
    public class NotepadSettingsEditor : EditorWindow
    {
        private NotepadSettings _settings;
        private string[] _fontOptions;
        private int _selectedFontIndex;

        [MenuItem("Strix/Notepad/Settings", priority = 101)]
        public static void ShowWindow()
        {
            GetWindow<NotepadSettingsEditor>("Notepad Settings");
        }

        private void OnEnable()
        {
            LoadSettings();
            LoadFonts();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Notepad Settings", EditorStyles.boldLabel);

            _settings = (NotepadSettings)EditorGUILayout.ObjectField("Settings Asset", _settings, typeof(NotepadSettings), false);

            if (!_settings)
            {
                EditorGUILayout.HelpBox("No NotepadSettings asset found.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space();
            GUILayout.Label("Colors", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            _settings.textColor = EditorGUILayout.ColorField("Text Color", _settings.textColor);
            _settings.backgroundColor = EditorGUILayout.ColorField("Background Color", _settings.backgroundColor);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            GUILayout.Label("Font", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            if (_fontOptions is { Length: > 0 })
            {
                _selectedFontIndex = EditorGUILayout.Popup("Font Name", _selectedFontIndex, _fontOptions);
                _settings.selectedFont = _fontOptions[_selectedFontIndex];
            }
            else
            {
                EditorGUILayout.HelpBox("No fonts found in the Notepad Fonts folder.", MessageType.Warning);
            }

            _settings.useCustomFont = EditorGUILayout.Toggle("Use Custom Font", _settings.useCustomFont);
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
            if (!GUILayout.Button("Save", GUILayout.Height(30))) return;
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }

        private void LoadSettings()
        {
            _settings = AssetDatabase.LoadAssetAtPath<NotepadSettings>("Assets/Strix/Editor/Notepad/Settings/NotepadSettings.asset");
            if (_settings) return;
            _settings = CreateInstance<NotepadSettings>();
            AssetDatabase.CreateAsset(_settings, "Assets/Strix/Editor/Notepad/Settings/NotepadSettings.asset");
            AssetDatabase.SaveAssets();
        }

        private void LoadFonts()
        {
            var fontPaths = AssetDatabase.FindAssets("t:Font", new[] { "Assets/Strix/Editor/Notepad/Fonts" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            _fontOptions = fontPaths.Select(Path.GetFileNameWithoutExtension).ToArray();
            _selectedFontIndex = System.Array.IndexOf(_fontOptions, _settings.selectedFont);
            if (_selectedFontIndex == -1) _selectedFontIndex = 0;
            if (_fontOptions.Length == 0) _selectedFontIndex = -1;
        }

    }
}
#endif