// File: CardSlotUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// Visual slot for a card in hand. Handles selection/click to set CardManager selection.
    /// </summary>
    public class CardSlotUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button button;

        private int index;
        private CardData data;

        public void Setup(CardData card, int idx)
        {
            data = card;
            index = idx;
            if (nameText != null) nameText.text = card.cardName;
            if (costText != null) costText.text = card.cost.ToString();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            CardManager.Instance?.SetSelectedIndex(index);
        }
    }
}
