using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// 設定画面を管理するクラス
    /// </summary>
    public class SettingUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// アニメーターコンポーネントの変数
        /// </summary>
        private Animator animator;

        /// <summary>
        /// 設定画面から戻るボタンの変数
        /// </summary>
        [SerializeField]
        private Button settingReturnButton = null;

        /// <summary>
        /// 設定UI内の演出用UIを参照する変数
        /// </summary>
        public GameObject settingTransitionUI;

        /// <summary>
        /// 設定画面を表示するまでの待機時間を参照する変数
        /// </summary>
        private float displaySettingTime = 0.5f;

        /// <summary>
        /// 設定ボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        private static readonly int onSettingTrigger = Animator.StringToHash("OnSetting");
        /// <summary>
        /// 設定画面から戻るときに呼ばれるIDの変数
        /// </summary>
        private static readonly int onSettingReturnTrigger = Animator.StringToHash("OnSettingReturn");

        /// <summary>
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;

        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        public string playerRootName = "PlayerRoot";

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            // コンポーネントの登録
            animator = GetComponent<Animator>();
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得

            settingReturnButton.onClick.AddListener(SettingReturn);// 設定画面から戻るボタンにその機能をつかさどる関数を登録
            Hide();
        }

        /// <summary>
        /// 設定画面から戻る関数
        /// </summary>
        public void SettingReturn()
        {
            StartCoroutine(SettingReturnCoroutine());// コルーチンを呼び出し

            // ゲームの最初の画面にいない場合
            if (!TitleScene.isStartScene)
            {
                playerController.isCanPause = true;// ポーズ操作を許可する
            }
        }

        /// <summary>
        /// 設定画面から戻る際の演出用コルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator SettingReturnCoroutine()
        {
            animator.SetTrigger(onSettingReturnTrigger);// 設定画面から戻るトリガーをセット
            yield return new WaitForSecondsRealtime(displaySettingTime);// 設定画面を表示するまでの待機
            Hide();
            TargetShow(settingTransitionUI);// 演出用UIを表示する

            // ゲームの最初の画面にいない場合
            if (!TitleScene.isStartScene)
            {
                PauseUI_Manager.Instance.Show();
            }
        }

        /// <summary>         
        /// UIを表示させる関数         
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
        /// UIを非表示させる関数         
        /// </summary>         
        private void Hide()
        {
            // 子オブジェクトをすべてアクティブ化
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 設定画面を表示する際の演出用コルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplaySettingCoroutine()
        {
            // ゲームの最初の画面にいない場合
            if (!TitleScene.isStartScene)
            {
                PauseUI_Manager.Instance.Hide();
            }

            TargetShow(settingTransitionUI);
            animator.SetTrigger(onSettingTrigger);// 設定画面を表示させるトリガーをセット
            yield return new WaitForSecondsRealtime(displaySettingTime);// 設定画面を表示するまでの待機
            Show();

            // ゲームの最初の画面にいる場合
            if (TitleScene.isStartScene)
            {
                playerController.isCanPause = false;// ポーズ操作を禁止する
            }
        }

        /// <summary>
        /// 設定画面から戻る関数
        /// </summary>
        public void DisplaySetting()
        {
            StartCoroutine(DisplaySettingCoroutine());// コルーチンを呼び出し

            // ゲームの最初の画面にいない場合
            if (!TitleScene.isStartScene)
            {
                playerController.isCanPause = false;// ポーズ操作を禁止する
            }
        }

        /// <summary>         
        /// 指定したUIの表示をやめる関数         
        /// </summary>         
        private void TargetShow(GameObject target)
        {
            target.SetActive(true);// 指定したUIをアクティブ化
        }
    }
}