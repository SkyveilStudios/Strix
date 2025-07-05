#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Strix.Editor.Stats {
    /// <summary>
    /// Analyzes project folders to gather metadata, code, and asset statistics.
    /// </summary>
    public static class ProjectStatsAnalyzer {
        private static readonly Dictionary<string, string[]> AssetTypeExtensions = new() {
            { "Prefabs", new[] { "*.prefab" } },
            { "Materials", new[] { "*.mat" } },
            { "Scenes", new[] { "*.unity" } },
            { "Textures", new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.tga" } },
            { "Audio Clips", new[] { "*.mp3", "*.wav", "*.ogg" } },
            { "Shaders", new[] { "*.shader" } },
            { "Animation Clips", new[] { "*.anim" } },
            { "Scriptable Objects", new[] { "*.asset" } },
            { "Sprite Atlases", new[] { "*.spriteatlas" } },
            { "Fonts", new[] { "*.ttf", "*.otf", "*.fontsettings" } },
            { "Video Clips", new[] { "*.mp4", "*.mov", "*.webm" } }
        };

        /// <summary>
        /// Performs the full stats analysis for a given folder path.
        /// </summary>
        /// <param name="folderPath">The folder to scan (e.g. "Assets")</param>
        /// <returns>Tuple with ProjectStatsData and log string</returns>
        public static (ProjectStatsData stats, string log) Analyze(string folderPath) {
            var stats = new ProjectStatsData();
            var log = new ProjectStatsLogBuilder();

            ResetStats(stats);
            FetchMetadata(stats.Metadata);

            log.AddSection("Project Metadata");
            log.AddLine("Project Name", stats.Metadata.Name);
            log.AddLine("Project Version", stats.Metadata.Version);
            log.AddLine("Target Platform", stats.Metadata.Platform);

            AnalyzeScripts(folderPath, stats.Code, log);
            log.AddSection("Assets");

            foreach (var kvp in AssetTypeExtensions) {
                var count = AnalyzeAssets(folderPath, kvp.Value, kvp.Key, log);
                SetAssetStat(stats.Assets, kvp.Key, count);
            }

            stats.Assets.TotalSize = CalculateTotalSize(folderPath);
            return (stats, log.ToString());
        }

        /// <summary>
        /// Gathers metadata like project name, version, and platform.
        /// </summary>
        private static void FetchMetadata(ProjectMetadata metadata) {
            metadata.Name = PlayerSettings.productName;
            metadata.Version = PlayerSettings.bundleVersion;
            metadata.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        /// <summary>
        /// Clears all current stat data.
        /// </summary>
        private static void ResetStats(ProjectStatsData stats) {
            stats.Code = new CodeStats();
            stats.Assets = new AssetStats();
        }

        /// <summary>
        /// Parses script files to count lines, types, and namespaces.
        /// </summary>
        private static void AnalyzeScripts(string folderPath, CodeStats codeStats, ProjectStatsLogBuilder log) {
            var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
            codeStats.ScriptCount = files.Length;

            log.AddSection("Scripts");

            foreach (var file in files.OrderByDescending(f => new FileInfo(f).Length)) {
                var lines = File.ReadAllLines(file);
                var inBlockComment = false;

                foreach (var line in lines) {
                    codeStats.TotalLines++;
                    var trimmed = line.Trim();

                    if (inBlockComment) {
                        if (trimmed.Contains("*/")) inBlockComment = false; continue;
                    }

                    if (trimmed.StartsWith("/*")) { inBlockComment = true; continue; }
                    if (trimmed.StartsWith("//")) continue;
                    if (trimmed.StartsWith("namespace ")) codeStats.TotalNamespaces++;
                    else if (trimmed.Contains(" class ")) codeStats.ClassCount++;
                    else if (trimmed.Contains(" interface ")) codeStats.InterfaceCount++;
                    else if (trimmed.Contains(" enum ")) codeStats.EnumCount++;
                    else if (trimmed.Contains(" struct ")) codeStats.StructCount++;
                }

                var fileInfo = new FileInfo(file);
                log.AddFile(fileInfo.Name, fileInfo.Length);
            }
        }

        /// <summary>
        /// Scans and counts files by extension group (e.g., .prefab).
        /// </summary>
        private static int AnalyzeAssets(string folderPath, string[] extensions, string label, ProjectStatsLogBuilder log) {
            log.AddSection(label);

            var files = extensions
                .SelectMany(ext => Directory.GetFiles(folderPath, ext, SearchOption.AllDirectories))
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.Length)
                .ToList();

            foreach (var file in files) log.AddFile(file.Name, file.Length);
            return files.Count;
        }

        /// <summary>
        /// Calculates the total size of all assets in the folder.
        /// </summary>
        private static long CalculateTotalSize(string folderPath) {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Sum(f => new FileInfo(f).Length);
        }

        /// <summary>
        /// Updates the corresponding field in AssetStats based on label.
        /// </summary>
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