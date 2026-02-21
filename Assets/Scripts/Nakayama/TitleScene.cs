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
        /// 初期設定の関数
        /// </summary>
        void Start()
        {
            audioSetting.PlayBGM(0);// タイトルBGMを再生
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
        public void InStageScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage");
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
            Debug.Log("ゲームを終了します。");
            Application.Quit();
        }
    }
}