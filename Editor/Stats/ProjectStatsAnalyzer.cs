#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Stats {
    /// <summary>
    /// Analyzes project folders to gather data, code, and assets
    /// </summary>
    public static class ProjectStatsAnalyzer {
        private static readonly string[] ScriptExtensions = { ".cs" };

        private static readonly Dictionary<string, string[]> AssetTypeExtensions = new() {
            { "Prefabs", new[] { "*.prefab" } },
            { "Materials", new[] { "*.mat" } },
            { "Scenes",  new[] { "*.unity" } },
            { "Textures", new[] { "*.png",  "*.jpg", "*.jpeg", "*.bmp", "*.tga" } },
            { "Animation Clips", new[] { "*.anim" } },
            { "Audio Clips", new[] { "*.mp3", "*.wav", "*.ogg" } },
            { "Shaders", new[] { "*.shader" } },
            { "Scriptable Objects", new[] { "*.asset" } },
            { "Sprite Atlases", new[] { "*.spriteatlas" } },
            { "Fonts", new[] { "*.ttf", "*.otf", "*.fontsettings" } },
            { "Video Clips", new[] { "*.mp4", "*.avi", "*.wmv" } }
        };

        /// <summary>
        /// Performs the analysis for the folder path
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static (ProjectStatsData stats, string log) Analyze(string folderPath) {
            var stats = new ProjectStatsData();
            var log = new ProjectStatsLogBuilder();
            
            log.AddSection("Project Metadata");
            log.AddLine("Project Name", stats.MetaData.Name);
            log.AddLine("Project Version", stats.MetaData.Version);
            log.AddLine("Project Platform", stats.MetaData.Platform);
            
            AnalyzeScripts(folderPath, stats.Code, log);

            foreach (var kvp in AssetTypeExtensions) {
                var count = AnalyzeAssets(folderPath, kvp.Value, kvp.Key, log);
                SetAssetStat(stats.Assets, kvp.Key, count);
            }
            
            stats.Assets.TotalSize = CalculateTotalSize(folderPath);

            return (stats, log.ToString());
        }

        private static void FetchMetaData(ProjectMetadata metaData) {
            metaData.Name = PlayerSettings.productName;
            metaData.Version = PlayerSettings.bundleVersion;
            metaData.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        private static void ResetStats(ProjectStatsData stats) {
            stats.Code = new CodeStats();
            stats.Assets = new AssetStats();
        }

        private static void AnalyzeScripts(string folderPath, CodeStats codeStats, ProjectStatsLogBuilder log) {
            var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
            codeStats.ScriptCount = files.Length;
            
            log.AddSection("Scripts");

            foreach (var file in files.OrderByDescending(f => new FileInfo(f).Length)) {
                var lines =  File.ReadAllLines(file);
                var inBlockComment = false;

                foreach (var line in lines) {
                    codeStats.TotalLines++;
                    var trimmed = line.Trim();

                    if (inBlockComment) {
                        if (trimmed.Contains("/*")) inBlockComment = false;
                        continue;
                    }
                    
                    if (trimmed.StartsWith("/*")) { inBlockComment = true; continue; }
                    if (trimmed.StartsWith("//")) continue;
                    
                    if (trimmed.StartsWith("namespace ")) codeStats.TotalNamespaces++;
                    else if (trimmed.Contains(" class ")) codeStats.ClassCount++;
                    else if (trimmed.Contains(" interface ")) codeStats.InterfaceCount++;
                    else if (trimmed.Contains(" enum")) codeStats.EnumCount++;
                    else if (trimmed.Contains(" struct ")) codeStats.StructCount++;
                }
                
                var fileInfo =  new FileInfo(file);
                log.AddFile(fileInfo.Name, fileInfo.Length);
            }
        }

        private static int AnalyzeAssets(string folderPath, string[] extensions, string label, ProjectStatsLogBuilder log) {
            log.AddSection(label);

            var files = extensions
                .SelectMany(ext => Directory.GetFiles(folderPath, ext, SearchOption.AllDirectories))
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.Length)
                .ToList();
            
            foreach (var file in files)
                log.AddFile(file.Name, file.Length);
            
            return files.Count;
        }

        private static long CalculateTotalSize(string folderPath) {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Sum(f => new FileInfo(f).Length);
        }

        private static void SetAssetStat(AssetStats stats, string label, int count) {
            switch (label) {
                case "Prefabs": stats.Prefabs = count; break;
                case "Materials": stats.Materials = count; break;
                case "Scenes": stats.Scenes = count; break;
                case "Textures": stats.Textures = count; break;
                case "Audio Clips": stats.AudioClips = count; break;
                case "Shaders": stats.Shaders = count; break;
                case "Animation Clips": stats.AnimClips = count; break;
                case "Scriptable Objects": stats.Scriptables = count; break;
                case "Sprite Atlases": stats.Atlases = count; break;
                case "Fonts": stats.Fonts = count; break;
                case "Video Clips": stats.Videos = count; break;
            }
        }
    }
}
#endif