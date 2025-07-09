using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Strix.Editor.Hierarchy {
    [InitializeOnLoad]
    public static class StrixHierarchyConnector {
        private const float IndentWidth = 14f;
        private const float TriangleCenterOffset = 39f;
        private static readonly Dictionary<Transform, int> DepthCache = new();

        static StrixHierarchyConnector() { EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI; }

        private static void DrawVerticalLine(float x, float yTop, float yBottom) => Handles.DrawLine(new Vector2(x, yTop), new Vector2(x, yBottom));
        private static void DrawHorizontalLine(float xStart, float xEnd, float y) => Handles.DrawLine(new Vector2(xStart, y), new Vector2(xEnd, y));

        private static void OnHierarchyGUI(int instanceID, Rect rect) {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null || go.transform.parent == null) return;

            var t = go.transform;
            var depth = GetDepth(t);
            var yMid = rect.yMin + rect.height / 2f;

            Handles.color = new Color(1f, 1f, 1f, 0.25f);

            for (var i = 0; i < depth; i++) {
                var ancestor = GetAncestorAtDepth(t, i);
                if (!HasSiblingBelow(ancestor)) continue;
                var x = i * IndentWidth + TriangleCenterOffset;
                Handles.DrawLine(new Vector2(x, rect.yMin), new Vector2(x, rect.yMax));
            }

            var currentX = depth * IndentWidth + TriangleCenterOffset;
            var yEnd = HasSiblingBelow(t) ? rect.yMax : yMid;
            DrawVerticalLine(currentX, rect.yMin, yEnd);

            var xEnd = currentX + IndentWidth;
            DrawHorizontalLine(currentX, xEnd, yMid);

            DepthCache.Clear();
        }

        private static int GetDepth(Transform t) {
            if (DepthCache.TryGetValue(t, out var cached)) return cached;

            var depth = 0;
            var current = t;
            while (current.parent != null) {
                current = current.parent;
                depth++;
            }
            DepthCache[t] = depth;
            return depth;
        }

        private static Transform GetAncestorAtDepth(Transform t, int targetDepth) {
            var depth = GetDepth(t);
            while (depth > targetDepth) {
                t = t.parent;
                depth--;
            }
            return t;
        }

        private static bool HasSiblingBelow(Transform t) {
            if (t == null || t.parent == null) return false;
            return t.GetSiblingIndex() < t.parent.childCount - 1;
        }
    }
}
#endif