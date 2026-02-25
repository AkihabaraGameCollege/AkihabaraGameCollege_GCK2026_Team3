using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace CardDefenseGame
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
        private static readonly int returnSettingTrigger = Animator.StringToHash("ReturnSetting");
        /// <summary>
        /// ゲーム終了するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int exitTrigger = Animator.StringToHash("Exit");

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        void Start()
        {
            audioSetting.PlayBGM(0);
            audioSetting.StartFadeIn(fadeTime);// タイトルBGMをフェードインさせるコルーチンを開始
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
        public void ReturnSetting()
        {
            animator.SetTrigger(returnSettingTrigger);// 設定画面を表示させるトリガーをセット
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
    }
}