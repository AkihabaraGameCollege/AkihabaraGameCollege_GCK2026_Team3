using CardDefenseGame;
using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int health = 1000;
    [SerializeField]
    private float damageInterval = 1;

    // –³“Gó‘Ô‚Ì”»•Ê
    private bool isTakingDamage = true;
    public void TakeDamage(int amount)
    {
        if (!isTakingDamage || health <= 0) return;

        // ƒ_ƒ[ƒW‚ğó‚¯‚é
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            isTakingDamage = false;
            StartCoroutine(DamageInterval());
        }
    }
    private IEnumerator DamageInterval()
    {
        yield return new WaitForSeconds(damageInterval);
        isTakingDamage = true;
    }
}
