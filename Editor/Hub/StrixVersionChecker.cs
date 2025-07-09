using System;
using Strix.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace Strix.Editor.Hub {
    internal static class StrixVersionChecker {
        public static void CheckForUpdateFromHub(bool showIfUpToDate = false) {
            GitHubReleaseChecker.CheckForUpdate((latestTag, releasePage, unityPackageUrl) => {
                var current = ParseVersion(StrixVersionInfo.CurrentVersion);
                var latest = ParseVersion(latestTag);
                
                if (latest >  current) {
                    EditorApplication.delayCall += () => {
                        var choice = EditorUtility.DisplayDialogComplex(
                            "Strix Update Available",
                            $"A new version ({latestTag}) is available.\n\nCurrent version: {StrixVersionInfo.CurrentVersion}",
                            "Download & Import",
                            "Later",
                            "View on GitHub"
                        );

                        switch (choice)
                        {
                            case 0:
                                GitHubReleaseChecker.DownloadAndImportPackage(unityPackageUrl);
                                break;
                            case 2:
                                Application.OpenURL(releasePage);
                                break;
                            case 1:
                            default:
                                break;
                        }
                    };
                } else if (showIfUpToDate) {
                    EditorApplication.delayCall += () => {
                        EditorUtility.DisplayDialog(
                            "Strix is Up To Date",
                            $"You're already using the latest version: {StrixVersionInfo.CurrentVersion}",
                            "OK"
                        );
                    };
                }
            });
        }
        
        private static Version ParseVersion(string tag) {
            return Version.TryParse(tag.TrimStart('v', 'V'), out var version) ? version : new Version(0, 0, 0);
        }
    }
}