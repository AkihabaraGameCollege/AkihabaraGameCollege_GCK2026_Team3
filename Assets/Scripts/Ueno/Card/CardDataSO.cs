using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// ScriptableObject defining a card.
    /// Fields:
    /// - CardName, Cost, Damage, EffectType, TargetType
    /// Create new cards via Create->GameCore->Card Data
    /// </summary>
    public enum CardEffectType1 { None, Damage, Heal, CostRecover, DamageBoost, DrawExtra }
    public enum CardTargetType1 { Single, Multiple, All }

    [CreateAssetMenu(fileName = "CardData", menuName = "GameCore/Card Data")]
    public class CardDataSO : ScriptableObject
    {
        [Header("Basic")]
        public string cardName = "New Card";
        [Range(0, 8)] public int cost = 1;

        [Header("Effect")]
        public CardEffectType1
            effectType = CardEffectType1.Damage;
        [Tooltip("Base damage (or heal amount) applied by the card")]
        public int value = 10;
        public CardTargetType1 targetType = CardTargetType1.Single;

        [Header("Support")]
        [Tooltip("Damage boost percent (0.5 = +50%) when effectType is DamageBoost")]
        public float damageBoostPercent = 0f;
        [Tooltip("Extra draw count when effectType is DrawExtra")]
        public int extraDraw = 0;

        [TextArea] public string description;
    }
}
