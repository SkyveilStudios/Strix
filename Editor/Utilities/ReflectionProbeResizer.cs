#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Utilities
{
    public class ReflectionProbeResizer : EditorWindow {
        [MenuItem("Strix/Auto Resize Reflection Probe")]
        private static void ResizeProbe() {
            if (Selection.activeGameObject == null || !Selection.activeGameObject.TryGetComponent<ReflectionProbe>(out var probe)) {
                StrixLogger.LogWarning("Select a Reflection Probe to auto expand.");
                return;
            }

            var origin = probe.transform.position;
            const float maxDistance = 50f;
            LayerMask mask = ~0;

            var directions = new[] {
                Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back
            };

            var distances = new float[6];

            for (var i = 0; i < directions.Length; i++) {
                if (Physics.Raycast(origin, directions[i], out var hit, maxDistance, mask)) {
                    distances[i] = hit.distance;
                }
                else {
                    distances[i] = maxDistance;
                }
            }
            
            var newSize = new Vector3(
                distances[0] + distances[1],
                distances[2] + distances[3],
                distances[4] + distances[5]);
            
            var centerOffset = new Vector3(
                (distances[0] - distances[1]) * 0.5f,
                (distances[2] - distances[3]) * 0.5f,
                (distances[4] - distances[5]) * 0.5f);
            
            probe.size = newSize;
            probe.center = centerOffset;
            
            StrixLogger.Log("Reflection Probe resized to fit the room.");
        }
    }
}
#endif