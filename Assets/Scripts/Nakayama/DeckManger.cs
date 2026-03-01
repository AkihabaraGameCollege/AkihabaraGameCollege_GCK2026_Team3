using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// デッキの管理クラス
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        /// <summary>
        /// デッキに入れるカードのリスト変数
        /// </summary>
        public List<CardData> currentDeck = new List<CardData>();

        /// <summary>
        /// デッキが変更されたときに呼び出されるイベントの変数
        /// </summary>
        public Action OnDeckChanged;

        /// <summary>
        /// デッキの最大枚数を定数で定義した変数
        /// </summary>
        private const int MAX_DECK_SIZE = 9;

        /// <summary>
        /// デッキにカードを追加する関数
        /// </summary>
        /// <param name="card"></param>
        public void AddToDeck(CardData card)
        {
            // もしデッキに入れるカードがすでにデッキに存在している場合
            if (currentDeck.Count < MAX_DECK_SIZE)
            {
                currentDeck.Add(card);
                OnDeckChanged?.Invoke();// UIを更新させるためにイベントを飛ばす
            }
            else
            {
                Debug.Log("デッキが満杯です！");
            }
        }

        /// <summary>
        /// デッキからカードを削除する関数
        /// </summary>
        /// <param name="card"></param>
        public void RemoveFromDeck(CardData card)
        {
            // もしデッキに削除するカードが存在している場合
            if (currentDeck.Contains(card))
            {
                currentDeck.Remove(card);// デッキリストから削除
                OnDeckChanged?.Invoke();// UIを更新させるためにイベントを飛ばす
            }
        }
    }
}