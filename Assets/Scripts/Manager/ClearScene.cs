using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        private AudioSetting audioSetting = null;

        /// <summary>
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;

        /// <summary>
        /// タイトルへ行くボタンを参照する変数
        /// </summary>
        private Button titleButton = null;
        /// <summary>
        /// クリア演出をスキップするボタンを参照する変数
        /// </summary>
        private Button skipButton = null;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator = null;

        /// <summary>
        /// タイトルシーン名を参照する変数
        /// </summary>
        private string titleSceneName = "Title";
        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        private string playerRootName = "PlayerRootClear";
        /// <summary>
        /// オーディオ設定クラスのオブジェクト名を参照する変数
        /// </summary>
        private string audioSettingName = "AudioSettings";
        /// <summary>
        /// タイトルボタンのオブジェクト名を参照する変数
        /// </summary>
        private string titleButtonName = "TitleButton";
        /// <summary>
        /// スキップボタンのオブジェクト名を参照する変数
        /// </summary>
        private string skipButtonName = "SkipButton";

        /// <summary>
        /// クリアシーンで何番目のBGMを再生するかのインデックスを参照する変数
        /// </summary>
        private int bgmIndex = 6;
        /// <summary>
        /// クラッカーサウンドで何番目のSEを再生するかのインデックスを参照する変数
        /// </summary>
        private int clackerSeIndex = 4;
        /// <summary>
        /// 花火サウンドで何番目のSEを再生するかのインデックスを参照する変数
        /// </summary>
        private int fireworkSeIndex = 8;
        /// <summary>
        /// クリア演出をスキップするときに呼ばれるIDを参照する変数
        /// </summary>
        private static readonly int skipTrigger = Animator.StringToHash("EventSkip");

        /// <summary>
        /// 花火サウンド再生中の時間を参照する変数
        /// </summary>
        private float fireworkSeTime = 1f;

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            // コンポーネントの登録
            animator = GetComponent<Animator>();
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
            audioSetting = GameObject.Find(audioSettingName).GetComponent<AudioSetting>();// シーン内からオーディオ設定クラスを探して取得
            titleButton = GameObject.Find(titleButtonName).GetComponent<Button>();// シーン内からタイトルボタンを探して取得
            skipButton = GameObject.Find(skipButtonName).GetComponent<Button>();// シーン内からスキップボタンを探して取得

            // ボタンにイベントの登録
            titleButton.onClick.AddListener(InTitleScene);// タイトルボタンにタイトルへ戻る関数を登録
            skipButton.onClick.AddListener(EventSkip);// スキップボタンにクリア演出をスキップする関数を登録

            playerController.isCanPause = true;// ポーズ操作を許可する

            // サウンド再生
            audioSetting.PlaySE(clackerSeIndex);// シーン開始時に花火の音を再生
            audioSetting.PlayBGM(bgmIndex);

            StartCoroutine(clearIntroCoroutine());// クリア演出のイントロ部分のコルーチンを開始する
        }

        /// <summary>
        /// タイトルシーンへ遷移する関数
        /// </summary>
        private void InTitleScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        }

        /// <summary>
        /// クリア演出をスキップする関数
        /// </summary>
        private void EventSkip()
        {
            animator.SetTrigger(skipTrigger);// クリア演出をスキップする
        }

        /// <summary>
        /// クリアイントロ演出のコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator clearIntroCoroutine()
        {
            yield return new WaitForSeconds(fireworkSeTime);// 花火の音が鳴り終わるまで待つ
            audioSetting.PlaySE(fireworkSeIndex);// 花火の音を再生
        }
    }
}