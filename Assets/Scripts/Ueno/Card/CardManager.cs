// File: CardManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// Manages deck, hand, playing cards and applying their effects.
    /// Uses ScriptableObject CardData for card definitions.
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        [Header("Deck & Hand")]
        [SerializeField] private List<CardData> deck = new List<CardData>();
        [SerializeField] private int maxHandSize = 8;
        [Header("Deck Rules")]
        [Tooltip("Force deck size to 9 for stage rules (if non-empty) - this is only advisory")]
        [SerializeField] private bool enforceDeckNine = true;

        [Header("References")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private PlayerController playerController;
        // discard pile for used cards (to be reshuffled when deck empties)
        private List<CardData> discard = new List<CardData>();

        public enum DrawFullBehavior { Skip = 0, DiscardDrawn = 1 }
        [Header("Draw Behavior")]
        [SerializeField] private DrawFullBehavior drawFullBehavior = DrawFullBehavior.Skip;

        private List<CardData> hand = new List<CardData>();
        private CardData selectedCard = null;
        // temporary buff state
        private float nextAttackMultiplier = 1f;

        void Start()
        {
            // shuffle deck
            ShuffleDeck();
            // initial draw to fill some hand
            for (int i = 0; i < 4; i++) DrawCardToHand();
            if (enforceDeckNine && deck.Count > 9)
            {
                // trim to 9 and leave rest
                deck.RemoveRange(9, deck.Count - 9);
            }
        }

        public void ShuffleDeck()
        {
            for (int i = 0; i < deck.Count; i++)
            {
                int r = Random.Range(i, deck.Count);
                var tmp = deck[i];
                deck[i] = deck[r];
                deck[r] = tmp;
            }
        }

        /// <summary>
        /// Draw one card from deck to hand if space.
        /// </summary>
        public void DrawCardToHand()
        {
            // if deck empty, try to refill from discard
            if (deck.Count == 0 && discard.Count > 0)
            {
                deck.AddRange(discard);
                discard.Clear();
                ShuffleDeck();
            }

            if (deck.Count == 0)
            {
                uiManager?.ShowFeedback("Deck is empty");
                return;
            }

            var drawn = deck[0];
            deck.RemoveAt(0);

            if (hand.Count >= maxHandSize)
            {
                if (drawFullBehavior == DrawFullBehavior.Skip)
                {
                    uiManager?.ShowFeedback("Hand full - draw skipped");
                    // discard the drawn card
                    discard.Add(drawn);
                }
                else
                {
                    // Discard the drawn card immediately
                    discard.Add(drawn);
                    uiManager?.ShowFeedback("Hand full - drawn card discarded");
                }
            }
            else
            {
                hand.Add(drawn);
                uiManager?.UpdateHandUI(hand, this);
            }
        }

        /// <summary>
        /// Play a card and apply its effect. Card may return to deck or be consumed.
        /// </summary>
        public void PlayCard(CardData card, EnemyController explicitTarget, Vector3 worldPoint)
        {
            if (card == null) return;

            // apply effect depending on type
            switch (card.effectType)
            {
                case CardData.EffectType.Attack:
                    ApplyAttackCard(card, explicitTarget, worldPoint);
                    break;
                case CardData.EffectType.Support:
                    ApplySupportCard(card);
                    break;
            }

            // after use, move to discard unless one-time (oneTimeUse = permanently consumed)
            if (!card.oneTimeUse)
            {
                discard.Add(card);
            }

            // remove from hand
            hand.Remove(card);
            selectedCard = null;
            uiManager?.UpdateHandUI(hand, this);
        }

        private void ApplyAttackCard(CardData card, EnemyController explicitTarget, Vector3 worldPoint)
        {
            switch (card.targetType)
            {
                case CardData.TargetType.Single:
                    if (explicitTarget != null)
                    {
                        int dmg = Mathf.RoundToInt(card.damage * nextAttackMultiplier);
                        // apply target damage reduction through StatusManager on enemy side is handled there
                        explicitTarget.ApplyDamage(dmg, CriticalType.Normal, playerController.transform);
                        // reset one-time next-attack multiplier
                        nextAttackMultiplier = 1f;
                    }
                    else
                    {
                        // find nearest enemy to worldPoint
                        var enemies = GameObject.FindObjectsOfType<EnemyController>();
                        EnemyController closest = null;
                        float best = float.MaxValue;
                        foreach (var en in enemies)
                        {
                            float d = (en.transform.position - worldPoint).sqrMagnitude;
                            if (d < best)
                            {
                                best = d; closest = en;
                            }
                        }
                        if (closest != null)
                        {
                            int dmg = Mathf.RoundToInt(card.damage * nextAttackMultiplier);
                            closest.ApplyDamage(dmg);
                            nextAttackMultiplier = 1f;
                        }
                    }
                    break;
                case CardData.TargetType.All:
                    var all = GameObject.FindObjectsOfType<EnemyController>();
                    foreach (var e in all)
                    {
                        int dmg = Mathf.RoundToInt(card.damage * nextAttackMultiplier);
                        e.ApplyDamage(dmg);
                    }
                    nextAttackMultiplier = 1f;
                    break;
                case CardData.TargetType.Area:
                    Collider[] cols = Physics.OverlapSphere(worldPoint, card.areaRadius);
                    foreach (var c in cols)
                    {
                        var e = c.GetComponentInParent<EnemyController>();
                        if (e != null)
                        {
                            int dmg = Mathf.RoundToInt(card.damage * nextAttackMultiplier);
                            e.ApplyDamage(dmg);
                        }
                    }
                    nextAttackMultiplier = 1f;
                    break;
            }
        }

        private void ApplySupportCard(CardData card)
        {
            // simple support effects by type name / enum
            if (card.supportSubtype == CardData.SupportType.Heal)
            {
                playerController?.Heal(card.supportValue);
            }
            else if (card.supportSubtype == CardData.SupportType.Cost)
            {
                playerController?.ChangeCost(card.supportValue);
            }
            else if (card.supportSubtype == CardData.SupportType.Buff)
            {
                // apply next-attack multiplier
                nextAttackMultiplier = card.attackMultiplier > 0f ? card.attackMultiplier : 1.5f;
                if (card.supportDuration > 0f)
                {
                    StartCoroutine(ClearMultiplierAfter(card.supportDuration));
                }
                uiManager?.ShowFeedback("Damage buff applied");
            }
            else if (card.supportSubtype == CardData.SupportType.DamageReduction)
            {
                // apply reduction on player StatusManager
                var playerStatus = playerController?.GetComponent<StatusManager>();
                if (playerStatus != null)
                {
                    playerStatus.ApplyDamageReduction(Mathf.Clamp01(card.supportValue / 100f), card.supportDuration);
                    uiManager?.ShowFeedback("Damage reduction applied to player");
                }
            }
            else if (card.supportSubtype == CardData.SupportType.Duplicate)
            {
                // duplicate highest cost card in deck into hand if space
                CardData highest = null;
                foreach (var c in deck)
                {
                    if (highest == null || c.cost > highest.cost) highest = c;
                }
                if (highest != null && hand.Count < maxHandSize)
                {
                    hand.Add(highest);
                    uiManager?.ShowFeedback($"Duplicated {highest.cardName} to hand");
                }
            }
        }

        private System.Collections.IEnumerator ClearMultiplierAfter(float t)
        {
            yield return new WaitForSeconds(t);
            nextAttackMultiplier = 1f;
        }


        public List<CardData> GetHand() => hand;

        public bool HasSelectedCard() => selectedCard != null;
        public CardData GetSelectedCard() => selectedCard;

        public void SelectCard(CardData card)
        {
            selectedCard = card;
            uiManager?.HighlightSelectedCard(card);
        }

        public void DeselectCard()
        {
            selectedCard = null;
            uiManager?.HighlightSelectedCard(null);
        }
    }

}