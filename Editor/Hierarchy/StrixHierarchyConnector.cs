using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using Unity.Profiling;
#endif

#if UNITY_EDITOR
namespace Strix.Editor.Hierarchy {
    [InitializeOnLoad]
    public static class StrixHierarchyConnector {
        #region Enums and Constants
        public enum LineStyle { 
            Solid, 
            Dotted, 
            Dashed, 
            DoubleLine,
            Minimal,
            Heavy,
            Zigzag
        }
        
        private const string ENABLED_KEY = "Strix.Hierarchy.ShowLines";
        private const string LINE_STYLE_KEY = "Strix.Hierarchy.LineStyle";
        private const string LINE_OPACITY_KEY = "Strix.Hierarchy.LineOpacity";
        private const string LINE_THICKNESS_KEY = "Strix.Hierarchy.LineThickness";
        private const float INDENT_WIDTH = 14f;
        private const float TRIANGLE_CENTER_OFFSET = 39f;
        private const float DEFAULT_OPACITY = 0.25f;
        private const float DEFAULT_THICKNESS = 1f;
        private const int CACHE_CLEANUP_THRESHOLD = 100;
        #endregion

        #region Caches
        private static readonly Dictionary<Transform, int> DepthCache = new();
        private static readonly Dictionary<Transform, bool> SiblingCache = new();
        private static readonly Dictionary<Transform, Transform[]> AncestorCache = new();
        private static int _cacheAccessCount;
        
        // Style caches
        private static LineStyle? _cachedLineStyle;
        private static float? _cachedOpacity;
        private static float? _cachedThickness;
        #endregion

        #region Profiler Markers
        #if UNITY_2020_1_OR_NEWER
        private static readonly ProfilerMarker OnGUIMarker = new("StrixHierarchyConnector.OnGUI");
        private static readonly ProfilerMarker DrawLinesMarker = new("StrixHierarchyConnector.DrawLines");
        private static readonly ProfilerMarker DepthCalculationMarker = new("StrixHierarchyConnector.DepthCalc");
        #endif
        #endregion

