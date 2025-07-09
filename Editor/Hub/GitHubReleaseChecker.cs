using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Strix.Editor.Utilities;
using UnityEditor;
using UnityEngine.Networking;

namespace Strix.Editor.Hub {
    public static class GitHubReleaseChecker {
        private const string RepoApiUrl = "https://api.github.com/repos/SkyveilStudios/Strix/releases/latest";

        public static void CheckForUpdate(System.Action<string, string, string> onSuccess) {
            _ = FetchLatestReleaseAsync(onSuccess);
        }

        private static async Task FetchLatestReleaseAsync(System.Action<string, string, string> onSuccess) {
            using var request = UnityWebRequest.Get(RepoApiUrl);
            request.SetRequestHeader("User-Agent", "UnityEditor");

            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success) {
                var rawJson = request.downloadHandler.text;
                var dict = Json.Deserialize(rawJson) as Dictionary<string, object>;

                var tag = dict!["tag_name"] as string;
                var htmlUrl = dict["html_url"] as string;
                string unityPackageUrl = null;

                if (dict.TryGetValue("assets", out var assetsObj) && assetsObj is List<object> assets) {
                    foreach (var a in assets) {
                        if (a is not Dictionary<string, object> assetDict ||
                            !assetDict.TryGetValue("name", out var nameObj) ||
                            nameObj is not string name ||
                            !name.EndsWith(".unitypackage") ||
                            !assetDict.TryGetValue("browser_download_url", out var urlObj)) continue;
                        unityPackageUrl = urlObj as string;
                        break;
                    }
                }
                onSuccess?.Invoke(tag, htmlUrl, unityPackageUrl);
            }
            else StrixLogger.LogWarning("GitHub version check failed: " + request.error);
        }

        public static async void DownloadAndImportPackage(string downloadUrl) {
            if (string.IsNullOrEmpty(downloadUrl)) {
                StrixLogger.LogWarning("No unitypackage download URL provided.");
                return;
            }

            var tempPath = Path.Combine("Temp", "Strix_Update.unitypackage");

            using var request = UnityWebRequest.Get(downloadUrl);
            request.SetRequestHeader("User-Agent", "UnityEditor");
            request.downloadHandler = new DownloadHandlerFile(tempPath);

            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success) {
                StrixLogger.Log("Downloaded update to " + tempPath);
                AssetDatabase.ImportPackage(tempPath, true);
            }
            else StrixLogger.LogError("Download failed: " + request.error);
        }
    }
}