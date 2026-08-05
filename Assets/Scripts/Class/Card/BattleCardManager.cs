using ForestDraw.Player.Combat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ForestDraw.Player.Combat.CardUseExecutor;

namespace ForestDraw
{
    /// <summary>
    /// バトルシーンにて編成画面で作ったデッキからカードを引いて手札に表示するクラス
    /// </summary>
    public class BattleCardManager : MonoBehaviour
    {
        /// <summary>
        /// カードのPrefabと、生成する親オブジェクトの変数
        /// </summary>
        [SerializeField]
        private GameObject _handCardPrefab;

        /// <summary>
        /// 空間上でカードを並べる場所の変数
        /// </summary>
        [SerializeField]
        private Transform _handArea;
        /// <summary>
        /// 空間上でカードを並べる場所の変数
        /// </summary>
        [SerializeField]
        private Transform _cardUseEffectPoision;

        /// <summary>
        /// カード使用時に出るエフェクトを参照する変数
        /// </summary>
        [SerializeField]
        private ParticleSystem _cardUseEffect;

        /// <summary>
        /// ドローする間隔の変数
        /// </summary>
        [SerializeField]
        private float _draw_Interval = 3f;

        /// <summary>
        /// 手持ちのカードの最大枚数の変数
        /// </summary>
        [SerializeField]
        private int _maxHandSize = 8;

        /// <summary>
        /// 手持ちのカード管理クラスのインスタンスを参照する変数
        /// </summary>
        public static BattleCardManager Instance { get; private set; }
        /// <summary>
        /// プレイヤーのコストの管理クラスを指定する変数
        /// </summary>
        private PlayerCost _playerCost;
        /// <summary>
        /// プレイヤーのHPの管理クラスを指定する変数
        /// </summary>
        private TreeHealth _playerHealth;

        /// <summary>
        /// リストに全カードのマスターデータを入れておく変数
        /// </summary>
        [SerializeField]
        private List<CardData> _allCardMasterList;

        /// <summary>
        /// リストにロードしたデッキの中身を入れておく変数
        /// </summary>
        private List<CardData> _playerDeck = new();
        /// <summary>
        /// ドローするための山札のリスト変数
        /// </summary>
        private List<CardData> _drawPile = new();

        /// <summary>
        /// セーブ機能で使うキーの定数の変数
        /// </summary>
        private const string _saveKey = "UserDeckSaveData";

        /// <summary>
        /// ドローのタイミングを管理するためのタイマーの変数
        /// </summary>
        private float _drawTimer = 0f;

        /// <summary>
        /// スタート時のシャッフルの回数の変数
        /// </summary>
        private int _shuffleStartCount = 1;
        /// <summary>
        /// ランダムなインデックスを生成するための変数
        /// </summary>
        private int _shuffleRange_Index = 1;
        /// <summary>
        /// 自動ドローするカードの枚数の変数
        /// </summary>
        private int _drawCount = 1;
        /// <summary>
        /// ドロー時のSEインデックスを参照する変数
        /// </summary>
        private int _drawSE_Index = 5;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Awake()
        {
            // もしインスタンスが無い場合
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // --- コンポーネントの登録 ---
            _playerCost = GetComponent<PlayerCost>();
            _playerHealth = gameObject.GetComponent<TreeHealth>();
        }

        /// <summary>
        /// 初回起動時の処理を行う関数
        /// </summary>
        private void Start()
        {
            // デッキの中身をロードする関数を呼び出す
            LoadDeckData();
            // デッキをシャッフルして、カードを引く準備をする
            InitializeDrawPile();
        }

        /// <summary>
        /// ロードしてデッキの中身をplayerDeckリストに入れる関数
        /// </summary>
        private void LoadDeckData()
        {
            // もしセーブデータがある場合
            if (PlayerPrefs.HasKey(_saveKey))
            {
                // セーブデータをJSONからクラスに変換
                string jsonString = PlayerPrefs.GetString(_saveKey);
                // ロードしたカードIDを元に、マスターデータからカードを探してデッキに追加
                DeckSaveData loadedData = JsonUtility.FromJson<DeckSaveData>(jsonString);

                // もしセーブデータのカードIDがマスターデータに存在する場合
                foreach (string id in loadedData.savedCardIds)
                {
                    // マスターデータからIDが一致するカードを探す
                    CardData foundCard = _allCardMasterList.Find(card => card.cardId == id);

                    // もし見つかった場合
                    if (foundCard != null)
                    {
                        // デッキに追加
                        _playerDeck.Add(foundCard);
                    }
                }
            }
        }

