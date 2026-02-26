using System;
using UnityEngine;

// 支援カード（Inspectorで調整可能）
// PlayerController を渡して各支援効果を実行するユーティリティを提供します。
// 現在「コスト支払い」はプレイヤーのライフを消費する形で実装しています。
public class SupportCard : MonoBehaviour
{
    [Header("基本コスト（ライフで支払う量)")]
    [SerializeField]
    [Tooltip("支援カード使用時のデフォルトのライフ消費量")]
    int defaultLifeCost = 1;

    [Header("次の攻撃を強化")]
    [SerializeField]
    [Tooltip("次の攻撃に掛けるダメージ倍率")]
    float boostDamageMultiplier = 1.5f;
    [SerializeField]
    [Tooltip("次の攻撃強化を使用する際のライフコスト")]
    int boostCost = 1;

    [Header("デッキからドロー")]
    [SerializeField]
    [Tooltip("デッキから1枚引く際のライフコスト")]
    int drawCost = 1;

    [Header("カード複製（手札に追加）")]
    [SerializeField]
    [Tooltip("指定カードを複製して手札に加える際のライフコスト")]
    int duplicateCost = 1;

    [Header("コスト回復バフ")]
    [SerializeField]
    [Tooltip("コスト回復力に掛ける倍率（例: 1.5 = 50%増加）")]
    float costRecoveryMultiplier = 1.5f;
    [SerializeField]
    [Tooltip("コスト回復バフの持続時間（秒）")]
    float costRecoveryDuration = 5.0f;
    [SerializeField]
    [Tooltip("コスト回復バフ使用時のライフコスト")]
    int costRecoveryCost = 1;

    [Header("回復 / シールド")]
    [SerializeField]
    [Tooltip("ライフ回復量")]
    int healAmount = 2;
    [SerializeField]
    [Tooltip("ライフ回復を使用する際のライフコスト")]
    int healCost = 1;
    [SerializeField]
    [Tooltip("付与するシールド量")]
    int shieldAmount = 2;
    [SerializeField]
    [Tooltip("シールド付与を使用する際のライフコスト")]
    int shieldCost = 1;

    // 1) 次の攻撃のダメージを増やす（支払いに失敗したら false）
    public bool BoostNextAttack(PlayerController owner, int lifeCost = -1, float multiplier = -1f)
    {
        if (owner == null) return false;
        if (lifeCost < 0) lifeCost = boostCost;
        if (multiplier <= 0f) multiplier = boostDamageMultiplier;

        if (!owner.ConsumeLife(lifeCost)) return false;

        owner.ApplyNextAttackMultiplier(multiplier);
        Debug.Log($"SupportCard: 次の攻撃倍率 x{multiplier} を付与 (cost:{lifeCost})");
        return true;
    }

    // 1b) 次の攻撃の基礎ダメージを上げる（支払いに失敗したら false）
    public bool BoostNextAttackBase(PlayerController owner, int lifeCost, int additionalBase)
    {
        if (owner == null) return false;
        if (lifeCost < 0) return false;
        if (additionalBase <= 0) return false;

        if (!owner.ConsumeLife(lifeCost)) return false;

        // 取得して既存上書きを組み合わせる: 既存上書きがあれば加算、なければ設定
        int existing = owner.GetAndConsumeNextAttackBaseOverride();
        int newBase = (existing >= 0) ? existing + additionalBase : additionalBase;
        owner.ApplyNextAttackBaseOverride(newBase);

        Debug.Log($"SupportCard: 次の攻撃基礎ダメージ +{additionalBase} を付与 (cost:{lifeCost})");
        return true;
    }

    // 2) 山札から1枚引く
    public PlayerController.Card DrawFromDeck(PlayerController owner, int lifeCost = -1)
    {
        if (owner == null) return null;
        if (lifeCost < 0) lifeCost = drawCost;
        if (!owner.ConsumeLife(lifeCost)) return null;

        var card = owner.DrawCard();
        Debug.Log($"SupportCard: デッキからドロー (cost:{lifeCost}) -> {(card != null ? card.displayName : "なし")}" );
        return card;
    }

    // 3) 指定したカードデータを複製して手札に加える（手札上限を超えると失敗）
    public bool DuplicateCardIntoHand(PlayerController owner, PlayerController.Card cardToDuplicate, int lifeCost = -1)
    {
        if (owner == null || cardToDuplicate == null) return false;
        if (lifeCost < 0) lifeCost = duplicateCost;
        if (!owner.ConsumeLife(lifeCost)) return false;

        var copy = new PlayerController.Card
        {
            id = $"{cardToDuplicate.id}_dup_{Guid.NewGuid():N}",
            displayName = cardToDuplicate.displayName,
            type = cardToDuplicate.type,
            cost = cardToDuplicate.cost,
            power = cardToDuplicate.power
        };

        bool added = owner.AddCardToHand(copy);
        Debug.Log($"SupportCard: カード複製 {(added ? "成功" : "失敗(手札満杯)")} (cost:{lifeCost})");
        return added;
    }

    // 4) コスト回復力を一定時間上げる
    public bool ApplyCostRecoveryBuff(PlayerController owner, int lifeCost = -1, float multiplier = -1f, float duration = -1f)
    {
        if (owner == null) return false;
        if (lifeCost < 0) lifeCost = costRecoveryCost;
        if (multiplier <= 0f) multiplier = costRecoveryMultiplier;
        if (duration <= 0f) duration = costRecoveryDuration;

        if (!owner.ConsumeLife(lifeCost)) return false;

        owner.ApplyCostRecoveryBuff(multiplier, duration);
        Debug.Log($"SupportCard: コスト回復力 x{multiplier} を {duration}s 付与 (cost:{lifeCost})");
        return true;
    }

    // 5) ライフを回復する
    public bool HealPlayerLife(PlayerController owner, int lifeCost = -1, int amount = -1)
    {
        if (owner == null) return false;
        if (lifeCost < 0) lifeCost = healCost;
        if (amount < 0) amount = healAmount;

        if (!owner.ConsumeLife(lifeCost)) return false;

        owner.HealLife(amount);
        Debug.Log($"SupportCard: ライフ回復 {amount} (cost:{lifeCost})");
        return true;
    }

    // 6) シールドを追加する
    public bool AddShield(PlayerController owner, int lifeCost = -1, int amount = -1)
    {
        if (owner == null) return false;
        if (lifeCost < 0) lifeCost = shieldCost;
        if (amount < 0) amount = shieldAmount;

        if (!owner.ConsumeLife(lifeCost)) return false;

        owner.AddShield(amount);
        Debug.Log($"SupportCard: シールド +{amount} (cost:{lifeCost})");
        return true;
    }
}