#if UNITY_EDITOR
using System.Collections.Generic;
using Strix.Editor.Utilities;
using UnityEditor;
using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using Unity.Profiling;
#endif

namespace Strix.Editor.Hierarchy {
    [InitializeOnLoad]
    public static class StrixHierarchyButtons {
        #region Constants
        private const float BUTTON_WIDTH = 18f;
        private const float BUTTON_SPACING = 2f;
        private const string LOCK_FLAG_PREFIX = "Strix_Hierarchy_Lock_";
        private const string SETTINGS_KEY = "Strix.Hierarchy.ShowButtons";
        private const int CACHE_CLEANUP_THRESHOLD = 100;
        #endregion

        #region Cached Content
        private static readonly GUIContent LockIcon = new(EditorGUIUtility.IconContent("LockIcon-On")) { tooltip = "Unlock GameObject" };
        private static readonly GUIContent UnlockIcon = new(EditorGUIUtility.IconContent("LockIcon")) { tooltip = "Lock GameObject" };
        private static readonly GUIContent ActiveOnIcon = new(EditorGUIUtility.IconContent("d_scenevis_visible_hover@2x")) { tooltip = "Hide GameObject" };
        private static readonly GUIContent ActiveOffIcon = new(EditorGUIUtility.IconContent("d_scenevis_hidden_hover@2x")) { tooltip = "Show GameObject" };
        
        private static GUIStyle _buttonStyle;
        private static GUIStyle ButtonStyle {
            get {
                if (_buttonStyle == null) {
                    _buttonStyle = new GUIStyle(GUI.skin.button) {
                        padding = new RectOffset(1, 1, 1, 1),
                        margin = new RectOffset(0, 0, 0, 0),
                        overflow = new RectOffset(0, 0, 0, 0),
                        fixedWidth = 0,
                        fixedHeight = 0
                    };
                }
                return _buttonStyle;
            }
        }
        #endregion

        #region Caches
        // Cache for locked state to avoid repeated EditorPrefs lookups
        private static readonly Dictionary<int, bool> LockedStateCache = new();
        
        // Cache for GameObject references to avoid repeated lookups
        private static readonly Dictionary<int, GameObject> GameObjectCache = new();
        
        // Track when we need to clean up the cache
        private static int _cacheAccessCount = 0;
        #endregion

        #region Profiler Markers
        #if UNITY_2020_1_OR_NEWER
        private static readonly ProfilerMarker OnGUIMarker = new("StrixHierarchyButtons.OnGUI");
        private static readonly ProfilerMarker LockCheckMarker = new("StrixHierarchyButtons.LockCheck");
        #endif
        #endregion

        #region Initialization
        static StrixHierarchyButtons() {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            RegisterLockProtection();
        }

        [InitializeOnLoadMethod]
        private static void RegisterLockProtection() {
            ObjectChangeEvents.changesPublished += OnObjectChangesPublished;
        }
        #endregion

        #region GUI Rendering
        private static void OnGUI(int instanceID, Rect rect) {
            #if UNITY_2020_1_OR_NEWER
            using (OnGUIMarker.Auto())
            #endif
            {
                // Early exit if buttons are disabled
                if (!ShowButtons) return;

                var obj = GetCachedGameObject(instanceID);
                if (obj == null) return;

                DrawButtons(obj, rect);
            }
        }
        
        private static void DrawButtons(GameObject obj, Rect rect) {
            var instanceID = obj.GetInstanceID();
            var x = rect.xMax - BUTTON_SPACING;
            
            // Active/Inactive button
            x -= BUTTON_WIDTH;
            var activeRect = new Rect(x, rect.yMin + 1, BUTTON_WIDTH, rect.height - 2);
            if (DrawButton(activeRect, obj.activeSelf ? ActiveOnIcon : ActiveOffIcon)) {
                Undo.RecordObject(obj, obj.activeSelf ? "Deactivate GameObject" : "Activate GameObject");
                obj.SetActive(!obj.activeSelf);
            }
            
            // Lock/Unlock button
            x -= BUTTON_WIDTH;
            var lockRect = new Rect(x, rect.yMin + 1, BUTTON_WIDTH, rect.height - 2);
            var isLocked = GetCachedLockState(instanceID);
            
            if (DrawButton(lockRect, isLocked ? LockIcon : UnlockIcon)) {
                SetLockState(obj, !isLocked);
            }
        }

        private static bool DrawButton(Rect rect, GUIContent content) {
            return GUI.Button(rect, content, ButtonStyle);
        }
        #endregion

        #region Lock State Management
        private static bool GetCachedLockState(int instanceID) {
            #if UNITY_2020_1_OR_NEWER
            using (LockCheckMarker.Auto())
            #endif
            {
                if (!LockedStateCache.TryGetValue(instanceID, out var isLocked)) {
                    isLocked = EditorPrefs.GetBool(LOCK_FLAG_PREFIX + instanceID, false);
                    LockedStateCache[instanceID] = isLocked;
                    
                    // Periodically clean up the cache
                    if (++_cacheAccessCount > CACHE_CLEANUP_THRESHOLD) {
                        CleanupCaches();
                        _cacheAccessCount = 0;
                    }
                }
                return isLocked;
            }
        }

        public static void SetLockState(GameObject obj, bool locked) {
            var instanceID = obj.GetInstanceID();
            var key = LOCK_FLAG_PREFIX + instanceID;
            
            if (locked) {
                EditorPrefs.SetBool(key, true);
                obj.hideFlags |= HideFlags.NotEditable;
            } else {
                EditorPrefs.DeleteKey(key);
                obj.hideFlags &= ~HideFlags.NotEditable;
            }
            
            LockedStateCache[instanceID] = locked;
            EditorUtility.SetDirty(obj);
            EditorApplication.RepaintHierarchyWindow();
            SceneView.RepaintAll();
        }

