using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForestDraw.Combat;

/// <summary>
/// プレイヤーのHP管理クラス
/// ダメージ処理・回復処理・ダメージ軽減バフを管理する
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{
    /// <summary>最大HP</summary>
    [SerializeField]
    private int maxHealth = 1000;

    /// <summary>現在HP</summary>
    private int health;

    /// <summary>HPバーUI</summary>
    [SerializeField] private Image hpFillImage;

    /// <summary>現在のダメージ軽減率（％）</summary>
    private float damageReduction = 0f;

    /// <summary>バフ前の軽減率保存用</summary>
    private float previousReduction;

    /// <summary>
    /// 初期化処理
    /// HPを最大値に設定しHPバーを更新
    /// </summary>
    private void Awake()
    {
        health = maxHealth;
        UpdateHPBar();
        previousReduction = damageReduction;
    }

    /// <summary>
    /// ダメージを受ける処理
    /// ダメージ軽減率を考慮してHPを減らす
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        // ダメージ軽減率を適用
        float reductionFactor = damageReduction / 100f;
        int reducedDamage = Mathf.RoundToInt(amount * (1f - reductionFactor));

        // HP減少
        health = Mathf.Max(health - reducedDamage, 0);

        UpdateHPBar();

        if (health > 0)
        {
            return;
        }

        // プレイヤー死亡処理
        // Die();
    }

    /// <summary>
    /// HPを回復する
    /// </summary>
    public void Heal(int amount)
    {
        if (health <= 0) return;

        health = Mathf.Min(health + amount, maxHealth);
        UpdateHPBar();
    }

    /// <summary>
    /// HPバーUIを更新する
    /// </summary>
    private void UpdateHPBar()
    {
        hpFillImage.fillAmount = (float)health / maxHealth;
    }

    /// <summary>
    /// 一定時間ダメージ軽減を適用する
    /// </summary>
    public void ApplyDamageReduction(float reductionPercent, float duration)
    {
        StartCoroutine(DamageReductionCoroutine(reductionPercent, duration));
    }

    /// <summary>
    /// ダメージ軽減バフ処理
    /// 指定時間後に元の軽減率に戻す
    /// </summary>
    private IEnumerator DamageReductionCoroutine(float reductionPercent, float duration)
    {
        // 軽減率を0〜100%の範囲に制限
        damageReduction = Mathf.Clamp(reductionPercent, 0f, 100f);

        yield return new WaitForSeconds(duration);

        // 元の軽減率に戻す
        damageReduction = previousReduction;
    }
}