using UnityEngine;
using System;

namespace ForestDraw
{
    /// <summary>
    /// デッキを管理するクラス
    /// </summary>
    public class DeckManger : MonoBehaviour
    {
        /// <summary>
        /// デッキのデータを管理する変数
        /// </summary>
        [SerializeField]
        private DeckData deckData;

        /// <summary>
        /// デッキの中身が変わったときに呼ぶイベント
        /// </summary>
        public Action OnDeckChanged; // デッキの中身が変わった時に呼ぶ


        /// <summary>
        /// デッキにカードを追加する関数
        /// </summary>
        /// <param name="card"></param>
        private void AddToDeck(CardData card)
        {
            if (deckData.deckList.Count < 9)
            {
                deckData.deckList.Add(card);
                Debug.Log("カードをデッキに追加しました！");
                OnDeckChanged?.Invoke();// デッキの中身が変わったときにイベントを呼び出す
            }
            else
            {
                Debug.Log("デッキがいっぱいです！");
            }
        }

        /// <summary>
        /// デッキからカードを削除する関数
        /// </summary>
        private void RemoveFromDeck(CardData card)
        {
            // リストにカードが存在するかチェック
            if (deckData.deckList.Contains(card))
            {
                deckData.deckList.Remove(card);
                Debug.Log("カードをデッキから削除しました！");
                OnDeckChanged?.Invoke();// デッキの中身が変わったときにイベントを呼び出す
            }
            else
            {
                Debug.Log("デッキにそのカードは存在しません！");
            }
        }
    }
}