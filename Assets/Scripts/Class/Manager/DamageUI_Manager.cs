using UnityEngine;

namespace ForestDraw
{
    /// <summary>     
    /// ポーズUIの管理を行うクラス     
    /// </summary>     
    public class DamageUI_Manager : MonoBehaviour
    {
        /// <summary>         
        /// 初期設定を行う関数         
        /// </summary>         
        private void Awake()
        {
            Hide();// 起動時はUIを隠す
        }

        /// <summary>         
        /// UIを表示する関数         
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
        /// UIを隠す関数         
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