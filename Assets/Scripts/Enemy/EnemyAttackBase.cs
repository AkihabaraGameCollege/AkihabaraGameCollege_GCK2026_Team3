using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    [SerializeField] protected float attackInterval = 1.5f;
    [SerializeField] protected int attackDamage = 10;

    protected float attackTimer;
    protected bool canAttack = false;
    protected GameObject target;

    public void SetTarget(GameObject t)
    {
        target = t;
    }

    protected virtual void Start()
    {
        EnemyMove move = GetComponent<EnemyMove>();
        move.OnReachGoal += StartAttack;
    }

    protected virtual void Update()
    {
        if (!canAttack) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            PerformAttack();
        }
    }

    protected void StartAttack()
    {
        canAttack = true;
    }

    protected abstract void PerformAttack(); // Å© çUåÇì‡óeÇÕéqÇ…îCÇπÇÈ
}