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
        Animator animator = null;

        /// <summary>
        /// スタートボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        static readonly int StartTrigger = Animator.StringToHash("Start");
        /// <summary>
        /// 設定ボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        static readonly int SettingTrigger = Animator.StringToHash("Setting");

        /// <summary>
        /// ステージセレクト画面を表示する関数
        /// </summary>
        public void DisplayStageSelect()
        {
            animator.SetTrigger(StartTrigger);// ステージセレクトを表示させるトリガーをセット
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
            animator.SetTrigger(SettingTrigger);// 設定画面を表示させるトリガーをセット
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