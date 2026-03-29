using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
        /// ステージイントロの木の板UIを参照する変数
        /// </summary>
        public GameObject treePanel_UI;

        /// <summary>
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController;
        /// <summary>
        /// チュートリアルUI管理クラスを参照する変数
        /// </summary>
        private Tutorial_UI_Manager tutorial_UI_Manager;
        /// <summary>
        /// 演出用UI管理クラスを参照する変数
        /// </summary>
        private TransitionUI_Manager transitionUI_Manager;
        /// <summary>
        /// ポーズ管理クラスを参照する変数
        /// </summary>
        private PauseUI_Manager pauseUI_Manager;
        /// <summary>
        /// メインステージ管理クラスのインスタンスを参照する変数
        /// </summary>
        public static StageScene Instance { get; private set; } 

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator = null;

        /// <summary>
        /// パネルのイメージの変数
        /// </summary>
        public Image Panel_Image;

        /// <summary>
        /// パネルのスプライトのリスト変数
        /// </summary>
        public Sprite[] Panel_Sprite = null;

        /// <summary>         
        /// 戻るボタンの変数         
        /// </summary>         
        public Button resumeButton;
        /// <summary>         
        /// 設定ボタンの変数         
        /// </summary>         
        public Button settingButton;
        /// <summary>         
        /// ステージセレクトボタンの変数         
        /// </summary>         
        public Button exitButton;
        /// <summary>
        /// 次へボタンを参照する変数
        /// </summary>
        public Button nextButton;

        /// <summary>         
        /// ResumeButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onResumeButtonClick;
        /// <summary>         
        /// SettingButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onSettingButtonClick;
        /// <summary>         
        /// ExitButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onExitButtonClick;

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
        public int stagePanel_Index = 0;
        /// <summary>
        /// チュートリアルを終了するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int tutorial_EndTrigger = Animator.StringToHash("Tutorial_End");

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
        /// チュートリアルUIのオブジェクト名を参照する変数
        /// </summary>
        private string tutorial_UI_Name = "Tutorial_UI";
        /// <summary>
        /// 演出用UIのオブジェクト名を参照する変数
        /// </summary>
        private string transitionUI_Name = "TransitionUI";
        /// <summary>
        /// ポーズUIのオブジェクト名を参照する変数
        /// </summary>
        private string pauseUI_Name = "PauseUI";

        /// <summary>
        /// チュートリアルのイントロ時間を参照する変数
        /// </summary>
        private float tutorial_IntroTime = 3.5f;
        /// <summary>
        /// チュートリアルのアウトロ時間を参照する変数
        /// </summary>
        private float tutorial_OutroTime = 1f;
        /// <summary>
        /// ステージイントロの時間を参照する変数
        /// </summary>
        private float stage_IntroTime = 1f;
        /// <summary>
        /// 時を動かす値を参照する変数
        /// </summary>
        private float timeCanMoveValue = 1f;

        /// <summary>
        /// チュートリアル中かどうかのフラグを参照する変数
        /// </summary>
        private bool isTutorial = false;

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
            tutorial_UI_Manager = GameObject.Find(tutorial_UI_Name).GetComponent<Tutorial_UI_Manager>();// シーン内からチュートリアルUIを探して取得
            transitionUI_Manager = GameObject.Find(transitionUI_Name).GetComponent<TransitionUI_Manager>();// シーン内から演出用UIを探して取得
            pauseUI_Manager = GameObject.Find(pauseUI_Name).GetComponent<PauseUI_Manager>();// シーン内からポーズUIを探して取得

            // UnityEvent を追加
            resumeButton.onClick.AddListener(() => { onResumeButtonClick.Invoke(); });// 戻るボタンのイベントを設定
            settingButton.onClick.AddListener(() => { onSettingButtonClick.Invoke(); });// 設定ボタンのイベントを設定
            exitButton.onClick.AddListener(() => { onExitButtonClick.Invoke(); });// ステージセレクトボタンのイベントを設定

            nextButton.enabled = false;

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

        /// <summary>
        /// イントロ中の演出を行う関数
        /// </summary>
        /// <param name="number"></param>
        private void Stage_Intro(int number)
        {
            Panel_Image.sprite = Panel_Sprite[stagePanel_Index];// パネルのスプライトをステージに合わせて変更
            transitionUI_Manager.TargetShow(treePanel_UI);// パネルを表示

            // もし第一ステージなら
            if (number == stageNumberStart)
            {
                StartCoroutine(Tutorial_IntroCoroutine());// チュートリアル開始
            }
            else 
            { 
                StartCoroutine(Stage_IntroCoroutine());// ステージイントロ開始
            }
    }

        /// <summary>
        /// チュートリアルイントロの演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator Tutorial_IntroCoroutine()
        {
            Time.timeScale = 0;
            audioSetting.PlayBGM(tutorial_BgmIndex);
            yield return new WaitForSecondsRealtime(tutorial_IntroTime);
            tutorial_UI_Manager.ShowFirstPage();// 最初のページを表示
            nextButton.enabled = true;
        }

        /// <summary>
        /// チュートリアルアウトロの演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        public IEnumerator Tutorial_OutroCoroutine()
        {
            tutorial_UI_Manager.HidePages();// ページUIだけ消す
            animator.SetTrigger(tutorial_EndTrigger);// チュートリアル終了演出
            yield return new WaitForSecondsRealtime(tutorial_OutroTime);// イントロ演出中は待機
            tutorial_UI_Manager.Hide();// UIを全部消す
            StartCoroutine(Stage_IntroCoroutine());// ステージイントロ開始
        }

        /// <summary>
        /// ステージイントロの演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator Stage_IntroCoroutine()
        {
            Time.timeScale = timeCanMoveValue;// 時を動かす
            audioSetting.PlayBGM(stageBgmIndex);

            // もしステージが第三ステージなら
            if (stageNumber == stageNumberMax)
            {
                audioSetting.PlayBGS(fireSeIndex);// ステージのBGSを再生
            }

            yield return new WaitForSeconds(stage_IntroTime);// ステージイントロ演出中は待機
            transitionUI_Manager.Hide();// 演出用UIを非表示にする
            sceneState = SceneState.Play;
        }

        /// <summary>
        /// ステージを出る関数
        /// </summary>
        public void ExitStage()
        {
            isTutorial = false;// チュートリアルフラグをリセット
            PauseTimeControl(timeCanMoveValue);// 時を動かす
            TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
            SceneManager.LoadScene(titleSceneName);
        }

        /// <summary>
        /// 時間をポーズする関数
        /// </summary>
        /// <param name="scale"></param>
        private void PauseTimeControl(float scale)
        {
            // もしチュートリアル中の場合
            if (isTutorial)
            {
                return;
            }

            Time.timeScale = scale;// 時間を止めるか動かす
        }
    }
}