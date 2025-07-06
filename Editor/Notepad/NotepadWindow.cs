#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Notepad
{
    public class NotepadWindow : EditorWindow, INoteObserver
    {
        [SerializeField] private int fontSize = 14;
        [SerializeField] private Vector2 scrollPosition;

        private NotepadModel _model;
        private NotepadSettings _settings;
        private GUIStyle _textAreaStyle;
        private GUIStyle _buttonStyle;
        private Font _customFont;

        [MenuItem("Strix/Notepad/Window", priority = 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<NotepadWindow>();
            window.titleContent = new GUIContent("Notepad", EditorGUIUtility.IconContent("d_TextAsset Icon").image);
        }

        private void OnEnable()
        {
            _model ??= new NotepadModel(this);
            _model.Init();

            LoadSettings();
            EditorApplication.quitting += OnEditorQuitting;
        }

        private void OnDisable()
        {
            EditorApplication.quitting -= OnEditorQuitting;
        }

        private void OnDestroy()
        {
            _model?.OnDestroy();
        }

        private void OnGUI()
        {
            _model ??= new NotepadModel(this);

            HandleShortcuts();
            LoadFont();
            SetupStyles();

            DrawToolbar();
            DrawFontControls();
            DrawTextArea();
        }

        public void ModelUpdated()
        {
            UpdateWindowTitle();
            Repaint();
        }

        private void UpdateWindowTitle()
        {
            titleContent.text = "Notepad" + (_model.HasUnsavedChanges ? " *" : "");
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Select File:");
            var newIndex = EditorGUILayout.Popup(_model.SelectedFileIndex, _model.Files.ToArray());
            _model.SelectFileFromList(newIndex);

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh", "Reload files"), _buttonStyle))
                _model.LoadFiles();

            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus", "Create new file"), _buttonStyle))
                _model.CheckForUnsavedChangesBeforeCreatingNewFile();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawFontControls()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Font Size:");
            fontSize = EditorGUILayout.IntSlider(fontSize, 10, 30);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTextArea()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            var newText = EditorGUILayout.TextArea(_model.Text, _textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            _model.UpdateTextIfChanged(newText);
        }

        private void LoadFont()
        {
            if (!_settings) return;

            if (_settings.useCustomFont)
            {
                var fontPath = AssetDatabase.FindAssets("t:Font", new[] { "Assets/Strix/Editor/Notepad/Fonts" })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == _settings.selectedFont);

                if (!string.IsNullOrEmpty(fontPath))
                    _customFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            }

            _textAreaStyle = NotepadStyles.CreateTextAreaStyle(
                _customFont,
                fontSize,
                _settings?.textColor ?? Color.white,
                _settings?.backgroundColor ?? Color.black
            );
        }

        private void SetupStyles()
        {
            _buttonStyle = NotepadStyles.CreateMiniButtonStyle();
        }

        private void HandleShortcuts()
        {
            var e = Event.current;
            if (e.type != EventType.KeyDown || (!e.control && !e.command) || e.keyCode != KeyCode.S) return;
            e.Use();
            _model.SaveTextToFile();
        }

        private void OnEditorQuitting()
        {
            _model?.CheckForUnsavedChanges();
        }

        private void LoadSettings()
        {
            _settings = AssetDatabase.LoadAssetAtPath<NotepadSettings>("Assets/Strix/Editor/Notepad/Settings/NotepadSettings.asset");
            if (!_settings)
            {
                _settings = CreateInstance<NotepadSettings>();
                AssetDatabase.CreateAsset(_settings, "Assets/Strix/Editor/Notepad/Settings/NotepadSettings.asset");
                AssetDatabase.SaveAssets();
                Debug.Log("Created new NotepadSettings asset at: " + "Assets/Strix/Editor/Notepad/Settings/NotepadSettings.asset");
            }
        }

    }
}
#endif