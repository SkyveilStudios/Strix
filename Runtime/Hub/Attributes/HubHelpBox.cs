#if UNITY_EDITOR
using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubHelpBox : MonoBehaviour {
        [HelpBox("This field is optional and used for debugging only")]
        [SerializeField] private string debugNote;
    }
}
#endif