using UnityEngine;

namespace Strix.Runtime.Components {
    /// <summary>
    /// Play an AudioSource in the Editor without entering Play Mode
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class AudioSourcePreview : MonoBehaviour {
        public bool stopOnDeselect = true;
        [HideInInspector] public bool isPreviewPlaying;
    }
}