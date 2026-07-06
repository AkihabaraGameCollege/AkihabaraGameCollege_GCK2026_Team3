using UnityEngine;
using UnityEngine.UI;
using ForestDraw.Enemy;
using ForestDraw.Player.Combat;
using System.Collections.Generic;

namespace ForestDraw
{
    /// <summary>
    /// 特殊カード関係のイベントを管理するクラス
    /// </summary>
    public class Special_CardManager : MonoBehaviour
    {
        /// <summary>
        /// 敵情報管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private EnemyManager _enemyManager;
        /// <summary>
        /// 特殊カードのUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private Special_CardUI_Manager _special_CardUI_Manager;
        /// <summary>
        /// プレイヤーのコスト管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private PlayerCost _playerCost;
        /// <summary>
        /// プレイヤーの体力管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private TreeHealth _treeHealth;
        /// <summary>
        /// 戦闘カードマネージャーを参照する変数
        /// </summary>
        [SerializeField]
        private BattleCardManager _battleCardManager;

        /// <summary>
        /// 使用するカードのデータを参照する変数
        /// </summary>
        [SerializeField]
        public CardData UsedCardData;

        /// <summary>
        /// 全ての特殊カードのデータを登録しておくマスターリスト
        /// </summary>
        [SerializeField]
        private List<CardData> _allSpecialCardList = new List<CardData>();

        /// <summary>
        /// カード使用対象の位置を示す変数
        /// </summary>
        [SerializeField]
        private Vector3 _executeCardTargetTransform;

        /// <summary>
        /// 一度に付与するポイントの量を参照する変数
        /// </summary>
        [SerializeField]
        private float _awardPoint = 0.05f;

        /// <summary>
        /// 特殊カードマネージャーのシングルトンインスタンスを参照する変数
        /// </summary>
        public static Special_CardManager Instance { get; private set; }

        /// <summary>
        /// 特殊カードを使用できるまでに必要なポイントを参照する変数
        /// </summary>
        private float _pointMaxNumber = 1;
        /// <summary>
        /// 現在のポイントを参照する変数
        /// </summary>
        private float _currentPoint = 0;

        /// <summary>
        /// 時間を停止するかどうかのフラグを参照する変数
        /// </summary>
        private bool _isStopTime = false;
        /// <summary>
        /// 特殊カードを使用できるようになったかのフラグを参照する変数
        /// </summary>
        private bool _isCanExecuted = false;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしシングルトンインスタンスがまだ存在しない場合
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // EnemyManagerの「敵が死んだイベント」に、GetPoint関数を登録する
            _enemyManager.OnEnemyDie += GetPoint;
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // もし時間停止のフラグが音の場合
            if (_isStopTime)
            {
                // 時を止める（ポーズ操作時のバグ対策）
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// ポイントを付与する関数
        /// </summary>
        private void GetPoint()
        {
            // もし特殊カード使用可能済みなら
            if (_isCanExecuted)
            {
                return;
            }

            // --- エネミー撃破ポイントをプレイヤーに付与する ---
            // ポイントを付与
            _currentPoint += _awardPoint;
            // UIを更新する
            _special_CardUI_Manager.UpdateGage_Image(_currentPoint, _pointMaxNumber);

            // もし現在のポイントが必要ポイント以上かつ特殊カード使用可能でない場合
            if (_currentPoint >= _pointMaxNumber&& !_isCanExecuted)
            {
                // 時間停止のフラグをオン
                _isStopTime = true;

                // マスターリストからランダムに2枚のカードを重複なしで抽選する
                List<CardData> drawnCards = GetRandomCards(2);

                // もしマスターリストにカードが2枚以上登録されている場合
                if (drawnCards.Count >= 2)
                {
                    // --- 抽選されたカードをUIのボタンにセットする ---
                    SetupCardButton(0, drawnCards[0]);
                    SetupCardButton(1, drawnCards[1]);
                }

                // カード選択のUIを表示
                _special_CardUI_Manager.TargetShow(_special_CardUI_Manager.CardSelectUI);
                // 特殊カード使用可能のフラグをオン
                _isCanExecuted = true;

                return;
            }
        }

        /// <summary>
        /// エラー対策用の関数
        /// </summary>
        private void OnDestroy()
        {
            // もしエネミー管理クラス参照変数の中身がある場合
            if (_enemyManager != null)
            {
                // このオブジェクトが消えるときは、イベントの登録を解除する
                _enemyManager.OnEnemyDie -= GetPoint;
            }
        }

        /// <summary>
        /// 特殊カードの能力を発動する関数
        /// </summary>
        public void ActiveSpecial_Card(CardData selectedCard)
        {
            // --- 止まっていた時を動かす ---
            // 時間停止のフラグをオフ
            _isStopTime = false;
            // 時を動かす
            Time.timeScale = 1f;

            // カード選択のUIを非表示
            _special_CardUI_Manager.TargetHide(_special_CardUI_Manager.CardSelectUI);

            // インスペクター確認用に代入
            UsedCardData = selectedCard;

            // --- 特殊カードの能力を発動する ---
            // 指定したカードの能力を発動
            BattleCardManager.Instance.CardAbilityExecute(selectedCard);
        }

        /// <summary>
        /// マスターリストから指定した枚数だけランダムに重複なしでカードを取得する関数
        /// </summary>
        private List<CardData> GetRandomCards(int count)
        {
            // --- 元のリストを崩さないようにコピーを作成 ---
            List<CardData> pool = new List<CardData>(_allSpecialCardList);
            List<CardData> result = new List<CardData>();

            // 指定した枚数分サーチ
            for (int i = 0; i < count; i++)
            {
                // もしプールが空になった場合
                if (pool.Count == 0)
                {
                    break;
                }

                // --- カードをランダムに抽選 ---
                // プール内の枚数分をランダムに抽選し参照する変数を定義
                int random_Index = Random.Range(0, pool.Count);
                // ランダムに選んだカードを結果に追加
                result.Add(pool[random_Index]);
                // 選んだカードをプールから消す（重複防止）
                pool.RemoveAt(random_Index);
            }

            // 結果を返す
            return result;
        }

        /// <summary>
        /// 指定したボタンの画像と、クリック時の機能をセットする関数
        /// </summary>
        private void SetupCardButton(int button_Index, CardData cardData)
        {
            // 特殊カード選択ボタンを参照する変数を定義
            Button button = _special_CardUI_Manager.AddSpecial_Card_Button[button_Index];

            // --- 画像の更新 ---
            // CardDataの中にある画像変数の名前に合わせて変更し参照する変数を定義
            Image button_Image = button.GetComponent<Image>();
            // もしボタンの画像があり、カードデータの画像もある場合
            if (button_Image != null && cardData.cardDetail_Image != null)
            {
                // 画像をそのカードの画像に変更
                button_Image.sprite = cardData.cardDetail_Image;
            }

            // --- クリックイベントの登録 ---
            // 以前の登録をリセット
            button.onClick.RemoveAllListeners();
            // 選ばれたカードを渡すように登録
            button.onClick.AddListener(() => ActiveSpecial_Card(cardData));
        }
    }
}