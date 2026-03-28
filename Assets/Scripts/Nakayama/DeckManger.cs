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
        /// カードマスターデータのリスト変数
        /// </summary>
        public List<CardData> allCardMasterList;

        /// <summary>
        /// デッキが変更されたときに呼び出されるイベントの変数
        /// </summary>
        public Action OnDeckChanged;

        /// <summary>
        /// デッキの最大枚数を定数で定義した変数
        /// </summary>
        private const int MAX_DECK_SIZE = 9;

        /// <summary>
        /// カードリストを生成するクラス（CardListGenerator）を参照するための変数
        /// </summary>
        private CardListGenerator cardListGenerator;

        /// <summary>
        /// プレイヤーのデッキデータを保存するためのキーを定数で定義した変数
        /// </summary>
        private const string SAVE_KEY = "UserDeckSaveData";

        /// <summary>
        /// デッキが満タンかどうかを返すプロパティを参照する変数
        /// </summary>
        public bool isDeckFull => currentDeck.Count == MAX_DECK_SIZE;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            LoadDeck();// デッキデータをロードする関数を呼び出す
        }

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
                SaveDeck();// デッキデータを保存する関数を呼び出す
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
                SaveDeck();// デッキデータを保存する関数を呼び出す
            }
        }

        /// <summary>
        /// セーブデータに現在のデッキを保存する関数
        /// </summary>
        public void SaveDeck()
        {
            DeckSaveData saveData = new DeckSaveData();

            // もしデッキにカードが存在する場合のループ
            foreach (CardData card in currentDeck)
            {
                saveData.savedCardIds.Add(card.cardId);// デッキに入っているカードのIDをセーブデータのリストに追加する
            }

            string jsonStr = JsonUtility.ToJson(saveData);// デッキセーブデータのクラスをJSON文字列に変換する

            // PlayerPrefsに文字列として保存
            PlayerPrefs.SetString(SAVE_KEY, jsonStr);// デッキデータを保存するためのキーとJSON文字列をPlayerPrefsに保存
            PlayerPrefs.Save();// 確実に書き込む
        }

        /// <summary>
        /// ロードしたデッキデータを現在のデッキに反映させる関数
        /// </summary>
        public void LoadDeck()
        {
            // もしPlayerPrefsに保存されたデータが存在する場合
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string jsonStr = PlayerPrefs.GetString(SAVE_KEY);// 保存されたJSON文字列を取得

                DeckSaveData loadedData = JsonUtility.FromJson<DeckSaveData>(jsonStr);// JSON文字列をデッキセーブデータのクラスに変換

                currentDeck.Clear();// 現在のデッキを一旦リセット

                // もし保存されたカードIDが存在する場合のループ
                foreach (string id in loadedData.savedCardIds)
                {
                    CardData foundCard = allCardMasterList.Find(card => card.cardId == id);// カードマスターデータから保存されたIDと一致するカードを探す

                    // もし見つかったカードが存在する場合
                    if (foundCard != null)
                    {
                        currentDeck.Add(foundCard);// デッキにカードを追加する
                    }
                }
            }

            OnDeckChanged?.Invoke();// デッキが変更されたときにUIを更新するためのイベントを呼び出す
        }
    }
}