using TMPro;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// ダメージポップアップの管理を行うクラス
    /// </summary>
    public class DamagePopup: MonoBehaviour
    {
        /// <summary>
        /// TextMeshProコンポーネントへの参照をする変数
        /// </summary>
        [SerializeField] 
        private TextMeshPro _textMesh;

        /// <summary>
        /// ポップアップの動く速度を設定する変数
        /// </summary>
        [SerializeField] 
        private float _moveSpeed = 2.0f;

        /// <summary>
        /// ダメージポップアップの表示時間を設定する変数
        /// </summary>
        private float _lifeTime = 2.0f;

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            // 上へ移動（ふわっと浮かぶ）
            transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

            // もしメインカメラがある場合
            if (Camera.main != null)
            {
                // カメラと逆方向（自分 - カメラ）を向く
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
        }

        /// <summary>
        /// ダメージポップアップを初期化する関数
        /// </summary>
        /// <param name="damage"></param>
        public void Setup(int damage)
        {
            // ダメージ値を表示
            _textMesh.text = damage.ToString();
            // ポップアップを一定時間後に破棄
            Destroy(gameObject, _lifeTime);
        }
    }
}
