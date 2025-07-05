using UnityEngine;

namespace Strix.Editor.MSF {
    internal readonly struct ScanResults {
        public readonly GameObject Object;
        public readonly int Index;
        public readonly string HierarchyPath;
        public readonly string ScenePath;
        public readonly string FilePath;

        public ScanResults(GameObject obj, int index, string path, string scenePath, string filePath) {
            Object = obj;
            Index = index;
            HierarchyPath = path;
            ScenePath = scenePath;
            FilePath = filePath;
        }
    }
}