using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// バトルシーンで、編成画面で作ったデッキからカードを引いて手札に表示するクラス
    /// </summary>
    public class BattleCardManager : MonoBehaviour
    {
        /// <summary>
        /// カードのPrefabと、生成する親オブジェクトの変数
        /// </summary>
        [SerializeField]
        private GameObject handCardPrefab;

        /// <summary>
        /// 空間上でカードを並べる場所の変数
        /// </summary>
        [SerializeField]
        private Transform handArea;

        /// <summary>
        /// リストに全カードのマスターデータを入れておく変数
        /// </summary>
        public List<CardData> allCardMasterList;
        /// <summary>
        /// リストにロードしたデッキの中身を入れておく変数
        /// </summary>
        private List<CardData> playerDeck = new List<CardData>();

        /// <summary>
        /// セーブ機能で使うキーの定数の変数
        /// </summary>
        private const string SAVE_KEY = "UserDeckSaveData";

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            LoadDeckData();
            DrawRandomCards(4);// ランダムに4枚引いて並べる
        }

        /// <summary>
        /// ロードしてデッキの中身をplayerDeckリストに入れる関数
        /// </summary>
        private void LoadDeckData()
        {
            // もしセーブデータがある場合
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string jsonStr = PlayerPrefs.GetString(SAVE_KEY);// セーブデータをJSONからクラスに変換
                DeckSaveData loadedData = JsonUtility.FromJson<DeckSaveData>(jsonStr);// ロードしたカードIDを元に、マスターデータからカードを探してデッキに追加

                // もしセーブデータのカードIDがマスターデータに存在する場合
                foreach (string id in loadedData.savedCardIds)
                {
                    CardData foundCard = allCardMasterList.Find(card => card.cardId == id);// マスターデータからIDが一致するカードを探す

                    // もし見つかった場合
                    if (foundCard != null)
                    {
                        playerDeck.Add(foundCard);// デッキに追加
                    }
                }
            }
        }

        /// <summary>
        /// ランダムにカードを引いて手札エリアに表示する関数
        /// </summary>
        /// <param name="drawCount"></param>
        private void DrawRandomCards(int drawCount)
        {
            // もしデッキが空の場合
            if (playerDeck.Count == 0)
            {
                return;
            }

            // ドローする枚数分繰り返す
            for (int i = 0; i < drawCount; i++)
            {
                // もしデッキが空になった場合は終了する
                int randomIndex = Random.Range(0, playerDeck.Count);// ランダムに選んだ番号のカードをデッキから取り出す
                CardData selectedCard = playerDeck[randomIndex];// デッキから取り出したカードをリストから削除する

                GameObject cardObj = Instantiate(handCardPrefab, handArea);// カードのPrefabを手札エリアに生成する

                // 生成したカードのUIクラスに、選んだカードのデータをセットアップする
                HandCardUI handCardUI = cardObj.GetComponent<HandCardUI>();// 生成したカードのUIクラスを取得
                handCardUI.Setup(selectedCard);// カードのUIクラスにカードのデータをセットアップする
            }
        }
    }
}