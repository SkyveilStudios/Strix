using System.Text;

namespace Strix.Editor.Stats {
    /// <summary>
    /// Builds a formatted log report for project stats.
    /// </summary>
    internal class ProjectStatsLogBuilder {
        private readonly StringBuilder _builder = new();

        /// <summary>
        /// Adds a section header to the log.
        /// </summary>
        public void AddSection(string title) {
            _builder.AppendLine();
            _builder.AppendLine($"{title}:");
        }

        /// <summary>
        /// Adds a label-value pair line.
        /// </summary>
        public void AddLine(string label, string value) {
            _builder.AppendLine($"{label}: {value}");
        }

        /// <summary>
        /// Adds a line showing file name and size.
        /// </summary>
        public void AddFile(string name, long sizeBytes) {
            _builder.AppendLine($"{name} - {FormatSize(sizeBytes)}");
        }

        /// <summary>
        /// Returns the full log as a string.
        /// </summary>
        public override string ToString() => _builder.ToString();

        /// <summary>
        /// Converts bytes to MB string.
        /// </summary>
        private static string FormatSize(long bytes) => $"{bytes / 1024f / 1024f:0.00} MB";
    }
}