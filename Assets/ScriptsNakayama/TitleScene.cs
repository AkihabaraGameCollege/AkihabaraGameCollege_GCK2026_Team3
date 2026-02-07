using UnityEngine;

namespace CardDefenseGame
{
    /// <summary>
    /// タイトルシーンの管理クラス
    /// </summary>
    public class TitleScene : MonoBehaviour
    {
        /// <summary>
        /// ステージシーンへ遷移する関数
        /// </summary>
        public void InStageScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage");
        }
    }
}