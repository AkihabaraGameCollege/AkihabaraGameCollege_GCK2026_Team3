using ForestDraw.Enemy;
using ForestDraw.Player.Combat;
using UnityEngine;
using UnityEngine.UI;
using static ForestDraw.Player.Combat.CardUseExecutor;

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
        /// カード使用対象の位置を示す変数
        /// </summary>
        [SerializeField]
        private Vector3 _executeCardTargetTransform;

        /// <summary>
        /// 取得カード選択のUIオブジェクトを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _cardSelectUI;

        /// <summary>
        /// 特殊カード追加ボタンのリストを参照する変数
        /// </summary>
        [SerializeField]
        private Button[] _addSpecial_Card_Button;

        /// <summary>
        /// 使用するカードのデータを参照する変数
        /// </summary>
        [SerializeField]
        private CardData _usedCardData;

        /// <summary>
        /// 一度に付与するポイントの量を参照する変数
        /// </summary>
        [SerializeField]
        private float _pointNumber = 0.05f;

        /// <summary>
        /// 特殊カードを使用できるまでに必要なポイントを参照する変数
        /// </summary>
        private float _pointMaxNumber = 1;
        /// <summary>
        /// 現在のポイントを参照する変数
        /// </summary>
        private float _currentPoint = 0;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // EnemyManagerの「敵が死んだイベント」に、GetPoint関数を登録する
            _enemyManager.OnEnemyDie += GetPoint;

            // --- ボタンに関数を登録 ---
            // 特殊カード発動ボタンに特殊カードの能力を発動する関数を登録
            _addSpecial_Card_Button[0].onClick.AddListener(ActiveSpecial_Card);
            // 特殊カード発動ボタンに特殊カードの能力を発動する関数を登録
            _addSpecial_Card_Button[1].onClick.AddListener(ActiveSpecial_Card);
        }

        /// <summary>
        /// ポイントを付与する関数
        /// </summary>
        private void GetPoint()
        {
            // もし現在のポイントが必要ポイント以上になった場合
            if (_currentPoint >= _pointMaxNumber)
            {
                // 時を止める
                Time.timeScale = 0f;
                // カード選択のUIを表示
                _special_CardUI_Manager.TargetShow(_cardSelectUI);

                return;
            }

            // ポイントを付与
            _currentPoint += _pointNumber;

            // UIを更新する
            _special_CardUI_Manager.UpdateGage_Image(_currentPoint, _pointMaxNumber);
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
        public void ActiveSpecial_Card()
        {
            // 時を動かす
            Time.timeScale = 1f;
            // カード選択のUIを非表示
            _special_CardUI_Manager.TargetHide(_cardSelectUI);

            // --- 特殊カードの能力を発動する ---
            // カード選択のUIから、使用するカードの情報を取得すし参照する変数を定義
            var context = new CardUseContext
            {
                PlayerCostClass = _playerCost,
                TreeHealthClass = _treeHealth,
                BattleCardManagerClass = _battleCardManager,
                ExecuteCardTargetTransform = _executeCardTargetTransform
            };
            // カード使用の実行クラスの関数を呼び出し、カードの能力を発動する
            CardUseExecutor.Execute(_usedCardData, context);
        }
    }
}