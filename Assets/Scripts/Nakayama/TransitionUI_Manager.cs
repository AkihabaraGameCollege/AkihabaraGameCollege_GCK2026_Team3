using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// 演出用UIを管理するクラス
    /// </summary>
    public class TransitionUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            Hide();
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