using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
namespace Strix.Editor.Hierarchy {
    [InitializeOnLoad]
    public class StrixHierarchyIcons {
        private struct SystemColor {
            public Color DarkColor;
            public Color LightColor;
            
            public Color Get() => EditorGUIUtility.isProSkin ? DarkColor : LightColor;
        }
        
        private static SystemColor Default => new() {
            DarkColor = new Color(0.2196f, 0.2196f, 0.2196f),
            LightColor = new Color(0.7843f, 0.7843f, 0.7843f)
        };

        private static SystemColor Selected => new() {
            DarkColor = new Color(0.1725f, 0.3647f, 0.5294f),
            LightColor = new Color(0.22745f, 0.447f, 0.6902f)
        };

        private static SystemColor SelectedUnfocused => new() {
            DarkColor = new Color(0.3f, 0.3f, 0.3f),
            LightColor = new Color(0.68f, 0.68f, 0.68f)
        };

        private static SystemColor Hovered => new() {
            DarkColor = new Color(0.2706f, 0.2706f, 0.2706f),
            LightColor = new Color(0.698f, 0.698f, 0.698f)
        };

        
        static StrixHierarchyIcons() {
            EditorApplication.hierarchyWindowItemOnGUI += ReplaceIcon;
        }

        private static void ReplaceIcon(int instanceID, Rect selectionRect) {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            Rect iconRect = new(selectionRect.x, selectionRect.y, 16, 16);

            EditorGUI.DrawRect(iconRect, GetHierarchyBackgroundColor(selectionRect, instanceID).Get());


            var icon = GetIconForGameObject(go);
            if (icon != null) GUI.DrawTexture(iconRect, icon);
        }

        private static Texture2D GetIconForGameObject(GameObject go) {
            if (go == null) return null;
            
            var components = go.GetComponents<Component>();

            foreach (var comp in components) {
                if (comp == null) continue;

                var ns = comp.GetType().Namespace;
                if (string.IsNullOrEmpty(ns) || ns!.StartsWith("UnityEngine") || ns.StartsWith("UnityEditor")) {
                    var iconName = GetBuiltInIconName(comp);
                    if (string.IsNullOrEmpty(iconName)) continue;
                    var icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;
                    if (icon != null) return icon;
                }
                else {
                    var content = EditorGUIUtility.ObjectContent(comp, comp.GetType());
                    if (content?.image != null) return content.image as Texture2D;
                }
            }
            
            return EditorGUIUtility.IconContent("Transform Icon").image as Texture2D;
        }
        
        private static SystemColor GetHierarchyBackgroundColor(Rect rect, int instanceID) {
            var evt = Event.current;

            if (Selection.instanceIDs.Contains(instanceID)) {
                if (EditorWindow.focusedWindow == EditorWindow.mouseOverWindow)
                    return Selected;
                return SelectedUnfocused;
            }

            if (rect.Contains(evt.mousePosition))
                return Hovered;

            return Default;
        }

        private static string GetBuiltInIconName(Component comp) {
            return comp switch {
                Camera => "Camera Icon",
                Light => "Light Icon",
                Canvas => "Canvas Icon",
                EventSystem => "EventSystem Icon",
                AudioSource => "AudioSource Icon",
                ParticleSystem => "ParticleSystem Icon",
                TrailRenderer => "TrailRenderer Icon",
                LineRenderer => "LineRenderer Icon",
                SpriteRenderer => "SpriteRenderer Icon",
                SkinnedMeshRenderer => "SkinnedMeshRenderer Icon",
                MeshRenderer => "MeshRenderer Icon",
                MeshFilter => "MeshFilter Icon",
                Animator => "Animator Icon",
                Rigidbody => "Rigidbody Icon",
                Rigidbody2D => "Rigidbody Icon",
                BoxCollider => "BoxCollider Icon",
                BoxCollider2D => "BoxCollider2D Icon",
                CapsuleCollider => "CapsuleCollider Icon",
                CapsuleCollider2D => "CapsuleCollider2D Icon",
                CircleCollider2D => "CircleCollider2D Icon",
                CompositeCollider2D => "CompositeCollider2D Icon",
                CustomCollider2D => "CustomCollider2D Icon",
                EdgeCollider2D => "EdgeCollider2D Icon",
                MeshCollider => "d_MeshCollider Icon",
                PolygonCollider2D => "d_PolygonCollider2D Icon",
                SphereCollider => "d_SphereCollider Icon",
                TerrainCollider => "d_TerrainCollider Icon",
                TilemapCollider2D  => "d_TilemapCollider2D Icon",
                WheelCollider => "d_WheelCollider Icon",
                WheelJoint2D => "d_WheelJoint2D Icon",
                NavMeshAgent => "NavMeshAgent Icon",
                RectTransform => "RectTransform Icon",
                ReflectionProbe => "ReflectionProbe Icon",
                LightProbeGroup => "LightProbeGroup Icon",
                Terrain => "TerrainCollider Icon",
                MonoBehaviour => "cs Script Icon",
                _ => null
            };
        }
    }
}
#endif