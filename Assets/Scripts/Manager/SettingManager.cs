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

        [SerializeField]
        private float settingReturnTime = 1;

        /// <summary>
        /// 設定ボタンが押されたときに呼ばれるIDの変数
        /// </summary>
        public static readonly int settingTrigger = Animator.StringToHash("Setting");
        /// <summary>
        /// 設定画面から戻るときに呼ばれるIDの変数
        /// </summary>
        public static readonly int settingReturnTrigger = Animator.StringToHash("SettingReturn");

        /// <summary>
        /// プレイヤー操作クラスを参照する変数
        /// </summary>
        private PlayerController playerController = null;
        /// <summary>
        /// タイトルシーン管理クラスを参照する変数
        /// </summary>
        private TitleScene titleScene = null;

        /// <summary>
        /// プレイヤーのオブジェクト名を参照する変数
        /// </summary>
        public string playerRootName = "PlayerRoot";
        /// <summary>
        /// タイトルマネージャーのオブジェクト名を参照する変数
        /// </summary>
        public string titleSceneRootName = "SceneRoot";

        /// <summary>
        /// 初期設定の関数
        /// </summary>
        private void Start()
        {
            playerController = GameObject.Find(playerRootName).GetComponent<PlayerController>();// シーン内からプレイヤーを探して取得
            titleScene = GameObject.Find(titleSceneRootName).GetComponent<TitleScene>();// シーン内からタイトルマネージャーを探して取得
            Hide();// UIは消しておく
            settingReturnButton.onClick.AddListener(SettingReturn);// 設定画面から戻るボタンにその機能をつかさどる関数を登録
        }

        /// <summary>
        /// 設定画面を表示する関数
        /// </summary>
        public void DisplaySetting()
        {
            animator.SetTrigger(settingTrigger);// 設定画面を表示させるトリガーをセット
          
            // ゲームの最初の画面にいない場合
            if (!titleScene.isStartScene)
            {
                playerController.isCanPause = false;// ポーズ操作を禁止する
            }
        }

        /// <summary>
        /// 設定画面から戻る関数
        /// </summary>
        public void SettingReturn()
        {
            StartCoroutine(SettingReturnCoroutine());// コルーチンを呼び出し

            // ゲームの最初の画面にいない場合
            if (!titleScene.isStartScene)
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
            animator.SetTrigger(settingReturnTrigger);// 設定画面から戻るトリガーをセット
            yield return new WaitForSecondsRealtime(settingReturnTime);// アニメーション分待機
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