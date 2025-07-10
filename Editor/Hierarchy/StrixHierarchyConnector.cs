using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Strix.Editor.Hierarchy {
    [InitializeOnLoad]
    public static class StrixHierarchyConnector {
        private enum LineStyle { Solid, Dotted, Dashed }
        private const string LineStyleKey = "Strix.Hierarchy.LineStyle";

        private const float IndentWidth = 14f;
        private const float TriangleCenterOffset = 39f;
        private static readonly Dictionary<Transform, int> DepthCache = new();

        static StrixHierarchyConnector() { EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI; }
        
        private static void DrawStyledLine(Vector2 start, Vector2 end, LineStyle style) {
            var length = Vector2.Distance(start, end);
            if (length <= 0.1f) return;

            var isVertical = Mathf.Approximately(start.x, end.x);

            switch (style) {
                case LineStyle.Solid:
                    Handles.DrawLine(start, end);
                    break;
                case LineStyle.Dotted:
                    Handles.DrawDottedLine(start, end, isVertical ? 3.5f : 2.5f);
                    break;
                case LineStyle.Dashed:
                    Handles.DrawDottedLine(start, end, isVertical ? 4.5f : 6f);
                    break;
            }
        }

        private static void OnHierarchyGUI(int instanceID, Rect rect) {
            if (!EditorPrefs.GetBool("Strix.Hierarchy.ShowLines", true)) return;
            
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null || go.transform.parent == null) return;

            var t = go.transform;
            var depth = GetDepth(t);
            var yMid = rect.yMin + rect.height / 2f;

            Handles.color = new Color(1f, 1f, 1f, 0.25f);

            var style = (LineStyle)EditorPrefs.GetInt(LineStyleKey, 0);
            for (var i = 0; i < depth; i++) {
                var ancestor = GetAncestorAtDepth(t, i);
                if (!HasSiblingBelow(ancestor)) continue;
                var x = i * IndentWidth + TriangleCenterOffset;
                DrawStyledLine(new Vector2(x, rect.yMin), new Vector2(x, rect.yMax), style);
            }

            var currentX = depth * IndentWidth + TriangleCenterOffset;
            var yEnd = HasSiblingBelow(t) ? rect.yMax : yMid;

            DrawStyledLine(new Vector2(currentX, rect.yMin), new Vector2(currentX, yEnd), (LineStyle)EditorPrefs.GetInt(LineStyleKey, 0));
            DrawStyledLine(new Vector2(currentX, yMid), new Vector2(currentX + IndentWidth, yMid), (LineStyle)EditorPrefs.GetInt(LineStyleKey, 0));

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