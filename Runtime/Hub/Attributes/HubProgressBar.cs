using Strix.Runtime.Attributes;
using UnityEngine;

namespace Strix.Runtime.Hub.Attributes {
    public class HubProgressBar : MonoBehaviour {
        [ProgressBar("Health", 0, 100, ProgressBarColor.Red, IsInteractable = true)]
        public float health = 50f;
    }
}