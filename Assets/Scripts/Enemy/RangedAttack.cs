using UnityEngine;

/// <summary>
/// 遠距離攻撃クラス
/// EnemyAttackBaseを継承し、
/// 一定間隔で弾を発射する
/// </summary>
public class RangedAttack : EnemyAttackBase
{
    // =========================
    // ▼ 弾設定
    // =========================
    [SerializeField] private GameObject bulletPrefab; // 発射する弾Prefab
    [SerializeField] private Transform firePoint;     // 発射位置
    [SerializeField] private float speed = 10f;       // 弾の移動速度

    // =========================
    // 攻撃実行処理
    // =========================
    /// <summary>
    /// 攻撃処理（EnemyAttackBaseから呼ばれる）
    /// </summary>
    protected override void PerformAttack()
    {
        // ターゲットが存在しなければ攻撃しない
        if (target == null) return;

        // 弾を生成
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        // 弾にターゲット・ダメージ・速度を設定
        bullet.GetComponent<Bullet>()
              ?.Initialize(target, attackDamage, speed);
    }
}