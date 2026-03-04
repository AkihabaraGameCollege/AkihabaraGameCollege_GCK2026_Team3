using UnityEngine;
using ForestDraw.Enemy.Data;
using ForestDraw.Enemy.Components;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// “G‚ÌUŒ‚ˆ—‚ÌŠî’êƒNƒ‰ƒXB
    /// UŒ‚ŠÔŠuŠÇ—‚ÆUŒ‚ŠJn^’â~§Œä‚ğs‚¢A
    /// ÀÛ‚ÌUŒ‚“à—e‚ÍŒp³æ‚ÅÀ‘•‚·‚éB
    /// </summary>
    public abstract class EnemyAttackBase : MonoBehaviour
    {
        // ===== UŒ‚İ’è =====
        protected float attackInterval;
        protected int attackDamage;

        // ===== ó‘Ô =====
        protected float attackTimer = 0f;
        protected bool canAttack = false;
        protected GameObject target;

        private EnemyMove move;
        private EnemyHealth health;

        /// <summary>
        /// UŒ‚‘ÎÛ‚ğİ’è‚·‚é
        /// </summary>
        public void SetTarget(GameObject t)
        {
            target = t;
        }

        /// <summary>
        /// ScriptableObject‚©‚çUŒ‚ƒpƒ‰ƒ[ƒ^‚ğİ’è‚·‚é
        /// </summary>
        public void Initialize(EnemyData data)
        {
            attackDamage = data.attackDamage;
            attackInterval = data.attackInterval;
        }

        protected virtual void Awake()
        {
            move = GetComponent<EnemyMove>();
            health = GetComponent<EnemyHealth>();
        }

        protected virtual void Start()
        {
            if (move != null)
                move.ReachedGoal += EnableAttack;

            if (health != null)
                health.Died += StopAttack;
        }

        protected virtual void Update()
        {
            if (!canAttack || target == null) return;

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                PerformAttack();
            }
        }

        protected void EnableAttack()
        {
            if (target == null) return;
            canAttack = true;
        }

        protected void StopAttack()
        {
            canAttack = false;
            attackTimer = 0f;
        }

        protected virtual void OnDestroy()
        {
            if (move != null)
                move.ReachedGoal -= EnableAttack;

            if (health != null)
                health.Died -= StopAttack;
        }

        /// <summary>
        /// ÀÛ‚ÌUŒ‚ˆ—‚ğÀ‘•‚·‚é
        /// </summary>
        protected abstract void PerformAttack();
    }
}