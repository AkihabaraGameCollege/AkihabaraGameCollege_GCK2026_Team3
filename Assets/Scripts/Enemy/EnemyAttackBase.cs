using UnityEngine;
using ForestDraw.Enemy.Data;
using ForestDraw.Enemy.Components;

/// <summary>
/// 敵の攻撃処理の基底クラス。
/// 実際の攻撃内容は継承先クラスで実装する。
/// </summary>
namespace ForestDraw.Enemy.Attack
{
    public abstract class EnemyAttackBase : MonoBehaviour
    {
        // =========================
        // ▼ 設定値
        // =========================

        protected float attackInterval;   // 攻撃間隔（秒）
        protected int attackDamage;       // 攻撃ダメージ量

        // =========================
        // ▼ 内部状態管理用
        // =========================

        protected float attackTimer = 0;  // 次の攻撃までの経過時間
        protected bool canAttack = false; // 攻撃可能状態かどうか
        protected GameObject target;      // 攻撃対象
        private EnemyMove move;
        private EnemyHealth health;

        // =========================
        // 外部から攻撃対象を設定
        // =========================
        public void SetTarget(GameObject t)
        {
            target = t;
        }
        // =========================
        // 外部からパラメータを設定
        // =========================
        public void Initialize(EnemyData data)
        {
            attackDamage = data.attackDamage;
            attackInterval = data.attackInterval;
        }

        // =========================
        // 初期処理
        // =========================
        protected virtual void Awake()
        {
            move = GetComponent<EnemyMove>();
            health = GetComponent<EnemyHealth>();
        }
        protected virtual void Start()
        {
            // EnemyMoveのゴール到達イベントを購読
            // ゴール到達時に攻撃を開始する
            if (move != null)
                move.ReachedGoal += EnableAttack;

            if (health != null)
                health.Died += StopAttack;
        }

        // =========================
        // 毎フレーム処理
        // =========================
        protected virtual void Update()
        {
            // 攻撃可能状態でなければ何もしない
            if (!canAttack || target == null) return;

            // 経過時間を加算
            attackTimer += Time.deltaTime;

            // 攻撃間隔を超えたら攻撃実行
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;   // タイマーをリセット
                PerformAttack();    // 継承先の攻撃処理
            }
        }

        // =========================
        // 攻撃開始処理
        // =========================
        protected void EnableAttack()
        {
            if (target == null) return;
            canAttack = true;
        }

        // =========================
        // 攻撃停止処理
        // =========================
        protected void StopAttack()
        {
            canAttack = false;
            attackTimer = 0f;
        }

        // =========================
        // オブジェクト削除時処理
        // =========================
        protected virtual void OnDestroy()
        {
            if (move != null)
                move.ReachedGoal -= EnableAttack;

            if (health != null)
                health.Died -= StopAttack;
        }

        // =========================
        // 継承先の攻撃処理
        // =========================
        protected abstract void PerformAttack();
    }
}