using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// クリアシーンの管理クラス
    /// </summary>
    public class ClearScene : MonoBehaviour
    {
        /// <summary>
        /// オーディオ設定の変数
        /// </summary>
        [SerializeField]
        private AudioSetting audioSetting = null;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";
        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        public string playerRootName = "PlayerRootClear";

        /// <summary>
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;

        /// <summary>
        /// クリアシーンで何番目のBGMを再生するかのインデックスを参照する変数
        /// </summary>
        private int bgmIndex = 5;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
            audioSetting.PlaySE(1);
            audioSetting.PlayBGM(bgmIndex);
            playerController.isCanPause = true;// ポーズ操作を許可する
        }

        /// <summary>
        /// タイトルシーンへ遷移する関数
        /// </summary>
        public void InTitleScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        }
    }
}