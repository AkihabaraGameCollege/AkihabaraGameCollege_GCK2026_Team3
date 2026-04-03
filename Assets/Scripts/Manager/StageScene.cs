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
        /// ステージクリアUIの変数
        /// </summary>
        [SerializeField]
        private GameObject stageClearUI = null;
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
        public Tutorial_UI_Manager tutorial_UI_Manager;
        /// <summary>
        /// 演出用UI管理クラスを参照する変数
        /// </summary>
        public TransitionUI_Manager transitionUI_Manager;
        /// <summary>
        /// ポーズ管理クラスを参照する変数
        /// </summary>
        public PauseUI_Manager pauseUI_Manager;
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
        /// 次へボタンを参照する変数
        /// </summary>
        public Button nextButton;

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
        /// スタート時に引くカードの枚数の変数
        /// </summary>
        private int startDrawCount = 4;
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
        /// チュートリアルを開始するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int tutorial_StartTrigger = Animator.StringToHash("Tutorial_Start");
        /// <summary>
        /// ステージイントロを開始するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int stage_IntroStartTrigger = Animator.StringToHash("Stage_IntroStart");
        /// <summary>
        /// ゲームオーバー演出を開始するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int gameOverTrigger = Animator.StringToHash("GameOver");
        /// <summary>
        /// ステージクリア演出を開始するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int stageClearTrigger = Animator.StringToHash("StageClear");

        /// <summary>
        /// クリアシーン名を参照する変数
        /// </summary>
        private string clearSceneName = "Clear";
        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        private string playerRootName = "PlayerRootStage";
        // <summary>
        /// ステージシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";

        /// <summary>
        /// チュートリアルのイントロ時間を参照する変数
        /// </summary>
        private float tutorial_IntroTime = 3.5f;
        /// <summary>
        /// チュートリアルのアウトロ時間を参照する変数
        /// </summary>
        private float tutorial_OutroTime = 0.9f;
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
        public bool isTutorial = false;

        [SerializeField]
        private Transform cardEffectSpawn;

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
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
                Instance = this;

            // コンポーネントの登録
            animator = GetComponent<Animator>();
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
        }

        /// <summary>
        /// ステージシーン開始時の準備を行う関数
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

            // UIを最初は非表示にする
            gameOverUI.SetActive(false);
            stageClearUI.SetActive(false);
            TransitionUI_Manager.instance.Hide();

            // パネルのスプライトをステージに合わせて変更
            Panel_Image.sprite = Panel_Sprite[stagePanel_Index];// パネルのスプライトをステージに合わせて変更
            transitionUI_Manager.TargetShow(treePanel_UI);// パネルを表示

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

                int number = stageNumber;// 今いるステージ番号を参照

                StartCoroutine(StageClearCoroutine(number));// ステージクリア演出開始
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

                StartCoroutine(GameOverCoroutine());// ゲームオーバー演出開始
            }
        }

        /// <summary>
        /// イントロ中の演出を行う関数
        /// </summary>
        /// <param name="number"></param>
        private void Stage_Intro(int number)
        {
            // もし第一ステージなら
            if (number == stageNumberStart)
            {
                animator.SetTrigger(tutorial_StartTrigger);// チュートリアル開始演出
                StartCoroutine(Tutorial_IntroCoroutine());// チュートリアル開始
            }
            else 
            { 
                animator.SetTrigger(stage_IntroStartTrigger);// ステージイントロ開始演出
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
            isTutorial = true;// チュートリアル中フラグをオン
            nextButton.enabled = false;
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
            Time.timeScale = timeCanMoveValue;// 時を動かす
            isTutorial = false;// チュートリアル中フラグをオフ
            StartCoroutine(Stage_IntroCoroutine());// ステージイントロ開始
        }

        /// <summary>
        /// ステージイントロの演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator Stage_IntroCoroutine()
        {
            audioSetting.PlayBGM(stageBgmIndex);

            // もしステージが第三ステージなら
            if (stageNumber == stageNumberMax)
            {
                audioSetting.PlayBGS(fireSeIndex);// ステージのBGSを再生
            }

            yield return new WaitForSeconds(stage_IntroTime);// ステージイントロ演出中は待機
            transitionUI_Manager.Hide();// 演出用UIを非表示にする
            BattleCardManager.instance.DrawCards(startDrawCount);// ?枚引く
            sceneState = SceneState.Play;
        }

        /// <summary>
        /// ゲームオーバー演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        public IEnumerator GameOverCoroutine()
        {
            gameOverUI.SetActive(true);
            animator.SetTrigger(gameOverTrigger);// ゲームオーバー演出
            yield return new WaitForSeconds(2.5f);// 演出中は待機
            TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
            UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        }

        /// <summary>
        /// ステージクリア演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        public IEnumerator StageClearCoroutine(int number)
        {
            stageClearUI.SetActive(true);
            animator.SetTrigger(stageClearTrigger);// ステージクリア演出
            yield return new WaitForSeconds(2f);// 演出中は待機

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

        /// <summary>カード使用時のエフェクト再生用</summary>
        public void PlayCardEffect(GameObject effect, float time)
        {
            GameObject obj = Instantiate(effect, cardEffectSpawn.position, effect.transform.rotation);
            Destroy(obj, time);
        }
        public void PlayCardEffect(GameObject effect, Transform target, float time)
        {
            GameObject obj = Instantiate(effect, cardEffectSpawn.position, effect.transform.rotation);

            var projectile = obj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Init(target);
            }

            Destroy(obj, time);
        }
    }
}