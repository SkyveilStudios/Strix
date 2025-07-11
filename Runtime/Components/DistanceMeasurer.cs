using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MeasurementTarget
{
    public Transform target;
    public Color gizmoColor = Color.yellow;
    public bool showDistance = true;
    public bool showLine = true;
    
    [Header("Display Options")]
    public bool show3DDistance = true;
    public bool showHorizontalDistance = false;
    public bool showVerticalDistance = false;
}

public class DistanceMeasurer : MonoBehaviour
{
    [Header("Measurement Targets")]
    [SerializeField] private List<MeasurementTarget> targets = new();
    
    [Header("Display Settings")]
    [SerializeField] private bool showInPlayMode = false;
    [SerializeField] private float lineWidth = 2f;
    [SerializeField] private bool showSelfPosition = true;
    [SerializeField] private Color selfPositionColor = Color.green;
    [SerializeField] private float sphereRadius = 0.1f;
    
    [Header("Text Settings")]
    [SerializeField] private float textSize = 1f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private bool showUnitSuffix = true;
    [SerializeField] private int decimalPlaces = 2;
    
    [Header("Advanced Options")]
    [SerializeField] private bool useWorldSpace = true;
    [SerializeField] private bool showMidpoint = true;
    [SerializeField] private Color midpointColor = Color.red;
    [SerializeField] private float midpointSize = 0.05f;

    public void AddTarget(Transform target)
    {
        if (target != null && !ContainsTarget(target))
        {
            targets.Add(new MeasurementTarget { target = target });
        }
    }

    public void RemoveTarget(Transform target)
    {
        targets.RemoveAll(t => t.target == target);
    }

    public bool ContainsTarget(Transform target)
    {
        return targets.Exists(t => t.target == target);
    }

    public float GetDistanceTo(Transform target)
    {
        if (target == null) return 0f;
        return Vector3.Distance(transform.position, target.position);
    }

    public Vector3 GetMidpoint(Transform target)
    {
        if (target == null) return transform.position;
        return (transform.position + target.position) * 0.5f;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && !showInPlayMode) return;
        
        DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && !showInPlayMode) return;
        
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        if (targets == null || targets.Count == 0) return;

        // Draw self position indicator
        if (showSelfPosition)
        {
            Gizmos.color = selfPositionColor;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }

        foreach (var measurementTarget in targets)
        {
            if (measurementTarget.target == null) continue;

            Vector3 startPos = transform.position;
            Vector3 endPos = measurementTarget.target.position;
            Vector3 midpoint = GetMidpoint(measurementTarget.target);

            // Draw connection line
            if (measurementTarget.showLine)
            {
                Gizmos.color = measurementTarget.gizmoColor;
                Gizmos.DrawLine(startPos, endPos);
                
                // Draw direction arrow
                Vector3 direction = (endPos - startPos).normalized;
                Vector3 arrowPos = endPos - direction * 0.2f;
                Vector3 right = Vector3.Cross(direction, Vector3.up).normalized * 0.1f;
                Vector3 up = Vector3.Cross(right, direction).normalized * 0.1f;
                
                Gizmos.DrawLine(endPos, arrowPos + right);
                Gizmos.DrawLine(endPos, arrowPos - right);
                Gizmos.DrawLine(endPos, arrowPos + up);
                Gizmos.DrawLine(endPos, arrowPos - up);
            }

            // Draw midpoint
            if (showMidpoint)
            {
                Gizmos.color = midpointColor;
                Gizmos.DrawWireSphere(midpoint, midpointSize);
            }

            // Draw target indicator
            Gizmos.color = measurementTarget.gizmoColor;
            Gizmos.DrawWireSphere(endPos, sphereRadius);

            // Distance calculations and display
            if (measurementTarget.showDistance)
            {
                string distanceText = "";
                
                if (measurementTarget.show3DDistance)
                {
                    float distance3D = Vector3.Distance(startPos, endPos);
                    string suffix = showUnitSuffix ? "u" : "";
                    distanceText += $"3D: {distance3D.ToString($"F{decimalPlaces}")}{suffix}";
                }
                
                if (measurementTarget.showHorizontalDistance)
                {
                    Vector3 horizontalStart = new Vector3(startPos.x, 0, startPos.z);
                    Vector3 horizontalEnd = new Vector3(endPos.x, 0, endPos.z);
                    float horizontalDistance = Vector3.Distance(horizontalStart, horizontalEnd);
                    string suffix = showUnitSuffix ? "u" : "";
                    if (distanceText.Length > 0) distanceText += "\n";
                    distanceText += $"H: {horizontalDistance.ToString($"F{decimalPlaces}")}{suffix}";
                }
                
                if (measurementTarget.showVerticalDistance)
                {
                    float verticalDistance = Mathf.Abs(endPos.y - startPos.y);
                    string suffix = showUnitSuffix ? "u" : "";
                    if (distanceText.Length > 0) distanceText += "\n";
                    distanceText += $"V: {verticalDistance.ToString($"F{decimalPlaces}")}{suffix}";
                }

                // Draw distance text using Gizmos (Note: This is a simplified approach)
                // For better text rendering, you might want to use Handles in the Editor script
                #if UNITY_EDITOR
                UnityEditor.Handles.color = textColor;
                Vector3 textPosition = useWorldSpace ? midpoint : midpoint + Vector3.up * 0.5f;
                UnityEditor.Handles.Label(textPosition, distanceText);
                #endif
            }
        }
    }

    // Helper method to get all measured distances
    public Dictionary<Transform, float> GetAllDistances()
    {
        Dictionary<Transform, float> distances = new Dictionary<Transform, float>();
        
        foreach (var target in targets)
        {
            if (target.target != null)
            {
                distances[target.target] = GetDistanceTo(target.target);
            }
        }
        
        return distances;
    }

    // Method to automatically find and measure to all objects with a specific tag
    public void MeasureToObjectsWithTag(string tag, Color gizmoColor = default)
    {
        if (gizmoColor == default) gizmoColor = Color.yellow;
        
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj.transform != this.transform && !ContainsTarget(obj.transform))
            {
                targets.Add(new MeasurementTarget 
                { 
                    target = obj.transform, 
                    gizmoColor = gizmoColor 
                });
            }
        }
    }
}