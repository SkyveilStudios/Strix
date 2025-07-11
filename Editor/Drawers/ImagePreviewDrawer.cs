#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Strix.Runtime.Attributes;

namespace Strix.Editor.Drawers {
    [CustomPropertyDrawer(typeof(ImagePreviewAttribute))]
    public class ImagePreviewDrawer : DecoratorDrawer {
        
        #region Constants
        private const float DefaultPadding = 4f;
        private const float BorderWidth = 2f;
        private const float ShadowOffset = 2f;
        private const float AnimationSpeed = 8f;
        private const float HoverScale = 1.05f;
        private const float FadeSpeed = 4f;
        private const int MaxCacheSize = 50;
        private const float FallbackViewWidth = 300f; // Fallback when currentViewWidth is unavailable
        #endregion

        #region Static Cache
        private static readonly Dictionary<string, CachedTexture> TextureCache = new();
        private static readonly Dictionary<string, bool> PathValidationCache = new();
        private static readonly List<string> CacheAccessOrder = new();
        private static GUIStyle _errorStyle;
        private static GUIStyle _backgroundStyle;
        private static bool _stylesInitialized;
        private static float _lastKnownViewWidth = FallbackViewWidth;
        #endregion

        #region Instance Fields
        private float _animationTime;
        private bool _isHovered;
        private float _currentScale = 1f;
        private float _fadeAlpha = 1f;
        private Rect _lastRect;
        private bool _showTooltip;
        private string _tooltipText;
        private float _cachedHeight = -1f;
        private string _lastHeightCalculationPath;
        #endregion

        #region Properties
        private ImagePreviewAttribute Attribute => (ImagePreviewAttribute)attribute;
        #endregion

        #region Public Methods
        public override void OnGUI(Rect position) {
            if (!_stylesInitialized) {
                InitializeStyles();
            }

            // Cache the current view width for GetHeight calculations
            try {
                _lastKnownViewWidth = EditorGUIUtility.currentViewWidth;
            } catch {
                // Fallback if currentViewWidth is not available
                _lastKnownViewWidth = position.width > 0 ? position.width + 40f : FallbackViewWidth;
            }

            var attr = Attribute;
            UpdateAnimation();
            
            if (string.IsNullOrEmpty(attr.Path)) {
                DrawError(position, "No image path specified");
                return;
            }

            var cachedTexture = GetCachedTexture(attr.Path);
            if (cachedTexture == null || !cachedTexture.IsValid) {
                DrawError(position, $"Image not found: {Path.GetFileName(attr.Path)}");
                return;
            }

            var layout = CalculateLayout(position, cachedTexture.Texture, attr);
            HandleInteraction(layout.imageRect);
            DrawImageWithEffects(layout, cachedTexture.Texture, attr);
            
            if (_showTooltip && !string.IsNullOrEmpty(_tooltipText)) {
                DrawTooltip(layout.imageRect);
            }
        }

        public override float GetHeight() {
            var attr = Attribute;
            
            // Use cached height if available and path hasn't changed
            if (_cachedHeight > 0 && attr.Path == _lastHeightCalculationPath) {
                return _cachedHeight;
            }

            var cachedTexture = GetCachedTexture(attr.Path);
            
            if (cachedTexture == null || !cachedTexture.IsValid) {
                _cachedHeight = EditorGUIUtility.singleLineHeight + DefaultPadding;
                _lastHeightCalculationPath = attr.Path;
                return _cachedHeight;
            }

            var width = CalculateDisplayWidthSafe(attr);
            var aspect = (float)cachedTexture.Texture.width / cachedTexture.Texture.height;
            var height = width / aspect;
            
            _cachedHeight = height + attr.Padding * 2f + (attr.ShowBorder ? BorderWidth * 2f : 0f);
            _lastHeightCalculationPath = attr.Path;
            
            return _cachedHeight;
        }
        #endregion

        #region Private Methods - Initialization
        private void InitializeStyles() {
            _errorStyle = new GUIStyle(EditorStyles.helpBox) {
                normal = { textColor = Color.red },
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter
            };

            _backgroundStyle = new GUIStyle(GUI.skin.box) {
                normal = { background = CreateColorTexture(new Color(0.2f, 0.2f, 0.2f, 0.8f)) }
            };

            _stylesInitialized = true;
        }

