// File: CardData.cs
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// CardData ScriptableObject defines card properties and behavior categories.
    /// Create new cards via Create->ActionCard->Card Data
    /// </summary>
    public enum CardEffectType { None, Damage, Heal, CostRecover, DamageBoost, DamageReduction }
    public enum CardTargetType { Single, Multiple, All }

    [CreateAssetMenu(fileName = "CardData", menuName = "ActionCard/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Basic")]
        public string cardName = "New Card";
        [Range(0, 8)] public int cost = 1;

        [Header("Effect")]
        public CardEffectType effectType = CardEffectType.Damage;
        [Tooltip("Base damage (or heal amount) applied by the card")]
        public int value = 10;
        public CardTargetType targetType = CardTargetType.Single;

        [Header("Support")]
        [Tooltip("Damage boost multiplier when effectType is DamageBoost")]
        public float damageBoostMultiplier = 1.5f;
        [Tooltip("Duration in seconds for temporary effects")]
        public float effectDuration = 5f;

        [TextArea]
        public string description;
    }
}
