using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHp = 1000;
    public int hp = 1000;

    void Start()
    {
        hp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