        /// <summary>
        /// ドローするための山札をシャッフルして準備する関数
        /// </summary>
        private void InitializeDrawPile()
        {
            // デッキの内容を山札にコピーする
            _drawPile = new List<CardData>(_playerDeck);

            // ドローのたびに山札の順番が変わるように、シャッフルするループ
            for (int i = _drawPile.Count - _shuffleStartCount; i > 0; i--)
            {
                // 0からiの範囲でランダムなインデックスを選ぶ
                int j = Random.Range(0, i + _shuffleRange_Index);
                // i番目のカードを一時的に保存する
                CardData template = _drawPile[i];
                // j番目のカードをi番目に移動する
                _drawPile[i] = _drawPile[j];
                // 一時的に保存しておいたカードをj番目に移動する
                _drawPile[j] = template;
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
                // もし手札の枚数が最大枚数以上の場合
                if (_handArea.childCount >= _maxHandSize)
                {
                    break;
                }
                // もし山札が空の場合
                else if (_drawPile.Count == 0)
                {
                    break;
                }

                // 山札の一番上のカードを引く
                CardData drawnCard = _drawPile[0];

                // カードを引くSEを再生する
                AudioSetting.Instance.PlaySE(_drawSE_Index);

                // 山札から引いたカードを削除する
                _drawPile.RemoveAt(0);

                // カードのPrefabを生成して、手札エリアの子オブジェクトにする
                GameObject cardObject = Instantiate(_handCardPrefab, _handArea);
                // 生成したカードオブジェクトからHandCardUIコンポーネントを取得する
                HandCardUI_Manager handCardUI = cardObject.GetComponent<HandCardUI_Manager>();
                // 取得したHandCardUIコンポーネントのSetup関数を呼び出して、引いたカードのデータを渡す
                handCardUI.Setup(drawnCard);
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される関数
        /// </summary>
        private void Update()
        {
            // ドローのタイマーを更新する
            _drawTimer += Time.deltaTime;

            // もしドローのタイマーがドローする間隔を超えた場合
            if (_drawTimer >= _draw_Interval)
            {
                // タイマーをリセットする
                _drawTimer = 0f;

                // もし手札の枚数が最大枚数より少なくて、山札にカードが残っている場合
                if (_handArea.childCount < _maxHandSize && _drawPile.Count > 0)
                {
                    // カードを引く関数を呼び出す
                    DrawCards(_drawCount);
                }
            }
        }

        /// <summary>
        /// カードを使用する関数
        /// </summary>
        /// <param name="usedCard"></param>
        /// <param name="cardObject"></param>
        public bool UseCard(CardData usedCard, GameObject cardObject)
        {
            // もしプレイヤーのコストがカードのコストより少ない場合
            if (!_playerCost.UseCost(usedCard.cost))
            {
                return false;
            }
            else
            {
                // カード能力を発動
                CardAbilityExecute(usedCard);

                // --- 使用後の後片付け ---
                // 使用するカードを山札の一番下に戻す
                _drawPile.Add(usedCard);
                // 使用したカードを破壊
                Destroy(cardObject);
                // trueで返す
                return true;
            }
        }

        /// <summary>
        /// カード能力を発動する関数
        /// </summary>
        /// <param name="usedCard"></param>
        public void CardAbilityExecute(CardData usedCard)
        {
            // カード使用時の情報をまとめたコンテキストを新しく生成し参照する変数を定義
            var context = new CardUseContext
            {
                // --- 情報を代入 ---
                PlayerCostClass = _playerCost,
                TreeHealthClass = _playerHealth,
                BattleCardManagerClass = this,
                ExecuteCardTargetTransform = gameObject.transform.position
            };

            // もしカード能力発動クラスが機能していない場合
            if (!CardUseExecutor.Execute(usedCard, context))
            {
                return;
            }

            // カード使用SEを再生
            AudioSetting.Instance.PlaySE(11);

            // 使用時のエフェクトを生成
            Instantiate(_cardUseEffect, _cardUseEffectPoision.transform);
        }
    }
}