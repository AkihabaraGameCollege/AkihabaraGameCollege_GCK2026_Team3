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
        /// 初期設定の関数
        /// </summary>
        void Start()
        {
            audioSetting.PlayBGM(0);
            audioSetting.StartFadeIn(fadeTime);// タイトルBGMをフェードインさせるコルーチンを開始

            // ボタンに関数を登録
            deckButton.onClick.AddListener(DisplayDeck);// デッキ表示のボタンにデッキ表示の関数を登録
            deckReturnButton.onClick.AddListener(DeckReturn);// デッキから戻るボタンにデッキから戻る関数を登録
            settingReturnButton.onClick.AddListener(SettingReturn);// 設定画面から戻るボタンに設定画面から戻る関数を登録
        }

        /// <summary>
        /// ステージセレクト画面を表示する関数
        /// </summary>
        public void DisplayStageSelect()
        {
            animator.SetTrigger(startTrigger);// ステージセレクトを表示させるトリガーをセット
        }

        /// <summary>
        /// ステージシーンへ遷移する関数
        /// </summary>
        public void InStageScene(int number)
        {
            stageSceneIndex = number;// ステージセレクトで選択されたステージのインデックスを取得
            UnityEngine.SceneManagement.SceneManager.LoadScene(stageSceneNames[stageSceneIndex]);// 指定の番号のステージシーンへ遷移
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
    }
}