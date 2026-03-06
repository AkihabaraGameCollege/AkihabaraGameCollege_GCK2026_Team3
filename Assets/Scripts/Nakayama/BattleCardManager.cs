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
        /// ドローするための山札のリスト変数
        /// </summary>
        private List<CardData> drawPile = new List<CardData>();

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
            InitializeDrawPile();// デッキをシャッフルして、カードを引く準備をする
            DrawCards(4);// 4枚引く
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
        /// ドローするための山札をシャッフルして準備する関数
        /// </summary>
        private void InitializeDrawPile()
        {
            drawPile = new List<CardData>(playerDeck);// デッキの内容を山札にコピーする

            // ドローのたびに山札の順番が変わるように、シャッフルするループ
            for (int i = drawPile.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);// 0からiの範囲でランダムなインデックスを選ぶ
                CardData temp = drawPile[i];// i番目のカードを一時的に保存する
                drawPile[i] = drawPile[j];// j番目のカードをi番目に移動する
                drawPile[j] = temp;// 一時的に保存しておいたカードをj番目に移動する
            }
        }

        /// <summary>
        /// ドローする関数
        /// </summary>
        /// <param name="drawCount"></param>
        private void DrawCards(int drawCount)
        {
            // 指定された枚数だけ引くループ
            for (int i = 0; i < drawCount; i++)
            {
                // もし山札が空の場合
                if (drawPile.Count == 0)
                {
                    break;
                }

                CardData drawnCard = drawPile[0];// 山札の一番上のカードを引く

                drawPile.RemoveAt(0);// 山札から引いたカードを削除する

                GameObject cardObj = Instantiate(handCardPrefab, handArea);// カードのPrefabを生成して、手札エリアの子オブジェクトにする
                HandCardUI handCardUI = cardObj.GetComponent<HandCardUI>();// 生成したカードオブジェクトからHandCardUIコンポーネントを取得する
                handCardUI.Setup(drawnCard);// 取得したHandCardUIコンポーネントのSetup関数を呼び出して、引いたカードのデータを渡す
            }
        }
    }
}