        #region Initialization
        static StrixHierarchyConnector() { 
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        #endregion

        #region Properties
        public static bool Enabled {
            get => EditorPrefs.GetBool(ENABLED_KEY, true);
            set {
                EditorPrefs.SetBool(ENABLED_KEY, value);
                _cachedLineStyle = null;
                _cachedOpacity = null;
            }
        }

        public static LineStyle CurrentLineStyle {
            get {
                if (!_cachedLineStyle.HasValue) {
                    _cachedLineStyle = (LineStyle)EditorPrefs.GetInt(LINE_STYLE_KEY, 0);
                }
                return _cachedLineStyle.Value;
            }
            set {
                EditorPrefs.SetInt(LINE_STYLE_KEY, (int)value);
                _cachedLineStyle = value;
            }
        }

        public static float LineOpacity {
            get {
                if (!_cachedOpacity.HasValue) {
                    _cachedOpacity = EditorPrefs.GetFloat(LINE_OPACITY_KEY, DEFAULT_OPACITY);
                }
                return _cachedOpacity.Value;
            }
            set {
                EditorPrefs.SetFloat(LINE_OPACITY_KEY, Mathf.Clamp01(value));
                _cachedOpacity = value;
            }
        }
        
        public static float LineThickness {
            get {
                if (!_cachedThickness.HasValue) {
                    _cachedThickness = EditorPrefs.GetFloat(LINE_THICKNESS_KEY, DEFAULT_THICKNESS);
                }
                return _cachedThickness.Value;
            }
            set {
                EditorPrefs.SetFloat(LINE_THICKNESS_KEY, Mathf.Clamp(value, 0.5f, 3f));
                _cachedThickness = value;
            }
        }
        #endregion

        #region Event Handlers
        private static void OnHierarchyChanged() {
            // Clear caches when hierarchy changes
            CleanupCaches();
        }

        private static void OnHierarchyGUI(int instanceID, Rect rect) {
            #if UNITY_2020_1_OR_NEWER
            using (OnGUIMarker.Auto())
            #endif
            {
                if (!Enabled) return;
                
                var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (go == null || go.transform.parent == null) return;

                DrawHierarchyLines(go.transform, rect);
                
                // Periodic cache cleanup
                if (++_cacheAccessCount > CACHE_CLEANUP_THRESHOLD) {
                    CleanupCaches();
                    _cacheAccessCount = 0;
                }
            }
        }
        #endregion

        #region Drawing Methods
        private static void DrawHierarchyLines(Transform transform, Rect rect) {
            #if UNITY_2020_1_OR_NEWER
            using (DrawLinesMarker.Auto())
            #endif
            {
                var depth = GetCachedDepth(transform);
                var yMid = rect.yMin + rect.height / 2f;

                Handles.color = new Color(1f, 1f, 1f, LineOpacity);

                var style = CurrentLineStyle;
                
                // Draw vertical lines for ancestors
                DrawAncestorLines(transform, rect, depth, style);

                // Draw current item's lines
                DrawCurrentLines(transform, rect, depth, yMid, style);
            }
        }

        private static void DrawAncestorLines(Transform transform, Rect rect, int depth, LineStyle style) {
            var ancestors = GetCachedAncestors(transform, depth);
            
            for (var i = 0; i < depth; i++) {
                if (i < ancestors.Length && GetCachedHasSiblingBelow(ancestors[i])) {
                    var x = i * INDENT_WIDTH + TRIANGLE_CENTER_OFFSET;
                    DrawStyledLine(new Vector2(x, rect.yMin), new Vector2(x, rect.yMax), style);
                }
            }
        }

        private static void DrawCurrentLines(Transform transform, Rect rect, int depth, float yMid, LineStyle style) {
            var currentX = depth * INDENT_WIDTH + TRIANGLE_CENTER_OFFSET;
            var hasSiblingBelow = GetCachedHasSiblingBelow(transform);
            var yEnd = hasSiblingBelow ? rect.yMax : yMid;

            // Vertical line
            DrawStyledLine(new Vector2(currentX, rect.yMin), new Vector2(currentX, yEnd), style);
            
            // Horizontal line
            DrawStyledLine(new Vector2(currentX, yMid), new Vector2(currentX + INDENT_WIDTH, yMid), style);
        }

        private static void DrawStyledLine(Vector2 start, Vector2 end, LineStyle style) {
            var length = Vector2.Distance(start, end);
            if (length <= 0.1f) return;

            var isVertical = Mathf.Approximately(start.x, end.x);
            var thickness = LineThickness;

            switch (style) {
                case LineStyle.Solid:
                    DrawSolidLine(start, end, thickness);
                    break;
                    
                case LineStyle.Dotted:
                    // True dotted line with circular dots
                    if (isVertical) {
                        DrawDottedLineVertical(start, end, thickness);
                    } else {
                        DrawDottedLineHorizontal(start, end, thickness);
                    }
                    break;
                    
                case LineStyle.Dashed:
                    // Proper dashed line with longer segments
                    Handles.DrawDottedLine(start, end, isVertical ? 6f : 8f);
                    break;
                    
                case LineStyle.DoubleLine:
                    DrawDoubleLine(start, end, thickness, isVertical);
                    break;
                    
                case LineStyle.Minimal:
                    // Only draw horizontal lines and short vertical connectors
                    if (!isVertical) {
                        DrawSolidLine(start, end, thickness * 0.5f);
                    } else if (length < INDENT_WIDTH) {
                        DrawSolidLine(start, end, thickness * 0.5f);
                    }
                    break;
                    
                case LineStyle.Heavy:
                    DrawSolidLine(start, end, thickness * 2f);
                    break;
                    
                case LineStyle.Zigzag:
                    if (isVertical) {
                        DrawZigzagLineVertical(start, end, thickness);
                    } else {
                        DrawSolidLine(start, end, thickness);
                    }
                    break;
            }
        }
        
        private static void DrawSolidLine(Vector2 start, Vector2 end, float thickness) {
            if (thickness <= 1f) {
                Handles.DrawLine(start, end);
            } else {
                // Draw multiple lines for thickness
                var perpendicular = Vector3.Cross(end - start, Vector3.forward).normalized;
                var offset = perpendicular * thickness * 0.5f;
                
                for (float i = -thickness * 0.5f; i <= thickness * 0.5f; i += 0.5f) {
                    var offsetVec = perpendicular * i;
                    Handles.DrawLine(start + (Vector2)offsetVec, end + (Vector2)offsetVec);
                }
            }
        }

        private static void DrawDottedLineVertical(Vector2 start, Vector2 end, float thickness) {
            var dotSpacing = 4f;
            var dotSize = thickness * 1.5f;
            var length = Vector2.Distance(start, end);
            var dots = Mathf.FloorToInt(length / dotSpacing);
            
            for (int i = 0; i <= dots; i++) {
                var t = i / (float)dots;
                var pos = Vector2.Lerp(start, end, t);
                DrawDot(pos, dotSize);
            }
        }
        
        private static void DrawDottedLineHorizontal(Vector2 start, Vector2 end, float thickness) {
            var dotSpacing = 3f;
            var dotSize = thickness * 1.5f;
            var length = Vector2.Distance(start, end);
            var dots = Mathf.FloorToInt(length / dotSpacing);
            
            for (int i = 0; i <= dots; i++) {
                var t = i / (float)dots;
                var pos = Vector2.Lerp(start, end, t);
                DrawDot(pos, dotSize);
            }
        }
        
        private static void DrawDot(Vector2 center, float size) {
            var segments = 8;
            var angleStep = 360f / segments;
            
            for (int i = 0; i < segments; i++) {
                var angle1 = i * angleStep * Mathf.Deg2Rad;
                var angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
                
                var p1 = center + new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * size * 0.5f;
                var p2 = center + new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)) * size * 0.5f;
                
                Handles.DrawLine(p1, p2);
            }
        }
        
