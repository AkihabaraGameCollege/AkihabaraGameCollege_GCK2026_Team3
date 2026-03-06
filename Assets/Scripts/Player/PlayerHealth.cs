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
    private float damageReduction = 0f;
    private float previousReduction;
    private void Start()
    {
        health = maxHealth;   // 最大healthを初期値に保存
        UpdateHPBar();
        previousReduction = damageReduction;
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        float reductionFactor = damageReduction / 100f;
        int reducedDamage = Mathf.RoundToInt(amount * (1f - reductionFactor));

        health = Mathf.Max(health - reducedDamage, 0);
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
    public void ApplyDamageReduction(float reductionPercent, float duration)
    {
        StartCoroutine(DamageReductionCoroutine(reductionPercent, duration));
    }

    private IEnumerator DamageReductionCoroutine(float reductionPercent, float duration)
    {
        damageReduction = Mathf.Clamp(reductionPercent, 0f, 100f); // 0〜100に制限

        yield return new WaitForSeconds(duration);

        damageReduction = previousReduction;
    }
}

