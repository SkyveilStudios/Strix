using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Strix.Editor.MSF {
    internal static class MissingScriptScanner {
        public static readonly List<ScanResults> Results = new();
        public static readonly ScanStats Stats = new();

        public static void Clear() {
            Stats.Clear();
            Results.Clear();
        }

        public static void ScanSelectedObjects() {
            Clear();
            foreach (var go in Selection.gameObjects) {
                Scan(go);
            }

            Debug.Log($"[Missing Scripts Finder] Searched {Stats.GameObjectCount} GameObjects, " +
                      $"{Stats.ComponentCount} components, found {Stats.MissingCount} missing scripts.");
        }

        public static void ScanEntireProject() {
            Clear();
            foreach (var prefab in GetAllPrefabs()) {
                Scan(prefab);
            }

            var skipped = 0;
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");
            foreach (var guid in sceneGuids) {
                var scenePath = AssetDatabase.GUIDToAssetPath(guid);
                if (!scenePath.StartsWith("Assets/")) {
                    skipped++;
                    continue;
                }

                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                foreach (var root in scene.GetRootGameObjects()) {
                    Scan(root);
                }

                if (scene.isLoaded && scene != SceneManager.GetActiveScene()) {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log($"[Missing Scripts Finder] Searched {Stats.GameObjectCount} GameObjects, " +
                      $"{Stats.ComponentCount} components, found {Stats.MissingCount} missing scripts. " +
                      $"Skipped {skipped} non-editable scenes.");
        }

        private static void Scan(GameObject go) {
            Stats.GameObjectCount++;
            var components = go.GetComponents<Component>();

            for (var i = 0; i < components.Length; i++) {
                Stats.ComponentCount++;
                if (components[i]) continue;

                Stats.MissingCount++;
                var path = GetFullPath(go.transform);
                var scenePath = go.scene.path;
                var filePath = PrefabUtility.IsPartOfPrefabAsset(go)
                    ? AssetDatabase.GetAssetPath(go)
                    : scenePath;

                Results.Add(new ScanResults(go, i, path, scenePath, filePath));
            }

            foreach (Transform child in go.transform) {
                Scan(child.gameObject);
            }
        }

        private static string GetFullPath(Transform t) {
            var path = t.name;
            while (t.parent) {
                t = t.parent;
                path = $"{t.name}/{path}";
            }

            return path;
        }

        private static IEnumerable<GameObject> GetAllPrefabs() {
            return from path in AssetDatabase.GetAllAssetPaths()
                where path.EndsWith(".prefab") && path.StartsWith("Assets/")
                let go = AssetDatabase.LoadAssetAtPath<GameObject>(path)
                where go
                select go;
        }
    }
}