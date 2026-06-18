using UnityEngine;
using ForestDraw.Enemy.Components;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// 敵の攻撃の基底クラス
    /// ・攻撃間隔やダメージ管理
    /// ・ターゲットを向く処理
    /// </summary>
    public abstract class EnemyAttackBase : MonoBehaviour, IEnemyComponent
    {
        // ===== 攻撃設定 =====
        protected float attackInterval;
        protected int attackDamage;

        // ===== 状態管理 =====
        protected float attackTimer = 0f;
        protected Transform target;

        protected EnemyMove move;
        protected EnemyHealth health;

        /// <summary>
        /// EnemyInitContextから初期化
        /// </summary>
        public void Initialize(EnemyInitContext context)
        {
            attackDamage = context.data.attackDamage;
            attackInterval = context.data.attackInterval;
            target = context.target;
        }

        /// <summary>
        /// コンポーネント取得
        /// </summary>
        protected virtual void Awake()
        {
            move = GetComponent<EnemyMove>();
            health = GetComponent<EnemyHealth>();

            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");// プレイヤーオブジェクトをタグで検索

                if (playerObj != null)
                {
                    target = playerObj.transform;// ターゲットをプレイヤーのTransformに設定
                }
            }
        }

        /// <summary>
        /// ターゲットの方向を向く
        /// </summary>
        protected void LookAtTarget()
        {
            // ターゲットが空なら、再度Playerタグで探してみる
            if (target == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) target = p.transform;
            }

            // --- この1行を追加 ---
            if (target == null) return;

            Vector3 dir = target.position - transform.position;
            dir.y = 0;

            // 方向ベクトルがほぼゼロでなければ向きを更新
            if (dir.sqrMagnitude > 0.001f)
                transform.forward = dir.normalized;
        }

        /// <summary>
        /// 個別攻撃処理（派生クラスで実装）
        /// </summary>
        protected abstract void PerformAttack();
    }
}