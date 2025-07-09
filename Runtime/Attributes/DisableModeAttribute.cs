using System;
using UnityEngine;

namespace Strix.Runtime.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableInPlayModeAttribute : PropertyAttribute {}
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableInEditModeAttribute : PropertyAttribute {}
}
