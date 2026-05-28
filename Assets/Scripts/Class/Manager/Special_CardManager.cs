using ForestDraw.Enemy;
using ForestDraw.Enemy.Components;
using UnityEngine;

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
        public EnemyManager EnemyManagerClass;
        /// <summary>
        /// 特殊カードのUI管理クラスを参照する変数
        /// </summary>
        public Special_CardUI_Manager Special_CardUI_ManagerClass;

        /// <summary>
        /// 特殊カードを使用できるまでに必要なポイントを参照する変数
        /// </summary>
        private float _pointMaxNumber = 1;
        /// <summary>
        /// 現在のポイントを参照する変数
        /// </summary>
        private float _currentPoint = 0;
        /// <summary>
        /// 一度に付与するポイントの量を参照する変数
        /// </summary>
        public float PointNumber = 0.05f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // EnemyManagerの「敵が死んだイベント」に、GetPoint関数を登録する
            EnemyManagerClass.OnEnemyDie += GetPoint;
        }

        /// <summary>
        /// ポイントを付与する関数
        /// </summary>
        private void GetPoint()
        {
            // もし現在のポイントが必要ポイント以上になった場合
            if (_currentPoint >= _pointMaxNumber)
            {
                Debug.Log("Special_Card!!");
                return;
            }

            // ポイントを付与
            _currentPoint += PointNumber;

            // UIを更新する
            Special_CardUI_ManagerClass.UpdateGage_Image(_currentPoint, _pointMaxNumber);
        }

        /// <summary>
        /// エラー対策用の関数
        /// </summary>
        private void OnDestroy()
        {
            // もしエネミー管理クラス参照変数の中身がある場合
            if (EnemyManagerClass != null)
            {
                // このオブジェクトが消えるときは、イベントの登録を解除する
                EnemyManagerClass.OnEnemyDie -= GetPoint;
            }
        }
    }
}