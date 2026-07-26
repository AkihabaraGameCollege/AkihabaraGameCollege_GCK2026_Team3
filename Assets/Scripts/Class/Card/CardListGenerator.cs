using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// カードリストを生成するクラス
    /// </summary>
    public class CardListGenerator : MonoBehaviour
    {
        /// <summary>
        /// カードのPrefabを入れる変数
        /// </summary>
        [SerializeField]
        private GameObject cardPrefab;

        /// <summary>
        /// スクロールビューのContentのTransformを入れる変数
        /// </summary>
        [SerializeField]
        private Transform contentTransform;

        /// <summary>
        /// デッキマネージャーの変数
        /// </summary>
        [SerializeField]
        private DeckManager deckManager;

        /// <summary>
        /// カードデータのリスト変数
        /// </summary>
        public List<CardData> allOwnedCards;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            GenerateList();
        }

        /// <summary>
        /// カードデータのリストを元に、カードのUIを生成する関数
        /// </summary>
        private void GenerateList()
        {
            // カードデータのリストをループ
            foreach (CardData cardData in allOwnedCards)
            {
                GameObject obj = Instantiate(cardPrefab, contentTransform);// カードのPrefabをContentの子オブジェクトとして生成

                // 生成するためのカードの設定
                CardUI_Manager cardUI = obj.GetComponent<CardUI_Manager>();// 生成したカードのUIスクリプトを取得
                cardUI.Setup(cardData, deckManager);// カードUIの初期設定を行う
            }
        }

        /// <summary>
        /// カードデータを元に、カードのUIを生成する関数（外部から呼び出すための関数）
        /// </summary>
        /// <param name="cardData"></param>
        public void AddList(CardData cardData)
        {
            GameObject obj = Instantiate(cardPrefab, contentTransform);// カードのPrefabをContentの子オブジェクトとして生成

            // 生成するためのカードの設定
            CardUI_Manager cardUI = obj.GetComponent<CardUI_Manager>();// 生成したカードのUIスクリプトを取得
            cardUI.Setup(cardData, deckManager);// カードUIの初期設定を行う
        }

        // 特定のカードUIのみを削除する関数
        public void RemoveList(CardData cardData)
        {
            // Contentの子オブジェクトをループ
            foreach (Transform child in contentTransform)
            {
                CardUI_Manager cardUI = child.GetComponent<CardUI_Manager>();// 子オブジェクトのCardUIスクリプトを取得
                // もしカードデータが一致する場合
                if (cardUI.MyCardData == cardData)
                {
                    Destroy(child.gameObject);// 子オブジェクトを削除
                    break;// ループを抜ける
                }
            }
        }
    }
}