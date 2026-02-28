using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// デッキのデータを管理するクラス
    /// </summary>
    public class DeckData : ScriptableObject
    {
        /// <summary>
        /// デッキのカードのリスト
        /// </summary>
        public List<CardData> deckList = new List<CardData>();

        /// <summary>
        /// カードをデッキに追加する関数
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool AddCard(CardData card)
        {
            // もしデッキのカードの数が9以上なら
            if (deckList.Count >= 9)
            {
                Debug.Log("デッキがいっぱいです！");
                return false;// 追加できない
            }

            deckList.Add(card);// カードをデッキに追加する
            return true;// 追加できる
        }
    }
}