using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubRequired : MonoBehaviour {
        [Required]
        [SerializeField] public GameObject obj;
    }
}