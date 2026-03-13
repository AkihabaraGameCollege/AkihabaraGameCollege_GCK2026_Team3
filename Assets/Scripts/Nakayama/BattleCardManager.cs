using System.Collections.Generic;
using UnityEngine;
using ForestDraw.Player.Combat;
using static ForestDraw.Player.Combat.CardUseExecutor;
using static UnityEngine.GraphicsBuffer;

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
        private GameObject handCardPrefab = null;

        /// <summary>
        /// 空間上でカードを並べる場所の変数
        /// </summary>
        [SerializeField]
        private Transform handArea = null;
        /// <summary>
        /// プレイヤーの
        /// </summary>
        [SerializeField]
        private GameObject player;

        /// <summary>
        /// ドローする間隔の変数
        /// </summary>
        [SerializeField]
        private float drawInterval = 3f;

        /// <summary>
        /// 手持ちのカードの最大枚数の変数
        /// </summary>
        [SerializeField]
        private int maxHandSize = 8;
        /// <summary>
        /// スタート時に引くカードの枚数の変数
        /// </summary>
        [SerializeField]
        private int startDrawCount = 4;
        /// <summary>
        /// コストの量を指定する変数
        /// </summary>
        [SerializeField]
        private int costCount = 4;

        /// <summary>
        /// プレイヤーのコストの管理クラスを指定する変数
        /// </summary>
        private PlayerCost playerCost = null;
        
        /// <summary>
        /// プレイヤーのHPの管理クラスを指定する変数
        /// </summary>
        private PlayerHealth playerHealth = null;

        /// <summary>
        /// リストに全カードのマスターデータを入れておく変数
        /// </summary>
        public List<CardData> allCardMasterList;
        /// <summary>
        /// リストにロードしたデッキの中身を入れておく変数
        /// </summary>
        private List<CardData> playerDeck = new();
        /// <summary>
        /// ドローするための山札のリスト変数
        /// </summary>
        private List<CardData> drawPile = new();

        /// <summary>
        /// セーブ機能で使うキーの定数の変数
        /// </summary>
        private const string SAVE_KEY = "UserDeckSaveData";

        /// <summary>
        /// ドローのタイミングを管理するためのタイマーの変数
        /// </summary>
        private float drawTimer = 0f;

        /// <summary>
        /// スタート時のシャッフルの回数の変数
        /// </summary>
        private int shuffleStartCount = 1;
        /// <summary>
        /// ランダムなインデックスを生成するための変数
        /// </summary>
        private int shuffleRangeIndex = 1;
        /// <summary>
        /// 自動ドローするカードの枚数の変数
        /// </summary>
        private int drawCount = 1;

        /// <summary>
        /// 初期設定の関数
        /// </summary>

        private void Awake()
        {
            playerCost = GetComponent<PlayerCost>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        private void Start()
        {
            LoadDeckData();
            InitializeDrawPile();// デッキをシャッフルして、カードを引く準備をする
            DrawCards(startDrawCount);// ?枚引く
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
            for (int i = drawPile.Count - shuffleStartCount; i > 0; i--)
            {
                int j = Random.Range(0, i + shuffleRangeIndex);// 0からiの範囲でランダムなインデックスを選ぶ
                CardData temp = drawPile[i];// i番目のカードを一時的に保存する
                drawPile[i] = drawPile[j];// j番目のカードをi番目に移動する
                drawPile[j] = temp;// 一時的に保存しておいたカードをj番目に移動する
            }
        }

        /// <summary>
        /// ドローする関数
        /// </summary>
        /// <param name="drawCount"></param>
        public void DrawCards(int drawCount)
        {
            // 指定された枚数だけ引くループ
            for (int i = 0; i < drawCount; i++)
            {
                Debug.Log(drawCount + "枚引く");
                // もし手札の枚数が最大枚数以上の場合
                if (handArea.childCount >= maxHandSize)
                {
                    break;
                }
                // もし山札が空の場合
                else if (drawPile.Count == 0)
                {
                    break;
                }

                CardData drawnCard = drawPile[0];// 山札の一番上のカードを引く

                drawPile.RemoveAt(0);// 山札から引いたカードを削除する

                GameObject cardObj = Instantiate(handCardPrefab, handArea);// カードのPrefabを生成して、手札エリアの子オブジェクトにする
                HandCardUI handCardUI = cardObj.GetComponent<HandCardUI>();// 生成したカードオブジェクトからHandCardUIコンポーネントを取得する
                handCardUI.Setup(drawnCard, this);// 取得したHandCardUIコンポーネントのSetup関数を呼び出して、引いたカードのデータを渡す
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される関数
        /// </summary>
        private void Update()
        {
            drawTimer += Time.deltaTime;

            // もしドローのタイマーがドローする間隔を超えた場合
            if (drawTimer >= drawInterval)
            {
                drawTimer = 0f;

                // もし手札の枚数が最大枚数より少なくて、山札にカードが残っている場合
                if (handArea.childCount < maxHandSize && drawPile.Count > 0)
                {
                    DrawCards(drawCount);
                }
            }
        }

        /// <summary>
        /// カードを使用する関数
        /// </summary>
        /// <param name="usedCard"></param>
        /// <param name="cardObj"></param>
        public bool UseCard(CardData usedCard, GameObject cardObj)
        {
            var context = new CardUseContext
            {
                playerCost = playerCost,
                playerHealth = playerHealth,
                cardManager = this,
                target = player.transform.position
            };
            if (!CardUseExecutor.Execute(usedCard, context)) return false;

            drawPile.Add(usedCard);// 使用するカードを山札の一番下に戻す

            Destroy(cardObj);

            return true;
        }
    }
}