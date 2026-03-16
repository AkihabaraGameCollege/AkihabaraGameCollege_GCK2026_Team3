using UnityEngine;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// ゴール到達後に攻撃可能になる敵の攻撃基底クラス
    /// ・ゴール到達で攻撃開始
    /// ・死亡時に攻撃停止
    /// </summary>
    public abstract class EnemyAttackGoal : EnemyAttackBase
    {
        protected bool canAttack = false;
        [SerializeField]
        protected GoalEnemyAnimator enemyAnimator;

        /// <summary>
        /// イベント登録（ゴール到達・死亡）
        /// </summary>
        protected virtual void Start()
        {
            if (move != null)
                move.ReachedGoal += EnableAttack;

            if (health != null)
                health.Died += StopAttack;
        }

        /// <summary>
        /// 攻撃タイマー管理
        /// </summary>
        protected virtual void Update()
        {
            if (!canAttack || target == null) return;

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                enemyAnimator.PlayAttack();
            }
        }

        /// <summary>
        /// ゴール到達時に攻撃を有効化
        /// </summary>
        private void EnableAttack()
        {
            LookAtTarget();
            canAttack = true;
            enemyAnimator.EnableAttack();
        }

        /// <summary>
        /// 死亡時に攻撃を停止
        /// </summary>
        private void StopAttack()
        {
            canAttack = false;
        }

        /// <summary>
        /// イベント解除
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (move != null)
                move.ReachedGoal -= EnableAttack;

            if (health != null)
                health.Died -= StopAttack;
        }
        public void PerformAttackEvent()
        {
            PerformAttack();
        }
    }
}