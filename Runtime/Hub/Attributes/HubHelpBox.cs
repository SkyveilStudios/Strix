using Strix.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubHelpBox : MonoBehaviour {
        [HelpBox("This field is optional and used for debugging only", MessageType.Info)]
        [SerializeField] private string debugNote;
    }
}