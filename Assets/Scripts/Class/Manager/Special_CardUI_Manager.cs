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
        [SerializeField]
        private Image _gage_Image;

        /// <summary>
        /// ゲージ系のUIオブジェクトを参照する変数
        /// </summary>
        [SerializeField]
        private GameObject _gageUI;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // 最初はUIを非表示
            Hide();
            // ゲージ系のUIを表示
            TargetShow(_gageUI);
        }

        /// <summary>
        /// 指定したUIの表示を行う関数
        /// </summary>
        /// <param name="_target"></param>
        public void TargetShow(GameObject _target)
        {
            // 指定したUIをアクティブ化
            _target.SetActive(true);
        }

        /// <summary>
        /// 指定したUIの非表示を行う関数
        /// </summary>
        /// <param name="_target"></param>
        public void TargetHide(GameObject _target)
        {
            // 指定したUIを非アクティブ化
            _target.SetActive(false);
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
            _gage_Image.fillAmount = (float)_currentGage / _maxGage;
        }
    }
}