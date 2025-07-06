#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Notepad
{
    public class NotepadModel
    {
        public string Text { get; private set; } = string.Empty;
        private string FilePath { get; set; } = "NewNote";
        public bool HasUnsavedChanges { get; private set; }
        public List<string> Files { get; private set; } = new();
        public int SelectedFileIndex { get; private set; } = -1;

        private readonly INoteObserver _view;

        public NotepadModel(INoteObserver view)
        {
            _view = view;
        }

        public void Init()
        {
            var notesDir = Path.Combine("Assets/SkyveilStudios/Notepad", "Notes");
            if (!Directory.Exists(notesDir))
                Directory.CreateDirectory(notesDir);
            
            LoadFiles();
            LoadTextFromFile();
        }

        public void SelectFileFromList(int index)
        {
            if (SelectedFileIndex == index)
                return;

            if (HasUnsavedChanges && EditorUtility.DisplayDialog("Unsaved Changes",
                    "You have unsaved changes. Do you want to save before discarding and changing to another file?",
                    "Yes",
                    "No"))
            {
                SaveTextToFile();
            }

            SelectedFileIndex = index;
            FilePath = Files[SelectedFileIndex];
            LoadTextFromFile();
        }

        public void UpdateTextIfChanged(string text)
        {
            if (Text == text) return;

            Text = text;
            HasUnsavedChanges = true;
            _view.ModelUpdated();
        }

        public void LoadFiles()
        {
            var notesFolder = Path.Combine("Assets/SkyveilStudios/Notepad", "Notes");
            if (!Directory.Exists(notesFolder))
            {
                Files.Clear();
                Debug.LogWarning("Notes folder not found: " + notesFolder);
                return;
            }

            Files = Directory.GetFiles(notesFolder)
                .Where(file => !file.EndsWith(".meta"))
                .Select(Path.GetFileName)
                .ToList();

            if (Files.Count > 0)
            {
                SelectedFileIndex = Files.FindIndex(x => x == FilePath);
                if (SelectedFileIndex == -1)
                {
                    SelectedFileIndex = 0;
                    FilePath = Files[0];
                }
            }
            else
            {
                SelectedFileIndex = -1;
                FilePath = "NewNote";
            }

            _view.ModelUpdated();
        }

        public void SaveTextToFile()
        {
            try
            {
                var fullPath = Path.Combine("Assets/SkyveilStudios/Notepad", "Notes", FilePath);
                File.WriteAllText(fullPath, Text);
                AssetDatabase.Refresh();
                HasUnsavedChanges = false;
                _view.ModelUpdated();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save Notepad: " + e.Message);
            }
        }

        private void LoadTextFromFile()
        {
            try
            {
                var fullPath = Path.Combine("Assets/SkyveilStudios/Notepad", "Notes", FilePath);
                if (File.Exists(fullPath))
                {
                    Debug.LogWarning("Notepad file not found: " + fullPath);
                    
                    if (FilePath == "NewNote")
                    {
                        Directory.CreateDirectory(Path.Combine("Assets/SkyveilStudios/Notepad", "Notes"));
                        File.WriteAllText(fullPath, string.Empty);
                        AssetDatabase.Refresh();
                        Debug.Log("Created default Notepad file: " + fullPath);
                    }
                }
                Text = File.Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
                HasUnsavedChanges = false;
                GUI.FocusControl(null);
                _view.ModelUpdated();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load Notepad: " + e.Message);
            }
        }

        public void CheckForUnsavedChangesBeforeCreatingNewFile()
        {
            if (HasUnsavedChanges && EditorUtility.DisplayDialog("Unsaved Changes",
                    "You have unsaved changes. Do you want to save before creating a new file?",
                    "Yes",
                    "No"))
            {
                SaveTextToFile();
            }

            CreateNewFile();
            _view.ModelUpdated();
            EditorWindow.GetWindow<NotepadWindow>().Repaint();
        }

        private void CreateNewFile()
        {
            string newFileName = EditorUtility.SaveFilePanel(
                "Create New File",
                Path.Combine("Assets/SkyveilStudios/Notepad", "Notes"),
                "NewNote",
                "txt");

            if (string.IsNullOrEmpty(newFileName))
                return;

            newFileName = Path.GetFileName(newFileName);
            var fullPath = Path.Combine("Assets/SkyveilStudios/Notepad", "Notes", newFileName);

            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, string.Empty);
                AssetDatabase.Refresh();
                LoadFiles();
                SelectedFileIndex = Files.FindIndex(name => name == newFileName);
                FilePath = newFileName;
                Text = string.Empty;
                HasUnsavedChanges = false;
                GUI.FocusControl(null);
                _view.ModelUpdated();
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "File Exists",
                    "A file with that name already exists. Please choose a different name.",
                    "No");
            }
        }

        public void CheckForUnsavedChanges()
        {
            if (HasUnsavedChanges && EditorUtility.DisplayDialog("Unsaved Changes",
                    "You have unsaved changes. Do you want to save before discarding and changing to another file?",
                    "Yes",
                    "No"))
            {
                SaveTextToFile();
            }
        }

        public void OnDestroy()
        {
            CheckForUnsavedChanges();
        }
    }
}
#endif