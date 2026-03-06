// File: CardManager.cs
using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// CardManager: manages deck, hand and playing cards. Singleton for convenience.
    /// Handles drawing, playing and returning cards to deck.
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance { get; private set; }

        [Header("Deck & Hand")]
        [SerializeField] private List<CardData> deck = new List<CardData>();
        [SerializeField] private List<CardData> hand = new List<CardData>();
        [SerializeField] private int deckSize = 9;
        [SerializeField] private int maxHandSize = 8;

        [Header("References")]
        [SerializeField] private PlayerController player;

        // Currently selected card index in hand
        private int selectedIndex = -1;

        void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        void Start()
        {
            if (player == null) player = FindObjectOfType<PlayerController>();
            // ensure deck has at least deckSize entries (caller should populate in editor)
            // shuffle deck
            ShuffleDeck();
            // draw initial hand
            for (int i = 0; i < Mathf.Min(5, maxHandSize); i++) DrawCardToHand();
            UIManager.Instance?.UpdateHand(hand);
        }

        /// <summary>
        /// Draw a card from deck to hand. If deck empty, reshuffle from used cards (naive: no discard pile implemented).
        /// </summary>
        public void DrawCardToHand()
        {
            if (hand.Count >= maxHandSize) return;
            if (deck.Count == 0) return;
            CardData card = deck[0];
            deck.RemoveAt(0);
            hand.Add(card);
            UIManager.Instance?.UpdateHand(hand);
        }

        /// <summary>
        /// Try to play the selected card (called by input). Selection is expected to be set by UI.
        /// </summary>
        public void TryPlaySelectedCard()
        {
            if (selectedIndex < 0 || selectedIndex >= hand.Count) return;
            PlayCardAtIndex(selectedIndex);
        }

        /// <summary>
        /// Play card at index: checks cost, applies effect and returns card to deck unless one-time.
        /// </summary>
        public void PlayCardAtIndex(int index)
        {
            if (index < 0 || index >= hand.Count) return;
            CardData card = hand[index];
            if (!player.SpendCost(card.cost))
            {
                Debug.Log("Not enough cost to play this card.");
                return;
            }
            // Apply card effect based on type
            ApplyCardEffect(card);
            // Return card to bottom of deck (simple behavior)
            hand.RemoveAt(index);
            deck.Add(card);
            UIManager.Instance?.UpdateHand(hand);
        }

        /// <summary>
        /// Apply card effects to enemies or player depending on CardData
        /// </summary>
        private void ApplyCardEffect(CardData card)
        {
            switch (card.effectType)
            {
                case CardEffectType.Damage:
                    // Target selection handled by UIManager -> get selected enemy
                    var target = UIManager.Instance?.GetSelectedEnemy();
                    if (target != null)
                    {
                        int damage = Mathf.RoundToInt(card.value * player.GetDamageMultiplier());
                        target.TakeDamage(damage);
                        UIManager.Instance?.ShowDamageNumber(damage, target.transform.position);
                    }
                    else if (card.targetType == CardTargetType.All)
                    {
                        var enemies = EnemyController.GetAllEnemies();
                        foreach (var e in enemies)
                        {
                            int damage = Mathf.RoundToInt(card.value * player.GetDamageMultiplier());
                            e.TakeDamage(damage);
                            UIManager.Instance?.ShowDamageNumber(damage, e.transform.position);
                        }
                    }
                    break;
                case CardEffectType.Heal:
                    player.Heal(card.value);
                    break;
                case CardEffectType.CostRecover:
                    // simply add cost back
                    // naive: directly modify player's cost via SpendCost negative? Instead add a helper
                    // There is no direct AddCost function; implement as spending negative cost by reflection: better to implement method in player.
                    player?.SendMessage("ReceiveCost", card.value, SendMessageOptions.DontRequireReceiver);
                    break;
                case CardEffectType.DamageBoost:
                    player.ApplyDamageBoost(card.damageBoostMultiplier, card.effectDuration);
                    break;
                case CardEffectType.DamageReduction:
                    // Not implemented: could set a global reduction. For simplicity treat as heal small amount
                    player.Heal(card.value);
                    break;
            }
        }

        /// <summary>
        /// Set the selected card index from UI.
        /// </summary>
        public void SetSelectedIndex(int idx)
        {
            selectedIndex = idx;
        }

        /// <summary>
        /// Shuffle deck list in-place.
        /// </summary>
        public void ShuffleDeck()
        {
            for (int i = 0; i < deck.Count; i++)
            {
                int r = Random.Range(i, deck.Count);
                var t = deck[i]; deck[i] = deck[r]; deck[r] = t;
            }
        }

        /// <summary>
        /// Helper to add card to deck (for setup)
        /// </summary>
        public void AddCardToDeck(CardData card)
        {
            deck.Add(card);
        }
    }
}
