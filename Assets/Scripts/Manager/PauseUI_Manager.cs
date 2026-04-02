using System.Collections;
using UnityEngine;

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
            animator = GetComponent<Animator>();
            Hide();// 起動時はUIを隠す
        }

        /// <summary>         
        /// UIを表示する関数         
        /// </summary>         
        private void Show()
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
        private void Hide()
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
        public IEnumerator PauseAnimationCoroutine(bool isPause)
        {
            if (isPause)
            {
                Show();
                animator.SetTrigger(onPauseTrigger);// ポーズのトリガーをセット
            }
            else
            {
                animator.SetTrigger(onPauseRemoveTrigger);// ポーズ解除のトリガーをセット
            }

            yield return new WaitForSecondsRealtime(pauseAnimationTime);// アニメーションが終わるまで待機

            if (!isPause)
            {
                Hide();
            }
        }
    }
}
