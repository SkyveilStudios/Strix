#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Stats {
    /// <summary>
    /// Editor window for viewing project stats
    /// </summary>
    public class ProjectStatsWindow : EditorWindow {
        [SerializeField] private string folderPath = "Assets";
        private static string LogFilePath => Path.Combine(Application.dataPath, "ProjectStatsLog.txt");
        
        private ProjectStatsData _stats;
        private string _log;
        
        [MenuItem("Strix/Project Stats")]
        public static void ShowWindow() {
            var window = GetWindow<ProjectStatsWindow>();
            window.titleContent = new GUIContent("Project Stats", EditorGUIUtility.IconContent("Project").image);
        }

        private void OnGUI() {
            GUILayout.Label("Folder Path", EditorStyles.boldLabel);
            folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

            if (GUILayout.Button("Analyze")) {
                (_stats, _log) = ProjectStatsAnalyzer.Analyze(folderPath);
                File.WriteAllText(LogFilePath, _log);
            }

            if (_stats == null)
                return;
            
            GUILayout.Space(20);
            DrawMetaData();
            DrawCodeStats();
            DrawAssetStats();
            
            GUILayout.Space(20);
            GUILayout.Label("Results saved to ProjectStatsLog.text", EditorStyles.boldLabel);
            if (GUILayout.Button("Open Log File"))
                EditorUtility.RevealInFinder(LogFilePath);
        }
        
        private void DrawMetaData() {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Project Name:", _stats.MetaData.Name);
            EditorGUILayout.LabelField("Project Version:", _stats.MetaData.Version);
            EditorGUILayout.LabelField("Target Platform", _stats.MetaData.Platform);
        }

        private void DrawCodeStats() {
            GUILayout.Space(10);
            GUILayout.Label("Code Analysis", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total .cs Scripts:", _stats.Code.ScriptCount.ToString());
            EditorGUILayout.LabelField("Total Lines:", _stats.Code.TotalLines.ToString());
            EditorGUILayout.LabelField("Total Namespaces:", _stats.Code.TotalNamespaces.ToString());
            EditorGUILayout.LabelField("Total Classes:", _stats.Code.ClassCount.ToString());
            EditorGUILayout.LabelField("Total Interfaces:", _stats.Code.InterfaceCount.ToString());
            EditorGUILayout.LabelField("Total Enums:", _stats.Code.EnumCount.ToString());
            EditorGUILayout.LabelField("Total Structs:", _stats.Code.StructCount.ToString());
        }

        private void DrawAssetStats() {
            GUILayout.Space(10);
            GUILayout.Label("Asset Analysis",  EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total Prefabs:", _stats.Assets.Prefabs.ToString());
            EditorGUILayout.LabelField("Total Materials:", _stats.Assets.Materials.ToString());
            EditorGUILayout.LabelField("Total Scenes:",  _stats.Assets.Scenes.ToString());
            EditorGUILayout.LabelField("Total Textures:",  _stats.Assets.Textures.ToString());
            EditorGUILayout.LabelField("Total Audio Clips:",  _stats.Assets.AudioClips.ToString());
            EditorGUILayout.LabelField("Total Shaders:", _stats.Assets.Shaders.ToString());
            EditorGUILayout.LabelField("Total Animation Clips:", _stats.Assets.AnimClips.ToString());
            EditorGUILayout.LabelField("Total Scriptable Objects:", _stats.Assets.Scriptables.ToString());
            EditorGUILayout.LabelField("Total Sprite Atlases:", _stats.Assets.Atlases.ToString());
            EditorGUILayout.LabelField("Total Fonts:", _stats.Assets.Fonts.ToString());
            EditorGUILayout.LabelField("Total Video Clips:", _stats.Assets.Videos.ToString());
            EditorGUILayout.LabelField("Total Asset Size (MB)", $"{_stats.Assets.TotalSize / 1024f / 1024f:0.00} MB");
        }
    }
}
#endif