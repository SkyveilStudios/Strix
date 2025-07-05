namespace Strix.Editor.MSF {
    internal class ScanStats {
        public int GameObjectCount;
        public int ComponentCount;
        public int MissingCount;

        public void Clear() {
            GameObjectCount = 0;
            ComponentCount = 0;
            MissingCount = 0;
        }
    }
}