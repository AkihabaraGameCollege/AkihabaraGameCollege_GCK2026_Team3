using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>     
    /// ポーズUIの管理を行うクラス     
    /// </summary>     
    public class PauseUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator animator;

        /// <summary>         
        /// ResumeButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onResumeButtonClick;
        /// <summary>         
        /// SettingButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onSettingButtonClick;
        /// <summary>         
        /// ExitButtonが押されたときに発生するUnityEventの変数         
        /// </summary>         
        public UnityEvent onExitButtonClick;

        /// <summary>         
        /// 戻るボタンの変数         
        /// </summary>         
        public Button resumeButton;
        /// <summary>         
        /// 設定ボタンの変数         
        /// </summary>         
        public Button settingButton;
        /// <summary>         
        /// ステージセレクトボタンの変数         
        /// </summary>         
        public Button exitButton;
        /// <summary>
        /// ポーズ管理クラスのインスタンスを参照する変数
        /// </summary>
        public static PauseUI_Manager Instance { get; private set; }

        /// <summary>
        /// ポーズをするときに呼ばれるIDの変数
        /// </summary>
        private static readonly int onPauseTrigger = Animator.StringToHash("OnPause");
        /// <summary>
        /// ポーズを解除するときに呼ばれるIDの変数
        /// </summary>
        private static readonly int onPauseRemoveTrigger = Animator.StringToHash("OnRemovePause");

        /// <summary>
        /// ポーズのアニメーション時間を参照する変数
        /// </summary>
        private float pauseAnimationTime = 1f;

        /// <summary>         
        /// 初期設定を行う関数         
        /// </summary>         
        private void Awake()
        {
            Instance = this;// シングルトンのインスタンスを設定
            animator = GetComponent<Animator>();

            // UnityEvent を追加
            resumeButton.onClick.AddListener(() => { onResumeButtonClick.Invoke(); });// 戻るボタンのイベントを設定
            settingButton.onClick.AddListener(() => { onSettingButtonClick.Invoke(); });// 設定ボタンのイベントを設定
            exitButton.onClick.AddListener(() => { onExitButtonClick.Invoke(); });// ステージセレクトボタンのイベントを設定

            Hide();// 起動時はUIを隠す
        }

        /// <summary>         
        /// UIを表示する関数         
        /// </summary>         
        public void Show()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        /// <summary>         
        /// UIを隠す関数         
        /// </summary>         
        public void Hide()
        {
            // 子オブジェクトをすべて非アクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ポーズのアニメーションを再生するためのコルーチン
        /// </summary>
        /// <param name="isPause"></param>
        /// <returns></returns>
        private IEnumerator PauseAnimationCoroutine(bool isPause)
        {
            if (isPause)
            {
                Show();
                PauseTimeControl(0f);// 時を止める
                animator.SetTrigger(onPauseTrigger);// ポーズのトリガーをセット
            }
            else if (!isPause)
            {
                PauseTimeControl(StageScene.Instance.timeCanMoveValue);// 時を動かす
                animator.SetTrigger(onPauseRemoveTrigger);// ポーズ解除のトリガーをセット
            }

            yield return new WaitForSecondsRealtime(pauseAnimationTime);// アニメーションが終わるまで待機

            if (!isPause)
            {
                Hide();
            }
        }

        /// <summary>
        /// ポーズのアニメーションを開始する関数
        /// </summary>
        /// <param name="isPause"></param>
        public void StartPause(bool isPause)
        {
            StartCoroutine(PauseAnimationCoroutine(isPause));
        }

        /// <summary>
        /// ステージを出る関数
        /// </summary>
        public void ExitStage()
        {
            StageScene.Instance.isTutorial = false;// チュートリアルフラグをリセット
            PauseTimeControl(StageScene.Instance.timeCanMoveValue);// 時を動かす
            TitleScene.isExit = true;// 別シーンからタイトルへ行ったフラグをオン
            SceneManager.LoadScene(StageScene.Instance.titleSceneName);
        }

        /// <summary>
        /// 時間をポーズする関数
        /// </summary>
        /// <param name="scale"></param>
        private void PauseTimeControl(float scale)
        {
            // もしチュートリアル中の場合
            if (StageScene.Instance.isTutorial)
            {
                return;
            }

            Time.timeScale = scale;// 時間を止めるか動かす
        }
    }
}
