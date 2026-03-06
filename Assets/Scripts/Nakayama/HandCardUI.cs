using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// デッキに登録したカードを表示するUIクラス
    /// </summary>
    public class HandCardUI : MonoBehaviour
    {
        /// <summary>
        /// カードのイラストを表示する画像の変数
        /// </summary>
        [SerializeField]
        private Image cardImage;

        /// <summary>
        /// ボタンコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Button clickButton;

        /// <summary>
        /// カードのデータを保持する変数
        /// </summary>
        private CardData myCardData;

        /// <summary>
        /// セットアップの関数
        /// </summary>
        /// <param name="data"></param>
        public void Setup(CardData data)
        {
            myCardData = data;

            // もしカードのイラストが存在する場合
            if (cardImage != null && data.cardImage != null)
            {
                cardImage.sprite = data.cardImage;// カードのイラストをUIにセット
            }

            // クリックした時の処理を登録
            clickButton.onClick.RemoveAllListeners();// 念のためリセット
            clickButton.onClick.AddListener(OnUseCard);// カードを使用する関数を登録
        }

        /// <summary>
        /// 使うカードをクリックしたときの処理
        /// </summary>
        private void OnUseCard()
        {
            Debug.Log(myCardData.cardName + " を使用しました！");
        }
    }
}