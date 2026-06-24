using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// タイトルシーンの管理クラス
    /// </summary>
    public class TitleScene : MonoBehaviour
    {
        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;
        /// <summary>
        /// 通知UI用アニメーターを参照する変数
        /// </summary>
        public Animator noticeAnimator;

        /// <summary>
        /// AudioSettingコンポーネントの変数
        /// </summary>
        [SerializeField]
        private AudioSetting audioSetting = null;

        /// <summary>
        /// ステージシーンへ遷移するときのシーン名のリスト変数
        /// </summary>
        [SerializeField]
        private string[] stageSceneNames = null;

        /// <summary>
        /// パネルのイメージの変数
        /// </summary>
        public Image panel_Image;

        /// <summary>
        /// パネルのスプライトのリスト変数
        /// </summary>
        public Sprite[] Panel_Sprite = null;

        /// <summary>
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;
        /// <summary>
        /// 設定機能クラスを参照する変数
        /// </summary>
        public SettingUI_Manager settingUI_Manager;
        /// <summary>
        /// デッキ管理クラスを参照する変数
        /// </summary>
        private DeckManager deckManager;
        /// <summary>
        /// 通知UI管理クラスを参照する変数
        /// </summary>
        private NoticeTextUI noticeTextUI;
        /// <summary>
        /// ポーズ機能クラスを参照する変数
        /// </summary>
        public PauseUI_Manager pauseUI_Manager;
        /// <summary>
        /// タイトル管理クラスのインスタンスを参照する変数
        /// </summary>
        public static TitleScene Instance { get; private set; }

        /// <summary>
        ///スタートボタンの変数
        /// </summary>
        private Button startButton = null;
        /// <summary>
        /// ゲーム終了ボタンの変数
        /// </summary>
        public Button exitButton;
        /// <summary>
        /// デッキ表示のボタンの変数
        /// </summary>
        public Button deckButton = null;
        /// <summary>
        /// デッキ表示から戻るボタンの変数
        /// </summary>
        public Button deckReturnButton = null;
        /// <summary>
        /// ステージセレクトから戻るボタンの変数
        /// </summary>
        public Button stageSelectReturnButton = null;
        /// <summary>
        /// 第一ステージへのボタンの変数
        /// </summary>
        public Button stageButton1 = null;
        /// <summary>
        /// 第二ステージへのボタンの変数
        /// </summary>
        public Button stageButton2 = null;
        /// <summary>
        /// 第三ステージへのボタンの変数
        /// </summary>
        public Button stageButton3 = null;
        /// <summary>
        /// 設定へのボタンの変数
        /// </summary>
        public Button settingButton;

        /// <summary>
        /// 演出用UIの木のスプライトオブジェクトを参照する変数
        /// </summary>
        public GameObject treesObject;
        /// <summary>
        /// 演出用UIのフェードスプライトオブジェクトを参照する変数
        /// </summary>
        public GameObject black_ImageObject;
        /// <summary>
        /// 演出用UIの木製パネルスプライトオブジェクトを参照する変数
        /// </summary>
        public GameObject treePanel_Object;
        /// <summary>
        /// デッキに関する通知UIオブジェクトを参照する変数
        /// </summary>
        public GameObject deckNotice;

        /// <summary>
        /// スタートボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        private static readonly int startTrigger = Animator.StringToHash("Start");
        /// <summary>
        /// ゲーム終了するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int exitTrigger = Animator.StringToHash("Exit");
        /// <summary>
        /// デッキ表示するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int deckTrigger = Animator.StringToHash("Deck");
        /// <summary>
        /// デッキから戻るときに呼ばれるIDの変数
        /// </summary>
        private static readonly int deckReturnTrigger = Animator.StringToHash("DeckReturn");
        /// <summary>
        /// ステージセレクトから戻るときに呼ばれるIDの変数
        /// </summary>
        private static readonly int stageSelectReturnTrigger = Animator.StringToHash("StageSelectReturn");
        /// <summary>
        /// ステージへ遷移するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int goStageTrigger = Animator.StringToHash("GoStage");
        /// <summary>
        /// デッキが満タンではないときに呼ばれるIDの変数
        /// </summary>
        private static readonly int deckNonFullTrigger = Animator.StringToHash("DeckNonFull");

        /// <summary>
        /// イントロアニメーション中の待機時間
        /// </summary>
        private float introTime = 2.5f;
        /// <summary>
        /// デッキが動いている最中の待機時間
        /// </summary>
        private float deckMoveTime = 1.25f;
        /// <summary>
        /// デッキが開いている最中の待機時間
        /// </summary>
        private float deckOpenTime = 1.25f;
        /// <summary>
        /// デッキが満タンではないことを通知するアニメーションの時間
        /// </summary>
        private float noticeTime = 1.0f;
        /// <summary>
        /// ゲーム終了までの待機時間
        /// </summary>
        private float exitTime = 3.0f;
        /// <summary>
        /// フェードアウトの時間
        /// </summary>
        public float fadeTime = 1.0f;
        /// <summary>
        /// ステージへ遷移するまでの待機時間
        /// </summary>
        public float GoStageTime = 2.0f;

        /// <summary>
        /// 通知UI管理クラスのオブジェクト名を参照する変数
        /// </summary>
        private string noticeTextUI_Name = "NoticeTextUI";
        /// <summary>
        /// デッキ管理クラスのオブジェクト名を参照する変数
        /// </summary>
        private string deckManagerName = "DeckManager";
        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        public string playerRootName = "PlayerRootTitle";
        /// <summary>
        /// 設定画面管理クラスのオブジェクト名を参照する変数
        /// </summary>
        public string settingUI_Name = "SettingUI";
        /// <summary>
        /// スタートボタンのオブジェクト名を参照する変数
        /// </summary>
        public string startButtonName = "StartButton";
        /// <summary>
        /// ステージセレクトUIのオブジェクト名を参照する変数
        /// </summary>
        public string stageSelectUI_Name = "StageSelectUI";
        /// <summary>
        /// デッキUIのオブジェクト名を参照する変数
        /// </summary>
        public string deckUI_Name = "DeckUI";

        /// <summary>
        /// シーン最初の画面にいるかどうか判別する変数
        /// </summary>
        public static bool isStartScene = true;
        /// <summary>
        /// 別シーンからタイトルへ遷移したかを判別する変数
        /// </summary>
        public static bool isExit = false;

        /// <summary>
        /// タイトルでBGMの何番を流すかのインデックスを参照する変数
        /// </summary>
        private int titleBgmIndex = 0;
        /// <summary>
        /// デッキ編集画面でBGMの何番を流すかのインデックスを参照する変数
        /// </summary>
        private int deckBgmIndex = 1;
        /// <summary>
        /// タイトルの環境SEの何番を流すかのインデックスを参照する変数
        /// </summary>
        private int titleDirectionSeIndex = 1;
        /// <summary>
        /// デッキ内のカードが足りていないSEの何番を流すかのインデックスを参照する変数
        /// </summary>
        private int notCardSeIndex = 9;
        /// <summary>
        /// デッキが開くときにSEの何番を流すかのインデックスを参照する変数
        /// </summary>
        private int deckOpenSeIndex = 2;
        /// <summary>
        /// 第何ステージかのインデックスを参照する変数
        /// </summary>
        public int stageSceneIndex = 0;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
                Instance = this;

            // コンポーネントの登録
            animator = GetComponent<Animator>();
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
            startButton = GameObject.Find(startButtonName).GetComponent<Button>();// シーン内からスタートボタンを探して取得
            noticeTextUI = GameObject.Find(noticeTextUI_Name).GetComponent<NoticeTextUI>();// シーン内から通知UI管理クラスを探して取得
            deckManager = GameObject.Find(deckManagerName).GetComponent<DeckManager>();// シーン内からデッキ管理クラスを探して取得

            // ボタンに関数を登録
            deckReturnButton.onClick.AddListener(DeckReturn);// デッキから戻るボタンにデッキから戻る関数を登録
            stageSelectReturnButton.onClick.AddListener(StageSelectReturn);// ステージセレクトから戻るボタンにステージセレクトから戻る関数を登録
            startButton.onClick.AddListener(DisplayStageSelect);// スタートボタンにステージセレクトへ行く関数を登録
            settingButton.onClick.AddListener(settingUI_Manager.DisplaySetting);// 設定画面表示のボタンに設定画面表示のコルーチンを登録
            stageButton1.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(0)));// 第一ステージへのボタンに第一ステージへの関数を登録
            stageButton2.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(1)));// 第二ステージへのボタンに第二ステージへの関数を登録
            stageButton3.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(2)));// 第三ステージへのボタンに第三ステージへの関数を登録
            exitButton.onClick.AddListener(GameExit);// 第三ステージへのボタンに第三ステージへの関数を登録
            deckButton.onClick.AddListener(() => StartCoroutine(DisplayDeckCoroutine()));// デッキ表示のボタンにデッキ表示の関数を登録
        }

        /// <summary>
        /// タイトルシーン開始時の準備を行う関数
        /// </summary>
        private void Start()
        {
            isStartScene = true;// 最初のシーン

            TransitionUI_Manager.instance.Hide();

            // タイトルBGMを再生
            audioSetting.PlayBGM(titleBgmIndex);
            audioSetting.PlayBGS(titleDirectionSeIndex);// タイトルシーンのBGSを再生

            // 別シーンからタイトルに来た場合
            if (isExit)
            {
                StartCoroutine(DisplayStageSelectCoroutine());// それ専用のステージセレクト表示イベントを呼び出し
            }
            else
            {
                StartCoroutine(title_IntroCoroutine());// イントロイベントを呼び出し
            }
        }

        /// <summary>
        /// ステージセレクト画面を表示する関数
        /// </summary>
        public void DisplayStageSelect()
        {
            animator.SetTrigger(startTrigger);// ステージセレクトを表示させるトリガーをセット

            // フラグ操作
            playerController.isCanPause = true;// ポーズ操作を許可する
            isStartScene = false;// 最初のシーンから出る
        }

        /// <summary>
        /// ゲームを終了するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExitCoroutine()
        {
            TransitionUI_Manager.instance.TargetShow(treesObject);// 演出用UIを表示
            TransitionUI_Manager.instance.TargetShow(black_ImageObject);// 演出用UIを表示
           animator.SetTrigger(exitTrigger);// ゲーム終了のトリガーをセット
            yield return new WaitForSeconds(exitTime);
            Debug.Log("ゲームを終了します。");
            Application.Quit();
        }

        /// <summary>
        /// デッキ画面から戻る関数
        /// </summary>
        public void DeckReturn()
        {
            audioSetting.PlayBGM(titleBgmIndex);
            animator.SetTrigger(deckReturnTrigger);// デッキ画面から戻るトリガーをセット
        }

        /// <summary>
        /// ステージセレクト画面から戻る関数
        /// </summary>
        public void StageSelectReturn()
        {
            animator.SetTrigger(stageSelectReturnTrigger);// ステージセレクト画面から戻るトリガーをセット

            // フラグ操作
            playerController.isCanPause = false;// ポーズ操作を許可しない
            isStartScene = true;// 最初のシーンに戻る
        }

        /// <summary>
        /// ステージシーンへ遷移するコルーチン
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private IEnumerator GoStageCoroutine(int number = 0)
        {
            if (deckManager.isDeckFull)
            {
                stageSceneIndex = number;// パネルスプライトのインデックスにステージの番号を代入

                panel_Image.sprite = Panel_Sprite[stageSceneIndex];// パネルのスプライトをステージに合わせて変更

                stageSceneIndex = number;// ステージセレクトで選択されたステージのインデックスを取得
                TransitionUI_Manager.instance.TargetShow(treePanel_Object);// 演出用UIの木製パネルを表示
                animator.SetTrigger(goStageTrigger);// ステージへ遷移するトリガーをセット
                yield return new WaitForSeconds(GoStageTime);
                UnityEngine.SceneManagement.SceneManager.LoadScene(stageSceneNames[stageSceneIndex]);// 指定の番号のステージシーンへ遷移
            }
            else
            {
                audioSetting.PlaySE(notCardSeIndex);// デッキが満タンではないときのSEを再生
                noticeTextUI.TargetShow(deckNotice);// 通知UIを表示
                noticeAnimator.SetTrigger(deckNonFullTrigger);// デッキが満タンではないことをプレイヤーに伝えるアニメーション
                yield return new WaitForSeconds(noticeTime);// アニメーション分待機
                noticeTextUI.Hide();// 通知UIを非表示
            }
        }

        /// <summary>
        /// 強制的にステージセレクトへ遷移するコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplayStageSelectCoroutine()
        {
            TransitionUI_Manager.instance.TargetShow(treesObject);// 演出用UIを表示
            TransitionUI_Manager.instance.TargetShow(black_ImageObject);// 演出用UIを表示
            yield return new WaitForSeconds(introTime);// イントロ中は待つ
            TransitionUI_Manager.instance.Hide();// 演出用UIを非表示
            DisplayStageSelect();
            isExit = false;// フラグをリセット
            isStartScene = false;// 最初のシーンから出る
        }

        /// <summary>
        /// デッキ編集画面に移る演出用コルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplayDeckCoroutine()
        {
            animator.SetTrigger(deckTrigger);// デッキ画面を表示させるトリガーをセット
            yield return new WaitForSeconds(deckMoveTime);// デッキが動いている時間は待つ
            audioSetting.PlaySE(deckOpenSeIndex);
            yield return new WaitForSeconds(deckOpenTime);// デッキが開いている時間は待つ
            audioSetting.StopBGS();
            audioSetting.PlayBGM(deckBgmIndex);
        }

        /// <summary>
        /// タイトルイントロ演出のコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator title_IntroCoroutine()
        {
            TransitionUI_Manager.instance.TargetShow(treesObject);// 演出用UIを表示
            TransitionUI_Manager.instance.TargetShow(black_ImageObject);// 演出用UIを表示
            yield return new WaitForSeconds(introTime);// イントロ中は待つ
            TransitionUI_Manager.instance.Hide();// 演出用UIを非表示
        }

        /// <summary>
        /// ゲームを終了する関数
        /// </summary>
        private void GameExit()
        {
            // ゲーム終了処理コルーチンを実行
            StartCoroutine(ExitCoroutine());
        }
    }
}