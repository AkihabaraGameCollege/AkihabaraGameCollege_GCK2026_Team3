using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// ステージシーンの管理クラス
    /// </summary>
    public class StageScene : MonoBehaviour
    {
        /// <summary>
        /// オーディオ設定の変数
        /// </summary>
        [SerializeField]
        private AudioSetting audioSetting = null;

        /// <summary>
        /// BGMのインデックスの変数
        /// </summary>
        [SerializeField]
        private int bgmIndex = 0;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            audioSetting.PlayBGM(bgmIndex);
        }

        /// <summary>
        /// クリアシーンへ遷移する関数
        /// </summary>
        public void InClearScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Clear");
        }
    }
}