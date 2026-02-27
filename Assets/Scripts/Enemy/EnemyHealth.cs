using CardDefenseGame;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int health = 1000;
    private int maxHealth;

    [SerializeField] private float damageInterval = 1;

    [SerializeField] private Image hpFillImage;

    // 無敵状態の判別
    private bool isTakingDamage = true;
    private void Start()
    {
        maxHealth = health;   // 初期値を最大HPに保存
        UpdateHPBar();
    }

    public void TakeDamage(int amount)
    {
        if (!isTakingDamage || health <= 0) return; 

        // ダメージを受ける
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            UpdateHPBar();
            Destroy(gameObject);
        }
        else
        {
            UpdateHPBar();
            isTakingDamage = false;
            StartCoroutine(DamageInterval());
        }
    }
    private IEnumerator DamageInterval()
    {
        yield return new WaitForSeconds(damageInterval);
        isTakingDamage = true;
    }
    private void UpdateHPBar()
    {
        hpFillImage.fillAmount = (float)health / maxHealth;
    }
}
