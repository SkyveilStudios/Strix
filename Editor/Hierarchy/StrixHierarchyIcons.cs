using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
#if UNITY_2020_1_OR_NEWER
using Unity.Profiling;
#endif

#if UNITY_EDITOR
namespace Strix.Editor.Hierarchy {
    [InitializeOnLoad]
    public partial class StrixHierarchyIcons {
        #region Nested Types
        private struct SystemColor {
            public Color DarkColor;
            public Color LightColor;
            
            public Color Get() => EditorGUIUtility.isProSkin ? DarkColor : LightColor;
        }
        #endregion

        #region Constants
        private const int MAX_CACHE_SIZE = 1000;
        private const int BUILTIN_ICON_CACHE_LIMIT = 100;
        private const string HIERARCHY_WINDOW_TYPE = "SceneHierarchyWindow";
        #endregion

        #region Color Definitions
        private static readonly SystemColor Default = new() {
            DarkColor = new Color(0.2196f, 0.2196f, 0.2196f),
            LightColor = new Color(0.7843f, 0.7843f, 0.7843f)
        };

        private static readonly SystemColor Selected = new() {
            DarkColor = new Color(0.1725f, 0.3647f, 0.5294f),
            LightColor = new Color(0.22745f, 0.447f, 0.6902f)
        };

        private static readonly SystemColor SelectedUnfocused = new() {
            DarkColor = new Color(0.3f, 0.3f, 0.3f),
            LightColor = new Color(0.68f, 0.68f, 0.68f)
        };

        private static readonly SystemColor Hovered = new() {
            DarkColor = new Color(0.2706f, 0.2706f, 0.2706f),
            LightColor = new Color(0.698f, 0.698f, 0.698f)
        };
        #endregion

        #region Caches
        // Cache for GameObjects and their icons
        private static readonly Dictionary<int, Texture2D> IconCache = new();
        
        // Cache for GameObject components
        private static readonly Dictionary<int, Component[]> ComponentCache = new();
        
        // Cache for built-in icons
        private static readonly Dictionary<string, Texture2D> BuiltInIconCache = new();
        
        // Cache for type namespace checks
        private static readonly Dictionary<Type, bool> TypeNamespaceCache = new();
        
        // Cache for component type to icon name mapping
        private static readonly Dictionary<Type, string> ComponentTypeIconCache = new();
        #endregion

        #region Profiler Markers
        #if UNITY_2020_1_OR_NEWER
        private static readonly ProfilerMarker ReplaceIconMarker = new("StrixHierarchy.ReplaceIcon");
        private static readonly ProfilerMarker GetIconMarker = new("StrixHierarchy.GetIcon");
        private static readonly ProfilerMarker GetComponentsMarker = new("StrixHierarchy.GetComponents");
        #endif
        #endregion

