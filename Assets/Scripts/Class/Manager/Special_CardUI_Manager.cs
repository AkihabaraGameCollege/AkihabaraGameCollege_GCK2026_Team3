using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>     
    /// 特殊カード関連UIの管理を行うクラス     
    /// </summary>     
    public class Special_CardUI_Manager : MonoBehaviour
   {
        /// <summary>
        /// ゲージの画像を参照する変数
        /// </summary>
        public Image Gage_Image;

        /// <summary>         
        /// UIを表示する関数         
        /// </summary>         
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform _child in transform)
            {
                _child.gameObject.SetActive(true);
            }
        }

        /// <summary>         
        /// UIを隠す関数         
        /// </summary>         
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform _child in transform)
            {
                _child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ゲージの画像を更新する関数
        /// </summary>
        /// <param name="_currentGage"></param>
        /// <param name="_maxGage"></param>
        public void UpdateGage_Image(float _currentGage, float _maxGage)
        {
            // ゲージの画像の状態を変更する
            Gage_Image.fillAmount = (float)_currentGage / _maxGage;
        }
    }
}