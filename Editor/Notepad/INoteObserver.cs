#if UNITY_EDITOR
namespace Strix.Editor.Notepad
{
    public interface INoteObserver
    {
        void ModelUpdated();
    }
}
#endif