using UnityEngine;

namespace CardDefenseGame
{
    /// <summary>
    /// クリアシーンの管理クラス
    /// </summary>
    public class ClearScene : MonoBehaviour
    {
        /// <summary>
        /// タイトルシーンへ遷移する関数
        /// </summary>
        public void InTitleScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }
}