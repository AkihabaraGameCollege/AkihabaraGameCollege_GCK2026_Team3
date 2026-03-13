using UnityEngine;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// ˆÚ“®‚µ‚È‚ª‚çUŒ‚‚·‚é“G‚ÌUŒ‚Šî’êƒNƒ‰ƒX
    /// EˆÚ“®’†‚Å‚àˆê’èŠÔŠu‚ÅUŒ‚
    /// E€–S‚ÉUŒ‚’â~
    /// </summary>
    public abstract class EnemyAttackMoving : EnemyAttackBase
    {
        protected bool isAttacking = false;

        /// <summary>
        /// €–SƒCƒxƒ“ƒg“o˜^
        /// </summary>
        protected virtual void Start()
        {
            if (health != null)
                health.Died += StopAttack;
        }

        /// <summary>
        /// UŒ‚ƒ^ƒCƒ}[ŠÇ—
        /// </summary>
        protected virtual void Update()
        {
            if (target == null || isAttacking) return;

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                PerformAttack(); // ÀÛ‚ÌUŒ‚ˆ—‚Í”h¶ƒNƒ‰ƒX‚ÅÀ‘•
            }
        }

        /// <summary>
        /// €–S‚ÉUŒ‚‚ğ’â~
        /// </summary>
        private void StopAttack()
        {
            isAttacking = true;
        }

        /// <summary>
        /// ƒCƒxƒ“ƒg‰ğœ
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (health != null)
                health.Died -= StopAttack;
        }
    }
}