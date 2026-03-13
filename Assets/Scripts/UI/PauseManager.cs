using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// ポーズUI
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
        /// UIを見せる関数
        /// </summary>
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        // このUIを非表示に設定します。
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}