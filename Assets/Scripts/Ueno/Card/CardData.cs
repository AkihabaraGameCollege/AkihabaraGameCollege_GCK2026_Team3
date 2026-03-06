// File: CardData.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// Card data ScriptableObject describing a card.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard", menuName = "Game/CardData")]
    public class CardData : ScriptableObject
    {
        public enum EffectType { Attack, Support }
        public enum TargetType { Single, Area, All }
        public enum SupportType { Heal, Cost, CostRegen, Buff, DamageReduction, Duplicate }

        [Header("Basic")]
        public string cardName = "Card";
        public Sprite artwork;
        public int cost = 1;
        public bool oneTimeUse = false; // if true, consumed permanently

        [Header("Attack")]
        public EffectType effectType = EffectType.Attack;
        public TargetType targetType = TargetType.Single;
        public int damage = 10;
        public float areaRadius = 3f;

        [Header("Support")]
        public SupportType supportSubtype = SupportType.Heal;
        public int supportValue = 10; // heal amount or cost amount
        // support specifics
        [Tooltip("Duration in seconds for temporary buffs (0 = instant)")]
        public float supportDuration = 0f;
        [Tooltip("Multiplier applied to next attack when using Buff support (e.g. 1.5)")]
        public float attackMultiplier = 1f;

        [TextArea]
        public string description;
    }

}
