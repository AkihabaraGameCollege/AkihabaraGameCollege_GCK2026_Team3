using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForestDraw.Combat;

public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{
    [SerializeField]
    private int maxHealth = 1000;
    private int health;

    [SerializeField] private Image hpFillImage;
    private void Start()
    {
        health = maxHealth;   // 最大healthを初期値に保存
        UpdateHPBar();
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        // ダメージを受ける
        health = Mathf.Max(health - amount, 0);
        UpdateHPBar();

        if (health > 0)
        {
            return;
        }
        //Die()
    }
    public void Heal(int amount)
    {
        if (health <= 0) return;

        // 回復を受ける
        health = Mathf.Min(health += amount, maxHealth);
        UpdateHPBar();
    }
    private void UpdateHPBar()
    {
        hpFillImage.fillAmount = (float)health / maxHealth;
    }
}

