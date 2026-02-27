using UnityEngine;

public class RangedAttack : EnemyAttackBase
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    protected override void PerformAttack()
    {
        if (target == null) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        bullet.GetComponent<Bullet>()
              ?.Initialize(target, attackDamage);  
    }
}