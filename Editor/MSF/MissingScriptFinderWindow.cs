using Strix.Editor.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Strix.Editor.MSF {
    public class MissingScriptFinderWindow : EditorWindow {
        private const string WindowTitle = "Missing Script Finder";
        private Vector2 _scrollPos;

        [MenuItem("Strix/Missing Script Finder")]
        public static void ShowWindow() {
            var window = GetWindow<MissingScriptFinderWindow>();
            window.titleContent = new GUIContent("Missing Scripts Finder", EditorGUIUtility.IconContent("Search Icon").image);
        }

        private void OnGUI() {
            EditorGUILayout.Space(10);
            StrixEditorUIUtils.DrawTitle(WindowTitle);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            StrixEditorUIUtils.DrawResponsiveButton(" Scan Selected", "d_Search Icon", MissingScriptScanner.ScanSelectedObjects, Selection.gameObjects.Length > 0, "Scan currently selected GameObjects");
            StrixEditorUIUtils.DrawResponsiveButton(" Scan Project", "d_FolderOpened Icon", MissingScriptScanner.ScanEntireProject, true, "Scan all prefabs and scenes in the project");
            StrixEditorUIUtils.DrawResponsiveButton(" Clear", "d_TreeEditor.Trash", MissingScriptScanner.Clear, MissingScriptScanner.Results.Count > 0, "Clear scan results");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);
            EditorGUILayout.HelpBox("Scan Summary", MessageType.Info);

            EditorGUILayout.BeginVertical("box");
            DrawStatRow("GameObjects Searched", MissingScriptScanner.Stats.GameObjectCount.ToString(), "Total GameObjects visited during scan");
            DrawStatRow("Components Scanned", MissingScriptScanner.Stats.ComponentCount.ToString(), "Total components scanned (including null slots)");
            DrawStatRow("Missing Scripts Found", MissingScriptScanner.Stats.MissingCount.ToString(), "Number of missing script slots found");

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField($"Missing Script Locations ({MissingScriptScanner.Results.Count})", EditorStyles.boldLabel);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            if (MissingScriptScanner.Results.Count == 0) {
                EditorGUILayout.HelpBox("No missing scripts found.", MessageType.Info);
            }
            else {
                var bgToggle = false;
                foreach (var result in MissingScriptScanner.Results) {
                    var bgColor = bgToggle ? new Color(0.2f, 0.2f, 0.2f, 0.2f) : new Color(0, 0, 0, 0);
                    bgToggle = !bgToggle;

                    var bgStyle = new GUIStyle(GUI.skin.box) {
                        normal = { background = MakeTex(1, 1, bgColor) },
                        margin = new RectOffset(2, 2, 2, 2),
                        padding = new RectOffset(5, 5, 5, 5)
                    };

                    EditorGUILayout.BeginVertical(bgStyle);
                    EditorGUILayout.BeginHorizontal();

                    var currentScene = SceneManager.GetActiveScene().path;
                    var willSwitchScene = !string.IsNullOrEmpty(result.ScenePath) && result.ScenePath != currentScene;

                    var tooltip = willSwitchScene
                        ? $"Ping and switch to scene:\n{result.ScenePath}"
                        : "Ping this object";

                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_console.infoicon").image, tooltip), GUILayout.Width(30), GUILayout.Height(20))) {
                        if (willSwitchScene) {
                            var shouldSwitch = EditorUtility.DisplayDialog(
                                "Switch Scene?",
                                $"The object is located in a different scene:\n\n{result.ScenePath}\n\nDo you want to switch to that scene?",
                                "Yes",
                                "No"
                            );

                            if (shouldSwitch && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                                EditorSceneManager.OpenScene(result.ScenePath, OpenSceneMode.Single);
                                EditorApplication.delayCall += () => EditorGUIUtility.PingObject(result.Object);
                            }
                        }
                        else {
                            EditorGUIUtility.PingObject(result.Object);
                        }
                    }

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField($"<b>{result.HierarchyPath}</b> (Slot {result.Index})", new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true });
                    EditorGUILayout.LabelField(result.FilePath, EditorStyles.miniLabel);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawStatRow(string label, string value, string tooltip = null) {
            EditorGUILayout.BeginHorizontal();
            var labelContent = new GUIContent(label, tooltip ?? label);
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(180));
            EditorGUILayout.LabelField(value, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        private static Texture2D MakeTex(int width, int height, Color col) {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; ++i)
                pix[i] = col;

            var tex = new Texture2D(width, height);
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }
}
