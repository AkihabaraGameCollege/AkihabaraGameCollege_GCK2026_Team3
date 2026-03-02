using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public PlayerManager player;
    public Camera mainCamera;

    public void UseCard(CardData data)
    {
        if (!player.UseCost(data.cost)) return;

        switch (data.effectType)
        {
            case CardEffectType.SingleAttack:
                SingleAttack(data);
                break;

            case CardEffectType.ScreenAttack:
                ScreenAttack(data);
                break;

            case CardEffectType.DamageBoost:
                player.ApplyDamageBoost(1.5f);
                break;

            case CardEffectType.Heal:
                player.Heal(data.healAmount);
                break;

            case CardEffectType.AddCost:
                player.AddCost(data.costAmount);
                break;

            case CardEffectType.DamageReduction:
                player.ApplyDamageReduction(0.5f, data.duration);
                break;

            case CardEffectType.CostRegenBoost:
                player.ApplyCostRegenBoost(2, data.duration);
                break;
        }
    }

    void SingleAttack(CardData data)
    {
        if (EnemyManager.Instance.enemies.Count == 0) return;

        Enemy target = EnemyManager.Instance.enemies[0];
        int damage = player.CalculateDamage(data.minDamage, data.maxDamage);
        target.TakeDamage(damage);
    }

    void ScreenAttack(CardData data)
    {
        List<Enemy> targets = EnemyManager.Instance.GetEnemiesInScreen(mainCamera);

        foreach (var enemy in targets)
        {
            int damage = player.CalculateDamage(data.minDamage, data.maxDamage);
            enemy.TakeDamage(damage);
        }
    }
}