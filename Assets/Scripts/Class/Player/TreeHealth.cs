using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForestDraw.Combat;
using ForestDraw;

/// <summary>
/// プレイヤーのHP管理クラス
/// ダメージ処理・回復処理・ダメージ軽減バフを管理する
/// </summary>
public class TreeHealth : MonoBehaviour, IDamageable, IHealable
{
    /// <summary>
    /// ダメージアニメーション用のアニメーター
    /// </summary>
    private Animator damageAnimator;

    /// <summary>
    /// ダメージUI管理クラスを参照する変数
    /// </summary>
    private DamageUI_Manager damageUI_Manager;

    /// <summary>最大HP</summary>
    [SerializeField]
    private int maxHealth = 1000;

    /// <summary>現在HP</summary>
    private int health;

    /// <summary>
    /// ダメージを受けたSEの何番を流すかのインデックスを参照する変数
    /// </summary>
    private int damageSeIndex = 10;

    /// <summary>
    /// ダメージを受けたときに呼ばれるIDを参照する変数
    /// </summary>
    private static readonly int getDamageTrigger = Animator.StringToHash("GetDamage");

    /// <summary>HPバーUI</summary>
    [SerializeField] private Image hpFillImage;

    /// <summary>現在のダメージ軽減率（％）</summary>
    private float damageReduction = 0f;

    /// <summary>バフ前の軽減率保存用</summary>
    private float previousReduction;

    /// <summary>
    /// 被ダメージ時の演出時間を参照する変数
    /// </summary>
    private float damageEffectTime = 0.5f;

    /// <summary>
    /// 被ダメージUIのオブジェクト名を参照する変数
    /// </summary>
    private string damageUI_Name = "DamageUI";

    /// <summary>
    /// 初期化処理
    /// HPを最大値に設定しHPバーを更新
    /// </summary>
    private void Awake()
    {
        // コンポーネントの登録
        damageAnimator = GameObject.Find(damageUI_Name).GetComponent<Animator>();// シーン内から被ダメージUIを探してアニメーターを取得
        damageUI_Manager = GameObject.Find(damageUI_Name).GetComponent<DamageUI_Manager>();// シーン内から被ダメージUIを探してクラスを取得

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

        AudioSetting.Instance.PlaySE(damageSeIndex);// ダメージを受けるSEを再生

        // HP減少
        health = Mathf.Max(health - reducedDamage, 0);

        UpdateHPBar();

        StartCoroutine(DamageEffectCoroutine());// ダメージ演出開始

        if (health > 0)
        {
            return;
        }

        // プレイヤー死亡処理
        StageScene.Instance.GameOver();
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

    /// <summary>
    /// 被ダメージ時の演出を行うコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageEffectCoroutine()
    {
        damageUI_Manager.Show();
        damageAnimator.SetTrigger(getDamageTrigger);// ダメージアニメーションを再生
        yield return new WaitForSeconds(damageEffectTime);// ダメージエフェクト中は待機
        damageUI_Manager.Hide();
    }
}