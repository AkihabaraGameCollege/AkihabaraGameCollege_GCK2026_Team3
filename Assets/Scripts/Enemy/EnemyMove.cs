using UnityEngine;
using System;

public class EnemyMove : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentIndex = 0;
    [SerializeField] private float moveSpeed = 3f;
    public Action OnReachGoal;

    public void SetPath(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
    }

    void Update()
    {
        if (waypoints == null || currentIndex >= waypoints.Length) return;

        Transform target = waypoints[currentIndex];
        Vector3 targetPos = new Vector3(
       target.position.x,
       transform.position.y,
       target.position.z);

        Vector3 direction = (targetPos - transform.position).normalized;
        transform.forward = direction;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 1f)
        {
            currentIndex++;
            // ÅŒã‚Ü‚Ås‚Á‚½‚ç’Ê’m
            if (currentIndex >= waypoints.Length)
            {
                OnReachGoal?.Invoke();
            }
        }
    }
}