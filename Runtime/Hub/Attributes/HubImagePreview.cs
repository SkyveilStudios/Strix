#if UNITY_EDITOR
using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubImagePreview : MonoBehaviour
    {
        [ImagePreview("Assets/Strix/Banners/StrixBanner.jpg", 400f, false)]
        [SerializeField] private GameObject obj;
    }
}
#endif