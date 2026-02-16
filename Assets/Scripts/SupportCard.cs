using System;
using UnityEngine;

// 支援カード（Inspectorで調整可能）
// PlayerController を渡して各支援効果を実行するユーティリティを提供します。
// 現在「コスト支払い」はプレイヤーのライフを消費する形で実装しています。
public class SupportCard : MonoBehaviour
{
    [Header("General cost (ライフで支払うための量)")]
    [SerializeField] int defaultLifeCost = 1;

    [Header("Boost Next Attack")]
    [SerializeField] float boostDamageMultiplier = 1.5f;
    [SerializeField] int boostCost = 1;

    [Header("Draw From Deck")]
    [SerializeField] int drawCost = 1;

    [Header("Duplicate Card into Hand")]
    [SerializeField] int duplicateCost = 1;

    [Header("Cost Recovery Buff")]
    [SerializeField] float costRecoveryMultiplier = 1.5f;
    [SerializeField] float costRecoveryDuration = 5.0f;
    [SerializeField] int costRecoveryCost = 1;

    [Header("Heal / Shield")]
    [SerializeField] int healAmount = 2;
    [SerializeField] int healCost = 1;
    [SerializeField] int shieldAmount = 2;
    [SerializeField] int shieldCost = 1;

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

    // 2) 山札から1枚引く
    public PlayerController.Card DrawFromDeck(PlayerController owner, int lifeCost = -1)
    {
        if (owner == null) return null;
        if (lifeCost < 0) lifeCost = drawCost;
        if (!owner.ConsumeLife(lifeCost)) return null;

        var card = owner.DrawCard();
        Debug.Log($"SupportCard: デッキからドロー (cost:{lifeCost}) -> {(card != null ? card.displayName : "なし")}");
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