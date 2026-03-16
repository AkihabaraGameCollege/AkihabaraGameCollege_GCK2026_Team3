using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ForestDraw
{
    /// <summary>     
    /// ポーズの管理を行うクラス     
    /// </summary>     
    public class PauseManager : MonoBehaviour
    {
        /// <summary>         
        /// 戻るボタンの変数         
        /// </summary>         
        [SerializeField]
        private Button resumeButton = null;
        /// <summary>         
        /// 設定ボタンの変数         
        /// </summary>         
        [SerializeField]
        private Button settingButton = null;
        /// <summary>         
        /// ステージセレクトボタンの変数         
        /// </summary>         
        [SerializeField]
        private Button exitButton = null;

        /// <summary>         
        /// ResumeButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onResumeButtonClick = null;
        /// <summary>         
        /// SettingButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onSettingButtonClick = null;
        /// <summary>         
        /// ExitButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onExitButtonClick = null;

        /// <summary>
        /// タイトル管理クラスの参照変数
        /// </summary>
        private TitleScene titleScene;

        /// <summary>         
        /// 初期設定を行う関数         
        /// </summary>         
        void Awake()
        {
            // UnityEvent を追加
            resumeButton.onClick.AddListener(() => { onResumeButtonClick.Invoke(); });// 戻るボタンのイベントを設定
            settingButton.onClick.AddListener(() => { onSettingButtonClick.Invoke(); });// 設定ボタンのイベントを設定
            exitButton.onClick.AddListener(() => { onExitButtonClick.Invoke(); });// ステージセレクトボタンのイベントを設定
            Hide();// 起動時はUIを隠す
        }

        /// <summary>
        /// ポーズの入力判定を行う関数
        /// </summary>
        /// <param name="context"></param>
        public void OnPause(InputAction.CallbackContext context)
        {
            // もし入力された場合
            if (context.started)
            {
                Show();// ポーズの機能を起動する
            }
        }

        /// <summary>         
        /// ポーズを行う関数         
        /// </summary>         
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        
            Time.timeScale = 0;
        }

        /// <summary>         
        /// ポーズをやめる関数         
        /// </summary>         
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// ステージを出る関数
        /// </summary>
        public void ExitStage()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Title");
        }
    }
}