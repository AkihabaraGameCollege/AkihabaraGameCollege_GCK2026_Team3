using UnityEngine;
using System;
using ForestDraw.Enemy.Data;

/// <summary>
/// 敵の移動処理クラス
/// ・Waypointに沿って移動
/// ・Waypoint周囲にランダム揺らぎを追加
/// ・最終到達時にイベント通知
/// </summary>
namespace ForestDraw.Enemy.Components
{
    public class EnemyMove : MonoBehaviour
    {
        // =========================
        // ▼ ルート情報
        // =========================
        private Transform[] waypoints;   // 移動ルート配列
        private int currentIndex = 0;    // 現在向かっているWaypoint番号

        // =========================
        // ▼ 移動パラメータ
        // =========================
        private float moveSpeed;       // 移動速度
        private float waypointRadius;  // Waypoint周囲のランダム半径

        // =========================
        // ▼ イベント
        // =========================
        public event Action ReachedGoal;  // ゴール到達時に発火

        // =========================
        // ▼ 内部状態
        // =========================
        private Vector3 currentTargetPos; // 現在の目標地点
        private bool hasTarget = false;   // 目標地点生成済みフラグ
        private Transform target;         // ゴール到達時に向く対象（例：MainCamera）
        private bool hasReachedGoal = false; //移動終了フラグ

        // =========================
        // 外部から移動パラメータを設定
        // =========================
        public void Initialize(EnemyData data)
        {
            moveSpeed = data.moveSpeed;
            waypointRadius = data.waypointRadius;
        }

        // =========================
        // 外部からルートを設定
        // =========================
        public void SetPath(Transform[] newWaypoints)
        {
            waypoints = newWaypoints;
            currentIndex = 0;
            hasTarget = false;
        }

        // =========================
        // 初期処理
        // =========================
        private void Start()
        {
            // MainCameraをターゲットとして取得
            if (Camera.main != null)
            {
                target = Camera.main.transform;
            }
        }

        // =========================
        // 毎フレーム処理
        // =========================
        private void Update()
        {
            if (!CanMove()) return;

            if (!hasTarget)
                GenerateRandomTarget();

            MoveToTarget();

            if (HasReachedTarget())
                AdvanceWaypoint();
        }

        // =========================
        // 移動可能か判定
        // =========================
        private bool CanMove()
        {
            return !hasReachedGoal
                   && waypoints != null
                   && currentIndex < waypoints.Length;
        }

        // =========================
        // 目標地点へ移動
        // =========================
        private void MoveToTarget()
        {
            // 進行方向を計算
            Vector3 direction = (currentTargetPos - transform.position).normalized;

            // 向きを進行方向へ変更
            transform.forward = direction;

            // 指定速度で移動
            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTargetPos,
                moveSpeed * Time.deltaTime
            );
        }

        // =========================
        // 目標地点に到達したか判定
        // =========================
        private bool HasReachedTarget()
        {
            return (transform.position - currentTargetPos).sqrMagnitude < 0.01f;
        }

        // =========================
        // Waypointを次へ進める
        // =========================
        private void AdvanceWaypoint()
        {
            currentIndex++;
            hasTarget = false;

            // 全Waypoint通過でゴール
            if (currentIndex >= waypoints.Length)
            {
                HandleGoalReached();
            }
        }

        // =========================
        // ランダム目標地点生成
        // =========================
        private void GenerateRandomTarget()
        {
            if (waypoints[currentIndex] == null)
            {
                Debug.LogError("Waypoint is null");
                return;
            }
            Transform waypoint = waypoints[currentIndex];

            Vector2 offset = GenerateRandomOffset();

            // Y座標固定（XZ平面移動）
            currentTargetPos = new Vector3(
                waypoint.position.x + offset.x,
                transform.position.y,
                waypoint.position.z + offset.y
            );

            hasTarget = true;
        }

        // =========================
        // ランダムオフセット生成
        // 最終WaypointのみZ方向ランダム無効
        // =========================
        private Vector2 GenerateRandomOffset()
        {
            Vector2 random = UnityEngine.Random.insideUnitCircle * waypointRadius;

            if (IsLastWaypoint())
            {
                random.y = 0f; // Z方向ランダムを無効化
            }

            return random;
        }

        // =========================
        // 最終Waypointか判定
        // =========================
        private bool IsLastWaypoint()
        {
            return currentIndex == waypoints.Length - 1;
        }

        // =========================
        // ゴール到達時処理
        // =========================
        private void HandleGoalReached()
        {
            hasReachedGoal = true;
            LookAtTarget();
            ReachedGoal?.Invoke();
        }

        // =========================
        // ターゲット方向を向く（Y軸固定）
        // =========================
        private void LookAtTarget()
        {
            if (target == null) return;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.forward = direction.normalized;
            }
        }
    }
}