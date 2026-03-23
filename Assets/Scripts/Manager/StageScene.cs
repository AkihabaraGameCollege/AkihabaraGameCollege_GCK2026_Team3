using UnityEngine;
using UnityEngine.UI;

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
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;
        /// <summary>
        /// メインステージ管理クラスのインスタンスを参照する変数
        /// </summary>
        public static StageScene Instance { get; private set; } = null;

        /// <summary>
        /// クリアボタンを参照する変数
        /// </summary>
        public Button clearButton = null;
        /// <summary>
        /// クリアボタンを参照する変数
        /// </summary>
        public Button gameOverButton = null;

        /// <summary>
        /// 最大のステージ番号を参照する変数
        /// </summary>
        private int stageNumberMax = 3;
        /// <summary>
        /// 今いるステージ番号を参照する変数
        /// </summary>
        public int stageNumber = 1;

        /// <summary>
        /// クリアシーン名を参照する変数
        /// </summary>
        private string clearSceneName = "Clear";
        // <summary>
        /// ステージシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";
        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        public string playerRootName = "PlayerRootStage";

        /// <summary>
        /// ステージの状況管理用(駒田追加)
        /// </summary>
        private enum SceneState
        {
            // ステージ開始演出中
            Intro,
            // ステージプレイ中
            Play,
            // ゲームオーバーが確定していて演出中
            GameOver,
            // ステージクリアーが確定していて演出中
            StageClear,
        }
        SceneState sceneState = SceneState.Intro;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Awake()
        {
            Instance = this; // 自分自身をインスタンスとして保存
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得

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

            // ボタンイベントの登録
            clearButton.onClick.AddListener(InClearScene);// クリアボタンにシーン遷移の関数を登録
            gameOverButton.onClick.AddListener(GameOver);// ゲームオーバーボタンにシーン遷移の関数を登録

            playerController.isCanPause = true;// ポーズ操作を許可する

            sceneState = SceneState.Play;
        }

        /// <summary>
        /// クリアシーンへ遷移する関数
        /// </summary>
        public void InClearScene()
        {
            if (sceneState == SceneState.Play)
            {
                sceneState = SceneState.StageClear;
                int number = stageNumber;// 今いるステージ番号を参照

                // 最終ステージの場合
                if (number == stageNumberMax)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(clearSceneName);
                }
                else
                {
                    TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
                    UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
                }
            }
        }

        /// <summary>
        /// ゲームオーバーした際にゲームオーバー画面を表示する関数
        /// </summary>
        public void GameOver()
        {
            if (sceneState == SceneState.Play)
            {
                sceneState = SceneState.GameOver;
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
                TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
                UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
            }
        }
    }
}