namespace Strix.Editor.Stats {
    /// <summary>
    /// Container for all collected project stats
    /// </summary>
    public class ProjectStatsData {
        public ProjectMetadata MetaData { get; } = new();
        public CodeStats Code { get; }  = new();
        public AssetStats Assets { get; } = new();
    }

    /// <summary>
    /// Metadata about the current project
    /// </summary>
    public class ProjectMetadata {
        public string Name;
        public string Version;
        public string Platform;
    }

    /// <summary>
    /// Statistics about code structure and lines
    /// </summary>
    public class CodeStats {
        public int ScriptCount, TotalLines, TotalNamespaces, ClassCount, InterfaceCount, EnumCount, StructCount;
    }

    /// <summary>
    /// Statistics about project asset count and project size in MBs
    /// </summary>
    public class AssetStats {
        public int Prefabs, Materials, Scenes, Textures, AudioClips, Shaders, AnimClips, Scriptables, Atlases, Fonts, Videos;
        public long TotalSize;
    }
}