using UnityEngine;
using ForestDraw.Enemy.Data;
using ForestDraw.Enemy.Components;
using ForestDraw.Enemy.Attack;

namespace ForestDraw.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private EnemyMove move;
        private EnemyAttackBase attack;
        private EnemyHealth health;

        private void Awake()
        {
            move = GetComponent<EnemyMove>();
            attack = GetComponent<EnemyAttackBase>();
            health = GetComponent<EnemyHealth>();
        }

        public void Initialize(Transform[] path, Transform target, EnemyData data)
        {
            move.Initialize(data);
            move.SetPath(path);

            attack.Initialize(data);
            attack.SetTarget(target);

            health.Initialize(data);

            health.Died += HandleDeath;
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}