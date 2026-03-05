using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// カードのUIを管理するクラス
    /// </summary>
    public class CardUI : MonoBehaviour
    {
        /// <summary>
        /// カードの見た目を管理するための変数
        /// </summary>
        [SerializeField]
        private Image cardImage;

        /// <summary>
        /// カードがクリックされたときの処理を管理するための変数
        /// </summary>
        [SerializeField]
        private Button clickButton;

        /// <summary>
        /// カードのデータを管理するための変数
        /// </summary>
        public CardData myCardData;
        /// <summary>
        /// デッキの管理クラスを参照するための変数
        /// </summary>
        private DeckManager deckManager;

        /// <summary>
        /// 生成されたカードUIを初期化するための関数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        public void Setup(CardData data, DeckManager manager)
        {
            myCardData = data;// カードのデータを保存
            deckManager = manager;// デッキマネージャーを保存

            // もしカードの画像が存在する場合
            if (cardImage != null && data.cardImage != null)
            {
                cardImage.sprite = data.cardImage;// カードの画像を設定
            }

            clickButton.onClick.AddListener(OnClickCard);// カードがクリックされたときの処理を登録

            deckManager.OnDeckChanged += UpdateVisibility;// デッキが変更されたときにカードの表示を更新する関数を登録

            UpdateVisibility();// 最初の表示更新
        }

        /// <summary>
        /// カードの表示を更新する関数
        /// </summary>
        private void OnDestroy()
        {
            if (deckManager != null)
            {
                deckManager.OnDeckChanged -= UpdateVisibility;// 登録を解除する（メモリリーク防止）
            }
        }

        /// <summary>
        /// カードがクリックされたときの処理を管理する関数
        /// </summary>
        private void OnClickCard()
        {
            deckManager.AddToDeck(myCardData);// デッキマネージャーの関数を呼び出して、カードをデッキに追加
        }

        /// <summary>
        /// カードの表示を更新する関数
        /// </summary>
        private void UpdateVisibility()
        {
            // もしカードのデータやデッキマネージャーが存在しない場合
            if (myCardData == null || deckManager == null)
            {
                return;
            }

            bool isAlreadyInDeck = deckManager.currentDeck.Contains(myCardData);// カードがすでにデッキに含まれているかどうかをチェック

            gameObject.SetActive(!isAlreadyInDeck);// もしカードがデッキに含まれている場合は非表示にする
        }
    }
}