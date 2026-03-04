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
        /// デッキ表示のボタンの変数
        /// </summary>
        [SerializeField]
        private Button deckButton = null;
        /// <summary>
        /// デッキ表示から戻るボタンの変数
        /// </summary>
        [SerializeField]
        private Button deckReturnButton = null;
        /// <summary>
        /// 設定画面から戻るボタンの変数
        /// </summary>
        [SerializeField]
        private Button settingReturnButton = null;
        /// <summary>
        /// ステージセレクトから戻るボタンの変数
        /// </summary>
        [SerializeField]
        private Button stageSelectReturnButton = null;
        /// <summary>
        /// 第一ステージへのボタンの変数
        /// </summary>
        [SerializeField]
        private Button stageButton1 = null;
        /// <summary>
        /// 第二ステージへのボタンの変数
        /// </summary>
        [SerializeField]
        private Button stageButton2 = null;
        /// <summary>
        /// 第三ステージへのボタンの変数
        /// </summary>
        [SerializeField]
        private Button stageButton3 = null;

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
        /// 設定ボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        private static readonly int settingTrigger = Animator.StringToHash("Setting");
        /// <summary>
        /// 設定画面から戻るときに呼ばれるIDの変数
        /// </summary>
        private static readonly int settingReturnTrigger = Animator.StringToHash("SettingReturn");
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
        /// 初期設定の関数
        /// </summary>
        void Start()
        {
            // タイトルBGMを再生
            audioSetting.PlayBGM(0);
            audioSetting.StartFadeIn(fadeTime);// タイトルBGMをフェードインさせるコルーチンを開始

            // ボタンに関数を登録
            deckButton.onClick.AddListener(DisplayDeck);// デッキ表示のボタンにデッキ表示の関数を登録
            deckReturnButton.onClick.AddListener(DeckReturn);// デッキから戻るボタンにデッキから戻る関数を登録
            settingReturnButton.onClick.AddListener(SettingReturn);// 設定画面から戻るボタンに設定画面から戻る関数を登録
            stageSelectReturnButton.onClick.AddListener(StageSelectReturn);// ステージセレクトから戻るボタンにステージセレクトから戻る関数を登録
            stageButton1.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(0)));// 第一ステージへのボタンに第一ステージへの関数を登録
            stageButton2.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(1)));// 第二ステージへのボタンに第二ステージへの関数を登録
            stageButton3.onClick.AddListener(() => StartCoroutine(GoStageCoroutine(2)));// 第三ステージへのボタンに第三ステージへの関数を登録
        }

        /// <summary>
        /// ステージセレクト画面を表示する関数
        /// </summary>
        public void DisplayStageSelect()
        {
            animator.SetTrigger(startTrigger);// ステージセレクトを表示させるトリガーをセット
        }

        /// <summary>
        /// 設定画面を表示する関数
        /// </summary>
        public void DisplaySetting()
        {
            animator.SetTrigger(settingTrigger);// 設定画面を表示させるトリガーをセット
        }

        /// <summary>
        /// 設定画面から戻る関数
        /// </summary>
        public void SettingReturn()
        {
            animator.SetTrigger(settingReturnTrigger);// 設定画面から戻るトリガーをセット
        }

        /// <summary>
        /// ゲームを終了する関数
        /// </summary>
        public void ExitGame()
        {
            StartCoroutine(ExitGameCoroutine());
        }

        /// <summary>
        /// ゲームを終了するコルーチン
        /// </summary>
        /// <returns></returns>
        IEnumerator ExitGameCoroutine()
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
        }

        /// <summary>
        /// ステージシーンへ遷移するコルーチン
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private IEnumerator GoStageCoroutine(int number)
        {
            panel_SpriteIndex = number;// パネルスプライトのインデックスにステージの番号を代入

            // 左右のパネルのスプライトをステージに合わせて変更
            Panel_Image.sprite = Panel_Sprite[panel_SpriteIndex];// 左パネルのスプライトを変更

            stageSceneIndex = number;// ステージセレクトで選択されたステージのインデックスを取得
            animator.SetTrigger(goStageTrigger);// ステージへ遷移するトリガーをセット
            yield return new WaitForSeconds(GoStageTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(stageSceneNames[stageSceneIndex]);// 指定の番号のステージシーンへ遷移
        }
    }
}