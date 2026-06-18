using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// デッキスロットUIを機能を担うクラス
    /// </summary>
    public class DeckSlotUI : MonoBehaviour
    {
        /// <summary>
        /// スロットに表示するカードの画像を指定する変数
        /// </summary>
        [SerializeField]
        private Image cardImage = null;

        /// <summary>
        /// ボタンコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Button slotButton = null;

        /// <summary>
        /// カードデータがないときに表示するビジュアルの変数
        /// </summary>
        private CardData currentCard;

        /// <summary>
        /// デッキマネージャーの変数
        /// </summary>
        private DeckManager deckManager;

        /// <summary>
        /// デッキマネージャーをセットアップする関数
        /// </summary>
        /// <param name="manager"></param>
        public void Setup(DeckManager manager)
        {
            deckManager = manager;// デッキマネージャーをセット

            // もしスロットにボタンコンポーネントがアタッチされている場合
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnClickRemove);// ボタンがクリックされたときにカードをデッキから外す関数を登録
            }
        }

        /// <summary>
        /// カードデータをスロットにセットする関数
        /// </summary>
        /// <param name="cardData"></param>
        public void SetCard(CardData cardData)
        {
            currentCard = cardData;// カードデータをセット
            cardImage.sprite = cardData.cardImage;
            cardImage.enabled = true;
        }

        /// <summary>
        /// スロットをクリアする関数
        /// </summary>
        public void ClearSlot()
        {
            currentCard = null;// カードデータをクリア
            cardImage.enabled = false;
        }

        /// <summary>
        /// デッキからカードを削除する関数
        /// </summary>
        private void OnClickRemove()
        {
            // もしスロットにカードデータが存在していて、デッキマネージャーもセットされている場合
            if (currentCard != null && deckManager != null)
            {
                deckManager.RemoveFromDeck(currentCard);// デッキマネージャーからカードを削除
            }
        }
    }
}