        private Texture2D CreateColorTexture(Color color) {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        #endregion

        #region Private Methods - Caching
        private CachedTexture GetCachedTexture(string path) {
            if (string.IsNullOrEmpty(path)) return null;

            // Check cache first
            if (TextureCache.TryGetValue(path, out var cached)) {
                UpdateCacheAccess(path);
                return cached;
            }

            // Load new texture
            var texture = LoadTextureFromPath(path);
            var cachedTexture = new CachedTexture(texture, path);
            
            // Manage cache size
            if (TextureCache.Count >= MaxCacheSize) {
                RemoveOldestCacheEntry();
            }

            TextureCache[path] = cachedTexture;
            UpdateCacheAccess(path);
            
            return cachedTexture;
        }

        private Texture2D LoadTextureFromPath(string path) {
            // Try AssetDatabase first
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null) return texture;

            // Try loading from Resources
            if (path.StartsWith("Assets/Resources/")) {
                var resourcePath = path.Replace("Assets/Resources/", "").Replace(".png", "").Replace(".jpg", "");
                return Resources.Load<Texture2D>(resourcePath);
            }

            // Try direct file loading for absolute paths
            if (File.Exists(path)) {
                var bytes = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2);
                if (tex.LoadImage(bytes)) {
                    return tex;
                }
            }

            return null;
        }

        private void UpdateCacheAccess(string path) {
            CacheAccessOrder.Remove(path);
            CacheAccessOrder.Add(path);
        }

        private void RemoveOldestCacheEntry() {
            if (CacheAccessOrder.Count > 0) {
                var oldestPath = CacheAccessOrder[0];
                TextureCache.Remove(oldestPath);
                CacheAccessOrder.RemoveAt(0);
            }
        }
        #endregion

        #region Private Methods - Layout
        private ImageLayout CalculateLayout(Rect position, Texture2D texture, ImagePreviewAttribute attr) {
            var displayWidth = CalculateDisplayWidth(attr);
            var aspect = (float)texture.width / texture.height;
            var displayHeight = displayWidth / aspect;

            var x = CalculateXPosition(position, displayWidth, attr);
            var y = position.y + attr.Padding;

            var imageRect = new Rect(x, y, displayWidth, displayHeight);
            var borderRect = attr.ShowBorder ? 
                new Rect(imageRect.x - BorderWidth, imageRect.y - BorderWidth, 
                         imageRect.width + BorderWidth * 2, imageRect.height + BorderWidth * 2) : 
                imageRect;

            return new ImageLayout {
                imageRect = imageRect,
                borderRect = borderRect,
                shadowRect = attr.ShowShadow ? 
                    new Rect(imageRect.x + ShadowOffset, imageRect.y + ShadowOffset, 
                             imageRect.width, imageRect.height) : 
                    imageRect
            };
        }

        private float CalculateDisplayWidth(ImagePreviewAttribute attr) {
            if (attr.FullWidth) {
                return _lastKnownViewWidth - 40f;
            }
            return attr.Width;
        }

        private float CalculateDisplayWidthSafe(ImagePreviewAttribute attr) {
            if (attr.FullWidth) {
                // Use cached view width to avoid GUI context issues
                return _lastKnownViewWidth - 40f;
            }
            return attr.Width;
        }

        private float CalculateXPosition(Rect position, float displayWidth, ImagePreviewAttribute attr) {
            if (attr.FullWidth) return position.x;

            return attr.Alignment switch {
                ImageAlignment.Center => position.x + (position.width - displayWidth) * 0.5f,
                ImageAlignment.Right => position.x + position.width - displayWidth,
                ImageAlignment.Left => position.x,
                _ => position.x
            };
        }
        #endregion

        #region Private Methods - Interaction
        private void HandleInteraction(Rect imageRect) {
            var currentEvent = Event.current;
            if (currentEvent == null) return; // Safety check for GUI context
            
            var wasHovered = _isHovered;
            _isHovered = imageRect.Contains(currentEvent.mousePosition);

            if (_isHovered != wasHovered) {
                _animationTime = 0f;
                EditorApplication.delayCall += () => {
                    if (EditorWindow.focusedWindow != null) {
                        EditorWindow.focusedWindow.Repaint();
                    }
                };
            }

            if (_isHovered) {
                var attr = Attribute;
                _showTooltip = attr.ShowTooltip;
                _tooltipText = attr.TooltipText ?? $"Image: {Path.GetFileName(attr.Path)}";
                
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0) {
                    if (attr.ClickAction != ClickAction.None) {
                        HandleClick(attr);
                    }
                }
            } else {
                _showTooltip = false;
            }