        private static void DrawDoubleLine(Vector2 start, Vector2 end, float thickness, bool isVertical) {
            var offset = isVertical ? new Vector2(2f, 0) : new Vector2(0, 2f);
            DrawSolidLine(start - offset, end - offset, thickness * 0.5f);
            DrawSolidLine(start + offset, end + offset, thickness * 0.5f);
        }

        private static void DrawCurvedLine(Vector2 start, Vector2 end, float thickness) {
            var midPoint = (start + end) * 0.5f;
            var controlPoint = new Vector2(start.x, midPoint.y);
            
            // Draw bezier curve
            var segments = 10;
            for (int i = 0; i < segments; i++) {
                var t1 = i / (float)segments;
                var t2 = (i + 1) / (float)segments;
                
                var p1 = GetBezierPoint(start, controlPoint, end, t1);
                var p2 = GetBezierPoint(start, controlPoint, end, t2);
                
                DrawSolidLine(p1, p2, thickness);
            }
        }
        
        private static Vector2 GetBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
            var u = 1 - t;
            return u * u * p0 + 2 * u * t * p1 + t * t * p2;
        }

        private static void DrawZigzagLineVertical(Vector2 start, Vector2 end, float thickness) {
            var amplitude = 2f;
            var frequency = 8f;
            var length = Vector2.Distance(start, end);
            var segments = Mathf.CeilToInt(length / 2f);
            
            for (int i = 0; i < segments; i++) {
                var t1 = i / (float)segments;
                var t2 = (i + 1) / (float)segments;
                
                var y1 = Mathf.Lerp(start.y, end.y, t1);
                var y2 = Mathf.Lerp(start.y, end.y, t2);
                
                var x1 = start.x + Mathf.Sin(t1 * frequency) * amplitude;
                var x2 = start.x + Mathf.Sin(t2 * frequency) * amplitude;
                
                Handles.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2));
            }
        }
        #endregion

        #region Cache Methods
        private static int GetCachedDepth(Transform t) {
            #if UNITY_2020_1_OR_NEWER
            using (DepthCalculationMarker.Auto())
            #endif
            {
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
        }

        private static Transform[] GetCachedAncestors(Transform t, int depth) {
            if (AncestorCache.TryGetValue(t, out var cached)) return cached;

            var ancestors = new Transform[depth];
            var current = t;
            
            for (var i = depth - 1; i >= 0; i--) {
                current = current.parent;
                ancestors[i] = current;
            }
            
            AncestorCache[t] = ancestors;
            return ancestors;
        }

        private static bool GetCachedHasSiblingBelow(Transform t) {
            if (SiblingCache.TryGetValue(t, out var cached)) return cached;

            var result = t != null && t.parent != null && t.GetSiblingIndex() < t.parent.childCount - 1;
            SiblingCache[t] = result;
            return result;
        }

        private static void CleanupCaches() {
            // Remove destroyed transforms
            var toRemove = new List<Transform>();
            
            foreach (var key in DepthCache.Keys) {
                if (key == null) toRemove.Add(key);
            }
            
            foreach (var key in toRemove) {
                DepthCache.Remove(key);
                SiblingCache.Remove(key);
                AncestorCache.Remove(key);
            }
        }
        #endregion

        #region Public API
        public static void ClearAllCaches() {
            DepthCache.Clear();
            SiblingCache.Clear();
            AncestorCache.Clear();
            _cachedLineStyle = null;
            _cachedOpacity = null;
            _cacheAccessCount = 0;
        }
        #endregion
    }
}
#endif