using UnityEngine;

public class MeleeAttack : EnemyAttackBase
{
    protected override void PerformAttack()
    {
        if (target == null) return;

        Debug.Log("Melee Attack!");

        //target.GetComponent<PlayerHealth>()
        //      ?.TakeDamage(attackDamage);
    }
}