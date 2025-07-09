using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubReadOnly : MonoBehaviour{
        [ReadOnly]
        [SerializeField] public float speed;
    }
}