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
                CardUI cardUI = obj.GetComponent<CardUI>();// 生成したカードのUIスクリプトを取得
                cardUI.Setup(cardData, deckManager);// カードUIの初期設定を行う
            }
        }
    }
}