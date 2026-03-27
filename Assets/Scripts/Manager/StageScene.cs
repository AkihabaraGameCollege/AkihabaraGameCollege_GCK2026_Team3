using System.Collections;
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
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;
        /// <summary>
        /// メインステージ管理クラスのインスタンスを参照する変数
        /// </summary>
        public static StageScene Instance { get; private set; } = null;

        /// <summary>
        /// 次へボタンを参照する変数
        /// </summary>
        private Button nextButton = null;
        /// <summary>
        /// クリアボタンを参照する変数
        /// </summary>
        public Button clearButton = null;
        /// <summary>
        /// クリアボタンを参照する変数
        /// </summary>
        public Button gameOverButton = null;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator = null;

        /// <summary>
        /// パネルのイメージの変数
        /// </summary>
        private Image Panel_Image = null;

        /// <summary>
        /// パネルのスプライトのリスト変数
        /// </summary>
        public Sprite[] Panel_Sprite = null;

        /// <summary>
        /// 最大のステージ番号を参照する変数
        /// </summary>
        private int stageNumberMax = 3;
        /// <summary>
        /// タイトルの環境SEの何番を流すかのインデックスを参照する変数
        /// </summary>
        private int fireSeIndex = 3;
        /// <summary>
        /// 最初のステージ番号を参照する変数
        /// </summary>
        private int stageNumberStart = 1;
        /// <summary>
        /// 時を動かす数字を参照する変数
        /// </summary>
        private int timeCanMoveNumber = 1;
        /// <summary>
        /// BGMのインデックスを参照する変数
        /// </summary>
        public int tutorial_BgmIndex = 6;
        /// <summary>
        /// 今いるステージ番号を参照する変数
        /// </summary>
        public int stageNumber = 1;
        /// <summary>
        /// BGMのインデックスを参照する変数
        /// </summary>
        public int stageBgmIndex = 2;
        /// <summary>
        /// 第何ステージかのインデックスを参照する変数
        /// </summary>
        public int stageSceneIndex = 0;

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
        private string playerRootName = "PlayerRootStage";
        /// <summary>
        /// 次へボタンのオブジェクト名を参照する変数
        /// </summary>
        private string nextButtonName = "NextButton";
        /// <summary>
        /// デッキUIのオブジェクト名を参照する変数
        /// </summary>
        private string TreePanel_Name = "TreePanel_Image";

        /// <summary>
        /// チュートリアルを終了するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int tutorial_EndTrigger = Animator.StringToHash("Tutorial_End");

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

            // コンポーネントの登録
            animator = GetComponent<Animator>();
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
            if (GameObject.Find(nextButtonName) != null) nextButton = GameObject.Find(nextButtonName).GetComponent<Button>();// シーン内から次へボタンを探して取得
            Panel_Image = GameObject.Find(TreePanel_Name).GetComponent<Image>();// シーン内からステージパネルを探して取得

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

            // ボタンイベントの登録
            if (clearButton != null) clearButton.onClick.AddListener(InClearScene);// クリアボタンにシーン遷移の関数を登録
            if (gameOverButton != null) gameOverButton.onClick.AddListener(GameOver);// ゲームオーバーボタンにシーン遷移の関数を登録
            nextButton.onClick.AddListener(SkipTutorial);// 次へボタンにチュートリアルスキップの関数を登録

            playerController.isCanPause = true;// ポーズ操作を許可する

            Stage_Intro(stageNumber);// イントロ開始
        }

        /// <summary>
        /// クリアシーンへ遷移する関数
        /// </summary>
        public void InClearScene()
        {
            if (sceneState == SceneState.Play)
            {
                sceneState = SceneState.StageClear;

                TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
                UnityEngine.SceneManagement.SceneManager.LoadScene(clearSceneName);

                //int number = stageNumber;// 今いるステージ番号を参照

                //// 最終ステージの場合
                //if (number == stageNumberMax)
                //{
                //    UnityEngine.SceneManagement.SceneManager.LoadScene(clearSceneName);
                //}
                //else
                //{
                //    TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
                //    UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
                //}
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

        /// <summary>
        /// イントロ中の演出を行う関数
        /// </summary>
        /// <param name="number"></param>
        private void Stage_Intro(int number)
        {

            Panel_Image.sprite = Panel_Sprite[stageSceneIndex];// パネルのスプライトをステージに合わせて変更

            // もし第一ステージなら
            if (number == stageNumberStart)
            {
                Time.timeScale = 0;
                audioSetting.PlayBGM(tutorial_BgmIndex);
            }
            else 
            { 
                audioSetting.PlayBGM(stageBgmIndex);

                // もしステージが第三ステージなら
                if (stageNumber == stageNumberMax)
                {
                    audioSetting.PlayBGS(fireSeIndex);// ステージのBGSを再生
                }

                sceneState = SceneState.Play;
            }
    }

        /// <summary>
        /// チュートリアルをスキップする関数
        /// </summary>
        private void SkipTutorial()
        {
            animator.SetTrigger(tutorial_EndTrigger);// チュートリアル終了演出
            audioSetting.PlayBGM(stageBgmIndex);
            sceneState = SceneState.Play;
            Time.timeScale = timeCanMoveNumber;// 時を動かす
        }
    }
}