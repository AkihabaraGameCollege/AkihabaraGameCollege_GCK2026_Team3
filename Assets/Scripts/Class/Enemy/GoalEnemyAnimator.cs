using ForestDraw.Enemy.Attack;
using UnityEngine;

namespace ForestDraw.Enemy
{
    public class GoalEnemyAnimator : MonoBehaviour
    {
        Animator animator;

        static readonly int attackStart = Animator.StringToHash("CanAttack");
        static readonly int attack = Animator.StringToHash("IsAttack");

        [SerializeField]
        private EnemyAttackGoal attackComponent;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void EnableAttack()
        {
            animator.SetTrigger(attackStart);
        }

        public void PlayAttack()
        {
            animator.SetTrigger(attack);
        }

        // AnimationEvent‚©‚çŒÄ‚Î‚ê‚é
        public void AttackEvent()
        {
            attackComponent?.PerformAttackEvent();
        }
    }
}