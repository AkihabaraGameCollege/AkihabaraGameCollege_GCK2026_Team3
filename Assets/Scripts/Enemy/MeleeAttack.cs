using ForestDraw.Combat;
using UnityEngine;
 
/// <summary>
/// 近接攻撃クラス
/// EnemyAttackBaseを継承し、
/// 一定間隔で直接ダメージを与える
/// </summary>
namespace ForestDraw.Enemy.Attack
{
    public class MeleeAttack : EnemyAttackBase
    {
        // =========================
        // 攻撃実行処理
        // =========================
        /// <summary>
        /// 攻撃処理（EnemyAttackBaseから呼ばれる）
        /// 直接ターゲットにダメージを与える
        /// </summary>
        protected override void PerformAttack()
        {
            // ターゲットが存在しない場合は何もしない
            if (target == null) return;

            // ターゲットのHPコンポーネントを取得してダメージを与える
            target.GetComponent<IDamageable>()
                  ?.TakeDamage(attackDamage);
        }
    }
}