using System.Collections;
using System.Runtime.CompilerServices;
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
        /// アニメーターコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Animator animator = null;

        /// <summary>
        /// AudioSettingコンポーネントの変数
        /// </summary>
        [SerializeField]
        private AudioSetting audioSetting = null;

        /// <summary>
        /// ゲーム終了までの待機時間
        /// </summary>
        [SerializeField]
        private float exitTime = 1.0f;
        /// <summary>
        /// フェードアウトの時間
        /// </summary>
        [SerializeField]
        private float fadeTime = 1.0f;
        /// <summary>
        /// ステージへ遷移するまでの待機時間
        /// </summary>
        [SerializeField]
        private float GoStageTime = 2.0f;

        /// <summary>
        /// ステージシーンへ遷移するときのシーン名のリスト変数
        /// </summary>
        [SerializeField]
        private string[] stageSceneNames = null;

        /// <summary>
        /// インデックスの変数
        /// </summary>
        [SerializeField]
        private int stageSceneIndex = 0;
        /// <summary>
        /// パネルスプライトのインデックスの変数
        /// </summary>
        [SerializeField]
        private int panel_SpriteIndex = 0;

        /// <summary>
        /// パネルのイメージの変数
        /// </summary>
        [SerializeField]
        private Image Panel_Image = null;

        /// <summary>
        /// パネルのスプライトのリスト変数
        /// </summary>
        [SerializeField]
        private Sprite[] Panel_Sprite = null;

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
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;
        /// <summary>
        /// ポーズ機能クラスを参照する変数
        /// </summary>
        private PauseManager pauseManager = null;
        /// <summary>
        /// 設定機能クラスを参照する変数
        /// </summary>
        public SettingManager settingManager = null;

        /// <summary>
        /// イントロアニメーション中の待機時間
        /// </summary>
        public float introTime = 1f;

        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        public string playerRootName = "PlayerRootTitle";
        /// <summary>
        /// ポーズUIのオブジェクト名を参照する変数
        /// </summary>
        public string pauseUI_Name = "PauseUI";
        /// <summary>
        /// 設定画面管理クラスのオブジェクト名を参照する変数
        /// </summary>
        public string settingUI_Name = "SettingUI";
        /// <summary>
        /// スタートボタンのオブジェクト名を参照する変数
        /// </summary>
        public string startButtonName = "StartButton";
        /// <summary>
        /// ゲーム終了ボタンのオブジェクト名を参照する変数
        /// </summary>
        public string exitButtonName = "ExitButton";
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
        public bool isStartScene = true;
        /// <summary>
        /// 別シーンからタイトルへ遷移したかを判別する変数
        /// </summary>
        public static bool isExit = false;

        /// <summary>
        ///スタートボタンの変数
        /// </summary>
        private Button startButton = null;
        /// <summary>
        /// ゲーム終了ボタンの変数
        /// </summary>
        private Button exitButton = null;
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
        public Button settingButton = null;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            // コンポーネントの登録
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
            pauseManager = GameObject.Find(pauseUI_Name).GetComponent<PauseManager>();// シーン内からポーズUIを探して取得
            startButton = GameObject.Find(startButtonName).GetComponent<Button>();// シーン内からスタートボタンを探して取得
            exitButton = GameObject.Find(exitButtonName).GetComponent<Button>();// シーン内からゲーム終了ボタンを探して取得

            // ボタンに関数を登録
            deckButton.onClick.AddListener(DisplayDeck);// デッキ表示のボタンにデッキ表示の関数を登録
            deckReturnButton.onClick.AddListener(DeckReturn);// デッキから戻るボタンにデッキから戻る関数を登録
            stageSelectReturnButton.onClick.AddListener(StageSelectReturn);// ステージセレクトから戻るボタンにステージセレクトから戻る関数を登録
            startButton.onClick.AddListener(DisplayStageSelect);// スタートボタンにステージセレクトへ行く関数を登録
            settingButton.onClick.AddListener(settingManager.DisplaySetting);// デッキ表示のボタンにデッキ表示の関数を登録
            stageButton1.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(0)));// 第一ステージへのボタンに第一ステージへの関数を登録
            stageButton2.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(1)));// 第二ステージへのボタンに第二ステージへの関数を登録
            stageButton3.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(2)));// 第三ステージへのボタンに第三ステージへの関数を登録
            exitButton.onClick.AddListener(() => StartCoroutine(ExitCoroutine()));// 第三ステージへのボタンに第三ステージへの関数を登録

            // タイトルBGMを再生
            audioSetting.PlayBGM(0);
            audioSetting.StartFadeIn(fadeTime);// タイトルBGMをフェードインさせるコルーチンを開始

            // 別シーンからタイトルに来た場合
            if (isExit)
            {
                StartCoroutine(DisplayStageSelectCoroutine());// それ専用のステージセレクト表示イベントを呼び出し
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
            animator.SetTrigger(exitTrigger);// ゲーム終了のトリガーをセット
            audioSetting.StopFadeOut(fadeTime);// BGMをフェードアウトさせるコルーチンを開始
            yield return new WaitForSeconds(exitTime);
            Debug.Log("ゲームを終了します。");
            Application.Quit();
        }

        /// <summary>
        /// デッキ画面を表示する関数
        /// </summary>
        public void DisplayDeck()
        {
            animator.SetTrigger(deckTrigger);// デッキ画面を表示させるトリガーをセット
        }

        /// <summary>
        /// デッキ画面から戻る関数
        /// </summary>
        public void DeckReturn()
        {
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
            panel_SpriteIndex = number;// パネルスプライトのインデックスにステージの番号を代入

            Panel_Image.sprite = Panel_Sprite[panel_SpriteIndex];// パネルのスプライトをステージに合わせて変更

            stageSceneIndex = number;// ステージセレクトで選択されたステージのインデックスを取得
            animator.SetTrigger(goStageTrigger);// ステージへ遷移するトリガーをセット
            yield return new WaitForSeconds(GoStageTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(stageSceneNames[stageSceneIndex]);// 指定の番号のステージシーンへ遷移
        }

        private IEnumerator DisplayStageSelectCoroutine()
        {
            yield return new WaitForSeconds(introTime);// イントロ中は待つ
            DisplayStageSelect();
            isExit = false;// フラグをリセット
        }
    }
}