using UnityEngine;

public class WaypointDebugger : MonoBehaviour
{
    [SerializeField] Color lineColor = new Color(1, 1, 1, 1);

    private void OnDrawGizmos()
    {
        Transform root = transform;

        if (root.childCount == 0) return;

        Gizmos.color = lineColor;

        for (int i = 1; i < root.childCount; i++)
        {
            var prev = root.GetChild(i - 1);
            var current = root.GetChild(i);

            Gizmos.DrawLine(prev.position, current.position);
        }
    }
}