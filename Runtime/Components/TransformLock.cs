#if UNITY_EDITOR
using UnityEngine;

namespace Strix.Runtime.Components {
    /// <summary>
    /// Locks position, rotation, and scale on a per-axis basis
    /// This is for editor-only protection and has no effect in builds
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TransformLock : MonoBehaviour {
        [Header("Position Locks")]
        public bool lockPositionX, lockPositionY, lockPositionZ;

        [Header("Rotation Locks")]
        public bool lockRotationX, lockRotationY, lockRotationZ;

        [Header("Scale Locks")]
        public bool lockScaleX, lockScaleY, lockScaleZ;

        private Vector3 _lastPosition;
        private Vector3 _lastRotation;
        private Vector3 _lastScale;

        private void OnEnable() => CacheCurrentTransform();

        private void Update() {
            if (!Application.isPlaying) {
                LockTransform();
            }
        }

        private void CacheCurrentTransform() {
            _lastPosition = transform.localPosition;
            _lastRotation = transform.localEulerAngles;
            _lastScale = transform.localScale;
        }

        private void LockTransform() {
            var pos = transform.localPosition;
            var rot = transform.localEulerAngles;
            var scale = transform.localScale;

            if (lockPositionX) pos.x = _lastPosition.x;
            if (lockPositionY) pos.y = _lastPosition.y;
            if (lockPositionZ) pos.z = _lastPosition.z;

            if (lockRotationX) rot.x = _lastRotation.x;
            if (lockRotationY) rot.y = _lastRotation.y;
            if (lockRotationZ) rot.z = _lastRotation.z;

            if (lockScaleX) scale.x = _lastScale.x;
            if (lockScaleY) scale.y = _lastScale.y;
            if (lockScaleZ) scale.z = _lastScale.z;

            transform.localPosition = pos;
            transform.localEulerAngles = rot;
            transform.localScale = scale;

            CacheCurrentTransform();
        }
        
        [ContextMenu("Lock All Axes")]
        private void LockAll() {
            lockPositionX = lockPositionY = lockPositionZ = true;
            lockRotationX = lockRotationY = lockRotationZ = true;
            lockScaleX = lockScaleY = lockScaleZ = true;
        }

        [ContextMenu("Unlock All Axes")]
        private void UnlockAll() {
            lockPositionX = lockPositionY = lockPositionZ = false;
            lockRotationX = lockRotationY = lockRotationZ = false;
            lockScaleX = lockScaleY = lockScaleZ = false;
        }
    }
}
#endif
