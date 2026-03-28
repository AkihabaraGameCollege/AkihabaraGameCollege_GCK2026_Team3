using System;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// 通知UIの管理を行うクラス
    /// </summary>
    public class NoticeTextUI : MonoBehaviour
    {
        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            Hide();
        }

        /// <summary>         
        /// UIの表示を行う関数         
        /// </summary>         
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
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
    }
}
