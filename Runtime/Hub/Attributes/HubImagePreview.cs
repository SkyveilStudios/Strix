using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Editor.Hub.Dummy {
    public class HubImagePreview : MonoBehaviour
    {
        [ImagePreview("Assets/Strix/Banners/StrixBanner.jpg", 400f)]
        [SerializeField] private GameObject obj;
    }
}