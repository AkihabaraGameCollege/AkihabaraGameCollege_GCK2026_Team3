using UnityEngine;
using System;
using ForestDraw.Enemy.Data;

namespace ForestDraw.Enemy.Components
{
    /// <summary>
    /// 敵の移動を管理するコンポーネント。
    /// Waypointに沿って移動し、最終到達時にイベントを通知する。
    /// </summary>
    public class EnemyMove : MonoBehaviour
    {
        // ===== ルート情報 =====
        private Transform[] waypoints;
        private int currentIndex = 0;

        // ===== 移動設定 =====
        private float moveSpeed;
        private float waypointRadius;

        /// <summary>
        /// ゴール到達時に発火するイベント
        /// </summary>
        public event Action ReachedGoal;

        // ===== 内部状態 =====
        private Vector3 currentTargetPos;
        private bool hasTarget = false;
        private bool hasReachedGoal = false;
        private bool isPaused = false;

        /// <summary>
        /// ScriptableObjectから移動パラメータを設定する
        /// </summary>
        public void Initialize(EnemyData data)
        {
            moveSpeed = data.moveSpeed;
            waypointRadius = data.waypointRadius;
        }

        /// <summary>
        /// 移動ルートを設定する
        /// </summary>
        public void SetPath(Transform[] newWaypoints)
        {
            waypoints = newWaypoints;
            currentIndex = 0;
            hasTarget = false;
        }

        private void Update()
        {
            if (!CanMove()) return;

            if (!hasTarget)
                GenerateRandomTarget();

            MoveToTarget();

            if (HasReachedTarget())
                AdvanceWaypoint();
        }

        private bool CanMove()
        {
            return !isPaused
                   && !hasReachedGoal
                   && waypoints != null
                   && currentIndex < waypoints.Length;
        }

        private void MoveToTarget()
        {
            Vector3 direction = (currentTargetPos - transform.position).normalized;

            transform.forward = direction;

            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTargetPos,
                moveSpeed * Time.deltaTime
            );
        }

        private bool HasReachedTarget()
        {
            return (transform.position - currentTargetPos).sqrMagnitude < 0.01f;
        }

        private void AdvanceWaypoint()
        {
            currentIndex++;
            hasTarget = false;

            if (currentIndex >= waypoints.Length)
            {
                HandleGoalReached();
            }
        }

        private void GenerateRandomTarget()
        {
            if (waypoints[currentIndex] == null)
            {
                Debug.LogError("Waypoint is null");
                return;
            }

            Transform waypoint = waypoints[currentIndex];
            Vector2 offset = GenerateRandomOffset();

            currentTargetPos = new Vector3(
                waypoint.position.x + offset.x,
                transform.position.y,
                waypoint.position.z + offset.y
            );

            hasTarget = true;
        }

        private Vector2 GenerateRandomOffset()
        {
            Vector2 random = UnityEngine.Random.insideUnitCircle * waypointRadius;

            if (IsLastWaypoint())
            {
                random.y = 0f;
            }

            return random;
        }

        private bool IsLastWaypoint()
        {
            return currentIndex == waypoints.Length - 1;
        }

        private void HandleGoalReached()
        {
            hasReachedGoal = true;
            ReachedGoal?.Invoke();
        }
        // ===== 外部制御 =====

        public void PauseMove()
        {
            isPaused = true;
        }

        public void ResumeMove()
        {
            isPaused = false;
        }
    }
}