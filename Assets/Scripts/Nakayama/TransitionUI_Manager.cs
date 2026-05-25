using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// 演出用UIを管理するクラス
    /// </summary>
    public class TransitionUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 演出用UIのオブジェクト名を参照する変数
        /// </summary>
        public static TransitionUI_Manager instance { get; private set; }

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // インスタンスの重複チェック
            if (instance == null)
            {
                instance = this;
            }
        }

        /// <summary>         
        /// UIの表示をやめる関数         
        /// </summary>         
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>         
        /// 指定したUIの表示をやめる関数         
        /// </summary>         
        public void TargetShow(GameObject target)
        {
               target.SetActive(true);// 指定したUIをアクティブ化
        }
    }
}