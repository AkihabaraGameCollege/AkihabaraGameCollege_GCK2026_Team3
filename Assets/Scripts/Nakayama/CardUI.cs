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
        private CardData myCardData;
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
        }

        /// <summary>
        /// カードがクリックされたときの処理を管理する関数
        /// </summary>
        private void OnClickCard()
        {
            Debug.Log(myCardData.cardName + " が選択されました！");
            deckManager.AddToDeck(myCardData);// デッキマネージャーの関数を呼び出して、カードをデッキに追加
        }
    }
}