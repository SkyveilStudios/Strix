using System.Text;

namespace Strix.Editor.Stats {
    /// <summary>
    /// Returns a formatted log
    /// </summary>
    internal class ProjectStatsLogBuilder {
        private readonly StringBuilder _builder = new();

        /// <summary>
        /// Adds a header to the log
        /// </summary>
        /// <param name="title"></param>
        public void AddSection(string title) {
            _builder.AppendLine();
            _builder.AppendLine($"{title}:");
        }

        /// <summary>
        /// Adds a label value
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        public void AddLine(string label, string value) {
            _builder.AppendLine($"{label}: {value}");
        }

        /// <summary>
        /// Adds file name and size
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sizeBytes"></param>
        public void AddFile(string name, long sizeBytes) {
            _builder.AppendLine($"{name}: - {FormatSize(sizeBytes)}");
        }
        
        /// <summary>
        /// Returns the full log
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _builder.ToString();

        /// <summary>
        /// Converts bytes to MB
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatSize(long bytes) => $"{bytes / 1024f / 1024f:00} MB";
    }
}