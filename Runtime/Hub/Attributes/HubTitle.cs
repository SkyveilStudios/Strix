#if UNITY_EDITOR
using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubTitle : MonoBehaviour {
        [Title("Settings", TitleColor.Cyan, TitleColor.Gray, 2f, 1f, true)] 
        [SerializeField] private int health;
        
        [Title("References")]
        [SerializeField] private GameObject player;
    }
}
#endif