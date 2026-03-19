using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// 設定画面を管理するクラス
    /// </summary>
    public class SettingManager : MonoBehaviour
    {
        /// <summary>
        /// アニメーターコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Animator animator = null;

        /// <summary>
        /// 設定画面から戻るボタンの変数
        /// </summary>
        [SerializeField]
        private Button settingReturnButton = null;
        /// <summary>
        /// 設定画面を表示するボタンの参照用変数
        /// </summary>
        [SerializeField]
        private Button settingButton = null;

        /// <summary>
        /// ポーズUIのオブジェクト参照変数
        /// </summary>
        [SerializeField]
        private GameObject pauseUI = null;

        [SerializeField]
        private float settingReturnTime = 1;

        /// <summary>
        /// 設定ボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        private static readonly int settingTrigger = Animator.StringToHash("Setting");
        /// <summary>
        /// 設定画面から戻るときに呼ばれるIDの変数
        /// </summary>
        private static readonly int settingReturnTrigger = Animator.StringToHash("SettingReturn");

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            Hide();// UIは消しておく
            settingReturnButton.onClick.AddListener(SettingReturn);// 設定画面から戻るボタンにその機能をつかさどる関数を登録
            settingButton.onClick.AddListener(DisplaySetting);// 設定画面を表示するボタンにその機能をつかさどる関数を登録
        }

        /// <summary>
        /// 設定画面を表示する関数
        /// </summary>
        public void DisplaySetting()
        {
            pauseUI.SetActive(false);
            animator.SetTrigger(settingTrigger);// 設定画面を表示させるトリガーをセット
        }

        /// <summary>
        /// 設定画面から戻る関数
        /// </summary>
        public void SettingReturn()
        {
            StartCoroutine(SettingReturnCoroutine());// コルーチンを呼び出し
        }

        /// <summary>
        /// 設定画面から戻る際の演出用コルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator SettingReturnCoroutine()
        {
            animator.SetTrigger(settingReturnTrigger);// 設定画面から戻るトリガーをセット
            yield return new WaitForSecondsRealtime(settingReturnTime);// アニメーション分待機
            pauseUI.SetActive(true);
        }

        /// <summary>         
        /// UIを表示させる関数         
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
        /// UIを非表示にする関数         
        /// </summary>         
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