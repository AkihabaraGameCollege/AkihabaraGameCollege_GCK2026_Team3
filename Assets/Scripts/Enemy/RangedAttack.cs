using UnityEngine;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// 遠距離攻撃を行うクラス。
    /// 一定間隔で弾を生成し、ターゲットへ発射する。
    /// </summary>
    public class RangedAttack : EnemyAttackBase
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float speed = 10f;

        /// <summary>
        /// 攻撃処理を実行する
        /// </summary>
        protected override void PerformAttack()
        {
            if (target == null || bulletPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation
            );

            bullet.GetComponent<EnemyBullet>()
                  ?.Initialize(target.transform, attackDamage, speed);
        }
    }
}