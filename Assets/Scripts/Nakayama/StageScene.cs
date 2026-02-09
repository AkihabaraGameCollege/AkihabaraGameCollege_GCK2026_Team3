using UnityEngine;

namespace CardDefenseGame
{
    /// <summary>
    /// ステージシーンの管理クラス
    /// </summary>
    public class StageScene : MonoBehaviour
    {
        /// <summary>
        /// クリアシーンへ遷移する関数
        /// </summary>
        public void InClearScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Clear");
        }
    }
}