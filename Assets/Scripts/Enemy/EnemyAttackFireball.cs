using UnityEngine;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// 火球を発射する敵の攻撃クラス
    /// ・移動を一時停止して攻撃
    /// ・ゴール到達で自滅
    /// ・死亡時に爆発
    /// </summary>
    public class EnemyAttackFireball : EnemyAttackMoving
    {
        [SerializeField] GameObject fireballPrefab;
        [SerializeField] GameObject explosionPrefab;
        [SerializeField] Transform firePoint;
        [SerializeField] float attackStopTime = 1f;
        [SerializeField] float speed = 10f;

        /// <summary>
        /// イベント登録（ゴール到達・死亡）
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (move != null)
                move.ReachedGoal += Suicide;

            if (health != null)
                health.Died += Explosion;
        }

        /// <summary>
        /// 攻撃開始
        /// </summary>
        protected override void PerformAttack()
        {
            StartCoroutine(AttackRoutine());
        }

        /// <summary>
        /// 攻撃中は移動停止、ターゲットを向き、火球を発射
        /// </summary>
        private System.Collections.IEnumerator AttackRoutine()
        {
            isAttacking = true;

            move.PauseMove(); // 移動停止
            LookAtTarget();   // ターゲット方向を向く

            ShootFireball();  // 火球発射

            yield return new WaitForSeconds(attackStopTime);

            move.ResumeMove(); // 移動再開
            isAttacking = false;
        }

        /// <summary>
        /// 火球生成＆初期化
        /// </summary>
        private void ShootFireball()
        {
            if (fireballPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(
                fireballPrefab,
                firePoint.position,
                firePoint.rotation
            );

            bullet.GetComponent<EnemyBullet>()
                  ?.Initialize(target, attackDamage, speed);
        }

        /// <summary>
        /// ゴール到達で自滅
        /// </summary>
        private void Suicide()
        {
            if (health != null)
                health.TakeDamage(int.MaxValue);
        }

        /// <summary>
        /// 死亡時に爆発して周囲にダメージ
        /// </summary>
        private void Explosion()
        {
            Debug.Log("爆発");

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        /// <summary>
        /// イベント解除
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (move != null)
                move.ReachedGoal -= Suicide;

            if (health != null)
                health.Died -= Explosion;
        }
    }
}