            _lastRect = imageRect;
        }

        private void HandleClick(ImagePreviewAttribute attr) {
            switch (attr.ClickAction) {
                case ClickAction.SelectInProject:
                    var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(attr.Path);
                    if (asset != null) {
                        Selection.activeObject = asset;
                        EditorGUIUtility.PingObject(asset);
                    }
                    break;
                case ClickAction.OpenInEditor:
                    EditorUtility.OpenWithDefaultApp(attr.Path);
                    break;
                case ClickAction.ShowInExplorer:
                    EditorUtility.RevealInFinder(attr.Path);
                    break;
            }
        }
        #endregion

        #region Private Methods - Drawing
        private void DrawImageWithEffects(ImageLayout layout, Texture2D texture, ImagePreviewAttribute attr) {
            // Apply scaling animation
            var scaledRect = ApplyScaling(layout.imageRect);
            var scaledBorderRect = ApplyScaling(layout.borderRect);
            var scaledShadowRect = ApplyScaling(layout.shadowRect);

            // Draw shadow
            if (attr.ShowShadow) {
                var shadowColor = new Color(0f, 0f, 0f, 0.3f * _fadeAlpha);
                EditorGUI.DrawRect(scaledShadowRect, shadowColor);
            }

            // Draw border
            if (attr.ShowBorder) {
                var borderColor = attr.BorderColor;
                borderColor.a *= _fadeAlpha;
                EditorGUI.DrawRect(scaledBorderRect, borderColor);
            }

            // Draw background
            if (attr.ShowBackground) {
                var backgroundColor = attr.BackgroundColor;
                backgroundColor.a *= _fadeAlpha;
                EditorGUI.DrawRect(scaledRect, backgroundColor);
            }

            // Draw the actual image
            var oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, _fadeAlpha);
            
            if (attr.Tint != Color.white) {
                GUI.color = attr.Tint;
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, GUI.color.a * _fadeAlpha);
            }

            var scaleMode = attr.ScaleMode;
            EditorGUI.DrawPreviewTexture(scaledRect, texture, null, scaleMode);
            
            GUI.color = oldColor;

            // Draw overlay effects
            if (attr.ShowOverlay && _isHovered) {
                var overlayColor = new Color(1f, 1f, 1f, 0.1f * _fadeAlpha);
                EditorGUI.DrawRect(scaledRect, overlayColor);
            }
        }

        private Rect ApplyScaling(Rect rect) {
            if (Mathf.Approximately(_currentScale, 1f)) return rect;

            var center = rect.center;
            var scaledSize = rect.size * _currentScale;
            return new Rect(center - scaledSize * 0.5f, scaledSize);
        }

        private void DrawError(Rect position, string message) {
            var errorRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(errorRect, message, _errorStyle);
        }

        private void DrawTooltip(Rect imageRect) {
            if (string.IsNullOrEmpty(_tooltipText)) return;

            var tooltipSize = _backgroundStyle.CalcSize(new GUIContent(_tooltipText));
            var tooltipRect = new Rect(
                imageRect.x + imageRect.width - tooltipSize.x - 5f,
                imageRect.y + 5f,
                tooltipSize.x + 10f,
                tooltipSize.y + 4f
            );

            GUI.Box(tooltipRect, _tooltipText, _backgroundStyle);
        }
        #endregion

        #region Private Methods - Animation
        private void UpdateAnimation() {
            if (Event.current == null) return; // Safety check for GUI context
            
            _animationTime += Time.deltaTime * AnimationSpeed;
            
            var targetScale = _isHovered ? HoverScale : 1f;
            _currentScale = Mathf.Lerp(_currentScale, targetScale, Time.deltaTime * AnimationSpeed);
            
            var targetAlpha = 1f;
            _fadeAlpha = Mathf.Lerp(_fadeAlpha, targetAlpha, Time.deltaTime * FadeSpeed);

            if (_isHovered || !Mathf.Approximately(_currentScale, 1f)) {
                EditorApplication.delayCall += () => {
                    if (EditorWindow.focusedWindow != null) {
                        EditorWindow.focusedWindow.Repaint();
                    }
                };
            }
        }
        #endregion

        #region Nested Types
        private class CachedTexture {
            public Texture2D Texture { get; }
            public string Path { get; }
            public bool IsValid => Texture != null;

            public CachedTexture(Texture2D texture, string path) {
                Texture = texture;
                Path = path;
            }
        }

        private struct ImageLayout {
            public Rect imageRect;
            public Rect borderRect;
            public Rect shadowRect;
        }
        #endregion
    }
}
#endif