        #region Initialization
        static StrixHierarchyIcons() {
            EditorApplication.hierarchyWindowItemOnGUI += ReplaceIcon;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        #endregion

        #region Event Handlers
        private static void OnHierarchyChanged() {
            CleanupDestroyedEntries();
        }

        private static void ReplaceIcon(int instanceID, Rect selectionRect) {
            #if UNITY_2020_1_OR_NEWER
            using (ReplaceIconMarker.Auto())
            #endif
            {
                var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (go == null) return;

                // Check settings
                if (!StrixHierarchyIconsSettings.Enabled) return;

                Rect iconRect = new(selectionRect.x, selectionRect.y, 16, 16);

                // Draw background
                EditorGUI.DrawRect(iconRect, GetHierarchyBackgroundColor(selectionRect, instanceID).Get());

                // Draw icon
                var icon = GetIconForGameObject(go, instanceID);
                if (icon != null) {
                    GUI.DrawTexture(iconRect, icon);
                }
            }
        }
        #endregion

        #region Icon Resolution
        private static Texture2D GetIconForGameObject(GameObject go, int instanceID) {
            #if UNITY_2020_1_OR_NEWER
            using (GetIconMarker.Auto())
            #endif
            {
                if (go == null) return null;
                
                // Check cache first
                if (IconCache.TryGetValue(instanceID, out var cachedIcon)) {
                    return cachedIcon;
                }
                
                var components = GetCachedComponents(go, instanceID);
                Texture2D icon = null;

                // Get components in priority order
                foreach (var comp in GetComponentsByPriority(components)) {
                    if (comp == null) continue;

                    icon = ResolveComponentIcon(comp);
                    if (icon != null) break;
                }
                
                // Fallback to transform icon
                if (icon == null) {
                    icon = GetCachedBuiltInIcon("Transform Icon");
                }
                
                // Cache the result
                if (icon != null) {
                    AddToCache(IconCache, instanceID, icon);
                }
                
                return icon;
            }
        }

        private static Texture2D ResolveComponentIcon(Component comp) {
            if (comp == null) return null;
            
            var type = comp.GetType();
            
            // Try custom component icon first
            if (IsCustomComponent(type) && StrixHierarchyIconsSettings.ShowCustomIcons) {
                var content = EditorGUIUtility.ObjectContent(comp, type);
                if (content?.image is Texture2D texture) {
                    return texture;
                }
            }
            
            // Try built-in icon
            if (StrixHierarchyIconsSettings.ShowBuiltInIcons) {
                var iconName = GetBuiltInIconName(comp);
                return !string.IsNullOrEmpty(iconName) ? GetCachedBuiltInIcon(iconName) : null;
            }
            
            return null;
        }

        private static Component[] GetCachedComponents(GameObject go, int instanceID) {
            #if UNITY_2020_1_OR_NEWER
            using (GetComponentsMarker.Auto())
            #endif
            {
                if (!ComponentCache.TryGetValue(instanceID, out var components)) {
                    components = go.GetComponents<Component>();
                    AddToCache(ComponentCache, instanceID, components);
                }
                return components;
            }
        }

        private static IEnumerable<Component> GetComponentsByPriority(Component[] components) {
            // Pre-allocate lists for better performance
            var customComponents = new List<Component>(components.Length);
            var highPriorityBuiltIn = new List<Component>(components.Length);
            var remainingBuiltIn = new List<Component>(components.Length);
            
            // Single pass categorization
            foreach (var comp in components) {
                if (comp == null) continue;
                
                var type = comp.GetType();
                if (IsCustomComponent(type)) {
                    customComponents.Add(comp);
                } else if (IsHighPriorityComponent(comp)) {
                    highPriorityBuiltIn.Add(comp);
                } else {
                    remainingBuiltIn.Add(comp);
                }
            }
            
            // Efficient concatenation
            var totalCount = customComponents.Count + highPriorityBuiltIn.Count + remainingBuiltIn.Count;
            var result = new Component[totalCount];
            
            var index = 0;
            customComponents.CopyTo(result, index);
            index += customComponents.Count;
            
            highPriorityBuiltIn.CopyTo(result, index);
            index += highPriorityBuiltIn.Count;
            
            remainingBuiltIn.CopyTo(result, index);
            
            return result;
        }
        #endregion

        #region Helper Methods
        private static bool IsCustomComponent(Type type) {
            if (!TypeNamespaceCache.TryGetValue(type, out var isCustom)) {
                var ns = type.Namespace;
                isCustom = !string.IsNullOrEmpty(ns) && 
                           !ns!.StartsWith("UnityEngine") && 
                           !ns.StartsWith("UnityEditor");
                TypeNamespaceCache[type] = isCustom;
            }
            return isCustom;
        }

        private static bool IsHighPriorityComponent(Component comp) {
            return comp != null && HighPriorityComponents.Contains(comp.GetType());
        }

        private static Texture2D GetCachedBuiltInIcon(string iconName) {
            if (BuiltInIconCache.TryGetValue(iconName, out var cachedIcon)) {
                return cachedIcon;
            }
            
            var icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;
            if (icon != null && BuiltInIconCache.Count < BUILTIN_ICON_CACHE_LIMIT) {
                BuiltInIconCache[iconName] = icon;
            }
            
            return icon;
        }

        private static SystemColor GetHierarchyBackgroundColor(Rect rect, int instanceID) {
            // Check selection first (most important)
            if (Selection.instanceIDs.Contains(instanceID)) {
                var hierarchyWindow = EditorWindow.focusedWindow;
                var isFocused = hierarchyWindow != null && 
                               hierarchyWindow.GetType().Name == HIERARCHY_WINDOW_TYPE;
                return isFocused ? Selected : SelectedUnfocused;
            }
            
            // Check hover
            return rect.Contains(Event.current.mousePosition) ? Hovered : Default;
        }

        private static string GetBuiltInIconName(Component comp) {
            var type = comp.GetType();
            
            // Check cache first
            if (ComponentTypeIconCache.TryGetValue(type, out var cachedIconName)) {
                return cachedIconName;
            }
            
            // Direct type match
            if (ComponentIconNames.TryGetValue(type, out var iconName)) {
                ComponentTypeIconCache[type] = iconName;
                return iconName;
            }
            
            // Check base types and interfaces
            foreach (var kvp in ComponentIconNames) {
                if (kvp.Key.IsAssignableFrom(type)) {
                    ComponentTypeIconCache[type] = kvp.Value;
                    return kvp.Value;
                }
            }
            
            return null;
        }
        #endregion

        #region Cache Management
        private static void AddToCache<T>(Dictionary<int, T> cache, int key, T value) {
            if (cache.Count >= MAX_CACHE_SIZE) {
                // Remove first entry (simple FIFO)
                var firstKey = cache.Keys.First();
                cache.Remove(firstKey);
            }
            cache[key] = value;
        }

        private static void CleanupDestroyedEntries() {
            var keysToRemove = new List<int>();
            
            // Find destroyed objects
            foreach (var kvp in IconCache) {
                if (EditorUtility.InstanceIDToObject(kvp.Key) == null) {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            // Remove from all caches
            foreach (var key in keysToRemove) {
                IconCache.Remove(key);
                ComponentCache.Remove(key);
            }
            
            // Clear type cache if it gets too large
            if (TypeNamespaceCache.Count > 500) {
                TypeNamespaceCache.Clear();
            }
            
            // Clear component type icon cache if needed
            if (ComponentTypeIconCache.Count > 200) {
                ComponentTypeIconCache.Clear();
            }
        }
        #endregion

        #region Component Data
        // Components that should take priority when multiple components exist
        private static readonly HashSet<Type> HighPriorityComponents = new() {
            typeof(Camera),
            typeof(Light),
            typeof(Canvas),
            typeof(EventSystem),
            typeof(ParticleSystem),
            typeof(Animator),
            typeof(NavMeshAgent),
            typeof(AudioSource),
            typeof(SpriteRenderer),
            typeof(MeshRenderer),
            typeof(SkinnedMeshRenderer)
        };

        // Comprehensive list of component types and their icons
        private static readonly Dictionary<Type, string> ComponentIconNames = new() {
            // Core Unity Components
            { typeof(Camera), "Camera Icon" },
            { typeof(Light), "Light Icon" },
            { typeof(Canvas), "Canvas Icon" },
            { typeof(EventSystem), "EventSystem Icon" },
            { typeof(AudioSource), "AudioSource Icon" },
            { typeof(AudioListener), "AudioListener Icon" },
            { typeof(Rigidbody), "Rigidbody Icon" },
            { typeof(Rigidbody2D), "Rigidbody2D Icon" },
            { typeof(Collider), "BoxCollider Icon" },
            { typeof(BoxCollider), "BoxCollider Icon" },
            { typeof(SphereCollider), "SphereCollider Icon" },
            { typeof(CapsuleCollider), "CapsuleCollider Icon" },
            { typeof(MeshCollider), "MeshCollider Icon" },
            { typeof(Collider2D), "Collider2D Icon" },
            { typeof(BoxCollider2D), "BoxCollider2D Icon" },
            { typeof(CircleCollider2D), "CircleCollider2D Icon" },
            { typeof(PolygonCollider2D), "PolygonCollider2D Icon" },
            { typeof(EdgeCollider2D), "EdgeCollider2D Icon" },
            { typeof(CompositeCollider2D), "CompositeCollider2D Icon" },
            { typeof(Animator), "Animator Icon" },
            { typeof(Animation), "Animation Icon" },
            { typeof(ParticleSystem), "ParticleSystem Icon" },
            { typeof(LineRenderer), "LineRenderer Icon" },
            { typeof(TrailRenderer), "TrailRenderer Icon" },
            { typeof(MeshRenderer), "MeshRenderer Icon" },
            { typeof(SkinnedMeshRenderer), "SkinnedMeshRenderer Icon" },
            { typeof(SpriteRenderer), "SpriteRenderer Icon" },
            { typeof(Tilemap), "Tilemap Icon" },
            { typeof(TilemapRenderer), "TilemapRenderer Icon" },
            { typeof(NavMeshAgent), "NavMeshAgent Icon" },
            { typeof(NavMeshObstacle), "NavMeshObstacle Icon" },
            
            // UI Components
            { typeof(RectTransform), "RectTransform Icon" },
            { typeof(Button), "Button Icon" },
            { typeof(Image), "Image Icon" },
            { typeof(Text), "Text Icon" },
            { typeof(InputField), "InputField Icon" },
            { typeof(Toggle), "Toggle Icon" },
            { typeof(Slider), "Slider Icon" },
            { typeof(Scrollbar), "Scrollbar Icon" },
            { typeof(Dropdown), "Dropdown Icon" },
            { typeof(ScrollRect), "ScrollRect Icon" },
            { typeof(Mask), "Mask Icon" },
            { typeof(RectMask2D), "RectMask2D Icon" },
            { typeof(CanvasGroup), "CanvasGroup Icon" },
            { typeof(GraphicRaycaster), "GraphicRaycaster Icon" },
            { typeof(ContentSizeFitter), "ContentSizeFitter Icon" },
            { typeof(LayoutElement), "LayoutElement Icon" },
            { typeof(HorizontalLayoutGroup), "HorizontalLayoutGroup Icon" },
            { typeof(VerticalLayoutGroup), "VerticalLayoutGroup Icon" },
            { typeof(GridLayoutGroup), "GridLayoutGroup Icon" },
        };
        #endregion
    }

    /// <summary>
    /// Settings for Strix Hierarchy Icons
    /// </summary>
    public static class StrixHierarchyIconsSettings {
        private const string ENABLED_KEY = "StrixHierarchy.Enabled";
        private const string SHOW_CUSTOM_ICONS_KEY = "StrixHierarchy.ShowCustomIcons";
        private const string SHOW_BUILTIN_ICONS_KEY = "StrixHierarchy.ShowBuiltInIcons";
        
        public static bool Enabled {
            get => EditorPrefs.GetBool(ENABLED_KEY, true);
            set => EditorPrefs.SetBool(ENABLED_KEY, value);
        }
        
        public static bool ShowCustomIcons {
            get => EditorPrefs.GetBool(SHOW_CUSTOM_ICONS_KEY, true);
            set => EditorPrefs.SetBool(SHOW_CUSTOM_ICONS_KEY, value);
        }
        
        public static bool ShowBuiltInIcons {
            get => EditorPrefs.GetBool(SHOW_BUILTIN_ICONS_KEY, true);
            set => EditorPrefs.SetBool(SHOW_BUILTIN_ICONS_KEY, value);
        }
    }
    
    // Extension to allow external cache clearing
    public partial class StrixHierarchyIcons {
        public static void ClearAllCaches() {
            IconCache.Clear();
            ComponentCache.Clear();
            BuiltInIconCache.Clear();
            TypeNamespaceCache.Clear();
            ComponentTypeIconCache.Clear();
        }
    }
}
#endif