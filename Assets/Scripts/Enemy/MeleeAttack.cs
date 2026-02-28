using UnityEngine;

public class MeleeAttack : EnemyAttackBase
{
    protected override void PerformAttack()
    {
        if (target == null) return;

        target.GetComponent<KariPlayerHealth>()
              ?.TakeDamage(attackDamage);
    }
}