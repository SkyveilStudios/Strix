using System;
using UnityEditor;
using UnityEngine;
using Strix.Runtime.Attributes;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(ImagePreviewAttribute))]
    public class ImagePreviewDrawer : DecoratorDrawer {
        private Texture2D _cachedTexture;
        private string _cachedPath;

        private Texture2D GetTexture(string path) {
            if (string.IsNullOrEmpty(path)) return null;

            if (_cachedTexture && path == _cachedPath) return _cachedTexture;
            _cachedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            _cachedPath = path;
            return _cachedTexture;
        }

        public override void OnGUI(Rect position) {
            var attr = (ImagePreviewAttribute)attribute;

            if (string.IsNullOrEmpty(attr.Path)) {
                EditorGUI.LabelField(position, "No image path set.");
                return;
            }

            var texture = GetTexture(attr.Path);
            if (!texture) {
                EditorGUI.LabelField(position, $"Image not found at path: {attr.Path}");
                return;
            }

            var maxWidth = attr.FullWidth ? EditorGUIUtility.currentViewWidth - 40f : attr.Width;
            var aspect = (float)texture.width / texture.height;
            var height = maxWidth / aspect;

            var x = position.x;
            if (!attr.FullWidth) {
                switch (attr.Alignment) {
                    case ImageAlignment.Center:
                        x += (position.width - maxWidth) / 2f;
                        break;
                    case ImageAlignment.Right:
                        x += position.width - maxWidth;
                        break;
                    case ImageAlignment.Left:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var rect = new Rect(x, position.y, maxWidth, height);
            EditorGUI.DrawPreviewTexture(rect, texture, null, ScaleMode.ScaleToFit);
        }

        public override float GetHeight() {
            var attr = (ImagePreviewAttribute)attribute;
            var texture = GetTexture(attr.Path);

            if (!texture) return EditorGUIUtility.singleLineHeight;
            var width = attr.FullWidth ? EditorGUIUtility.currentViewWidth - 40f : attr.Width;
            var aspect = (float)texture.width / texture.height;
            return width / aspect + 4;

        }
    }
}