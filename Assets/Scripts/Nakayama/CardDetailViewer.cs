using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// カード詳細UIを管理するクラス
    /// </summary>
    public class CardDetailViewer : MonoBehaviour
    {
        /// <summary>
        /// このクラスのシングルトンを参照する変数
        /// </summary>
        public static CardDetailViewer instance { get; private set; }

        /// <summary>
        /// カード詳細画像を表示するImageコンポーネントを参照する変数
        /// </summary>
        public Image detail_Image;

        /// <summary>
        /// カード詳細画像オブジェクトの座標を参照する変数
        /// </summary>
        public Transform detail_ImageTrasform;

        /// <summary>
        /// 表示したときのX軸方向の位置を調整する値を参照する変数
        /// </summary>
        public float showOffsetX;

        /// <summary>
        /// 表示したときのY軸方向の位置を調整する値を参照する変数
        /// </summary>
        public float showOffsetY;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // もしシングルトンが無い場合
            if (instance == null)
            {
                instance = this;// シングルトンの初期化
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 画像を受け取って表示する関数
        /// </summary>
        /// <param name="largeImage"></param>
        public void ShowDetail(Sprite largeImage, Vector3 targetPosition)
        {
            detail_Image.sprite = largeImage;
            gameObject.SetActive(true);

            detail_ImageTrasform.position = targetPosition;// カードUIと同じ座標に移動

           detail_ImageTrasform.position += new Vector3(showOffsetX, showOffsetY, 0);// 任意の分XY方向に移動したのちに表示

        }

        /// <summary>
        /// 非表示にする関数
        /// </summary>
        public void HideDetail()
        {
            gameObject.SetActive(false);
        }
    }
}