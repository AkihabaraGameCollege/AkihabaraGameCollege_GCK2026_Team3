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
        /// ゲームオーバーUIの変数
        /// </summary>
        [SerializeField]
        private GameObject gameOverUI = null;
        /// <summary>
        /// プレイヤー関係UIの変数配列
        /// </summary>
       [SerializeField]
        private GameObject[] playerUI = null;

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
            // 配列内のゲームオブジェクトをすべて参照
            foreach (GameObject obj in playerUI)
            {
                // オブジェクトがあった場合
                if (obj != null) 
                {
                    obj.SetActive(true);
                }
            }

            gameOverUI.SetActive(false);

            audioSetting.PlayBGM(bgmIndex);
        }

        /// <summary>
        /// クリアシーンへ遷移する関数
        /// </summary>
        public void InClearScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Clear");
        }

        /// <summary>
        /// ゲームオーバーした際にゲームオーバー画面を表示する関数
        /// </summary>
        public void GameOver()
        {
            // 配列内のゲームオブジェクトをすべて参照
            foreach (GameObject obj in playerUI)
            {
                // オブジェクトがあった場合
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            gameOverUI.SetActive(true);
        }
    }
}