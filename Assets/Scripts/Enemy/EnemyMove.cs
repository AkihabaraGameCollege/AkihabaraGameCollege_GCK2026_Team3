using UnityEngine;
using System;

namespace ForestDraw.Enemy.Components
{
    /// <summary>
    /// 敵の移動を管理するコンポーネント。
    /// ・Waypointに沿って移動
    /// ・最終到達時にイベントを通知
    /// </summary>
    public class EnemyMove : MonoBehaviour, IEnemyComponent
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
        /// 初期化（パラメータとルートを設定）
        /// </summary>
        public void Initialize(EnemyInitContext context)
        {
            moveSpeed = context.data.moveSpeed;
            waypointRadius = context.data.waypointRadius;
            waypoints = context.path;
            currentIndex = 0;
            hasTarget = false;
        }

        private void Update()
        {
            if (!CanMove()) return;

            if (!hasTarget)
                GenerateRandomTarget(); // 次のWaypointを決める

            MoveToTarget();

            if (HasReachedTarget())
                AdvanceWaypoint(); // Waypoint更新
        }

        /// <summary>
        /// 移動可能か判定
        /// </summary>
        private bool CanMove()
        {
            return !isPaused
                   && !hasReachedGoal
                   && waypoints != null
                   && currentIndex < waypoints.Length;
        }

        /// <summary>
        /// 現在のターゲットに向かって移動
        /// </summary>
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

        /// <summary>
        /// ターゲットに到達したか判定
        /// </summary>
        private bool HasReachedTarget()
        {
            return (transform.position - currentTargetPos).sqrMagnitude < 0.01f;
        }

        /// <summary>
        /// Waypointを進める
        /// </summary>
        private void AdvanceWaypoint()
        {
            currentIndex++;
            hasTarget = false;

            if (currentIndex >= waypoints.Length)
            {
                HandleGoalReached(); // ゴール到達処理
            }
        }

        /// <summary>
        /// 次のWaypointに向かう位置を決定
        /// </summary>
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

        /// <summary>
        /// Waypoint位置にランダムオフセットを加える
        /// </summary>
        private Vector2 GenerateRandomOffset()
        {
            Vector2 random = UnityEngine.Random.insideUnitCircle * waypointRadius;

            // 最終WaypointではY方向オフセットを無効に
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

        /// <summary>
        /// ゴール到達時の処理
        /// </summary>
        private void HandleGoalReached()
        {
            hasReachedGoal = true;
            ReachedGoal?.Invoke();
        }

        // ===== 外部制御 =====

        /// <summary>
        /// 移動一時停止
        /// </summary>
        public void PauseMove()
        {
            isPaused = true;
        }

        /// <summary>
        /// 移動再開
        /// </summary>
        public void ResumeMove()
        {
            isPaused = false;
        }
    }
}