        public static bool IsLocked(GameObject obj) {
            return obj != null && GetCachedLockState(obj.GetInstanceID());
        }
        #endregion

        #region Event Handlers
        private static void OnObjectChangesPublished(ref ObjectChangeEventStream stream) {
            for (var i = 0; i < stream.length; i++) {
                var eventType = stream.GetEventType(i);

                switch (eventType) {
                    case ObjectChangeKind.ChangeGameObjectStructure:
                        stream.GetChangeGameObjectStructureEvent(i, out var structureEvent);
                        HandleObjectChange(structureEvent.instanceId);
                        break;

                    case ObjectChangeKind.ChangeGameObjectOrComponentProperties:
                        stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var propertiesEvent);
                        HandleObjectChange(propertiesEvent.instanceId);
                        break;
                }
            }
        }

        private static void HandleObjectChange(int instanceId) {
            var obj = GetCachedGameObject(instanceId);
            if (obj != null && IsLocked(obj)) {
                StrixLogger.LogWarning($"'{obj.name}' is locked and cannot be modified.");
                
                // Reapply lock state to ensure it's not bypassed
                obj.hideFlags |= HideFlags.NotEditable;
                EditorUtility.SetDirty(obj);
            }
        }

        private static void OnHierarchyChanged() {
            // Only process visible GameObjects to improve performance
            var visibleObjects = GetVisibleGameObjectsInHierarchy();
            
            foreach (var obj in visibleObjects) {
                if (obj == null) continue;
                
                var instanceID = obj.GetInstanceID();
                if (GetCachedLockState(instanceID)) {
                    obj.hideFlags |= HideFlags.NotEditable;
                } else {
                    obj.hideFlags &= ~HideFlags.NotEditable;
                }
            }
            
            // Clean up deleted objects from cache
            CleanupDeletedObjects();
        }
        #endregion

        #region Cache Management
        private static GameObject GetCachedGameObject(int instanceID) {
            if (!GameObjectCache.TryGetValue(instanceID, out var obj) || obj == null) {
                obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (obj != null) {
                    GameObjectCache[instanceID] = obj;
                }
            }
            return obj;
        }

        private static void CleanupCaches() {
            // Remove entries for destroyed objects
            var keysToRemove = new List<int>();
            
            foreach (var kvp in LockedStateCache) {
                if (EditorUtility.InstanceIDToObject(kvp.Key) == null) {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove) {
                LockedStateCache.Remove(key);
                GameObjectCache.Remove(key);
                
                // Also clean up EditorPrefs for destroyed objects
                EditorPrefs.DeleteKey(LOCK_FLAG_PREFIX + key);
            }
        }

        private static void CleanupDeletedObjects() {
            var keysToRemove = new List<int>();
            
            foreach (var kvp in GameObjectCache) {
                if (kvp.Value == null) {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove) {
                GameObjectCache.Remove(key);
                LockedStateCache.Remove(key);
            }
        }

        private static GameObject[] GetVisibleGameObjectsInHierarchy() {
            // Use more efficient method to get only root objects and traverse manually
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            var visibleObjects = new List<GameObject>();
            
            foreach (var root in rootObjects) {
                if (root.activeInHierarchy) {
                    CollectVisibleObjects(root.transform, visibleObjects);
                }
            }
            
            return visibleObjects.ToArray();
        }

        private static void CollectVisibleObjects(Transform transform, List<GameObject> list) {
            list.Add(transform.gameObject);
            
            // Only traverse children if the object is expanded in hierarchy
            for (var i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                if (child.gameObject.activeInHierarchy) {
                    CollectVisibleObjects(child, list);
                }
            }
        }
        #endregion

        #region Public API
        public static void ClearAllCaches() {
            LockedStateCache.Clear();
            GameObjectCache.Clear();
            _cacheAccessCount = 0;
            _buttonStyle = null;
        }

        public static void UnlockAll() {
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in allObjects) {
                if (IsLocked(obj)) {
                    SetLockState(obj, false);
                }
            }
        }

        public static bool ShowButtons {
            get => EditorPrefs.GetBool(SETTINGS_KEY, true);
            set => EditorPrefs.SetBool(SETTINGS_KEY, value);
        }
        #endregion
    }

    #if UNITY_EDITOR
    public static class StrixHierarchyButtonsMenu {
        [MenuItem("GameObject/Strix/Lock Selected", false, 20)]
        private static void LockSelected() {
            foreach (var obj in Selection.gameObjects) {
                StrixHierarchyButtons.SetLockState(obj, true);
            }
        }

        [MenuItem("GameObject/Strix/Unlock Selected", false, 21)]
        private static void UnlockSelected() {
            foreach (var obj in Selection.gameObjects) {
                StrixHierarchyButtons.SetLockState(obj, false);
            }
        }

        [MenuItem("GameObject/Strix/Unlock All", false, 22)]
        private static void UnlockAll() {
            StrixHierarchyButtons.UnlockAll();
        }

        [MenuItem("GameObject/Strix/Lock Selected", true)]
        [MenuItem("GameObject/Strix/Unlock Selected", true)]
        private static bool ValidateSelection() {
            return Selection.gameObjects.Length > 0;
        }
    }
    #endif
}
#endif