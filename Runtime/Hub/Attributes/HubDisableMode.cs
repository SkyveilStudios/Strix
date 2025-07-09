#if UNITY_EDITOR
using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubDisableMode : MonoBehaviour {
        [DisableInPlayMode] 
        [SerializeField] private int editableInEditModeOnly;
        [DisableInEditMode]
        [SerializeField] private float editableInPlayModeOnly;
    }
}
#endif