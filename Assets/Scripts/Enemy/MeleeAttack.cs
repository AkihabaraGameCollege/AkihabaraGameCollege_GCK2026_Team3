using ForestDraw.Combat;
using UnityEngine;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// 近接攻撃を行うクラス。
    /// 一定間隔でターゲットへ直接ダメージを与える。
    /// </summary>
    public class MeleeAttack : EnemyAttackBase
    {
        /// <summary>
        /// 攻撃処理を実行する
        /// </summary>
        protected override void PerformAttack()
        {
            if (target == null) return;

            target.GetComponent<IDamageable>()
                  ?.TakeDamage(attackDamage);
        }
    }
}