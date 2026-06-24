using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// チュートリアル用UIを管理するクラス
    /// </summary>
    public class Tutorial_UI_Manager : MonoBehaviour
    {
        /// <summary>
        /// ページオブジェクトの配列を参照する変数
        /// </summary>
        public GameObject[] tutorial_Pages;

        /// <summary>
        /// 次へボタンを参照する変数
        /// </summary>
        public Button nextButton;

        /// <summary>
        /// スキップボタンを参照する変数
        /// </summary>
        public Button skipButton;

        /// <summary>
        /// メインステージ管理クラスを参照する変数
        /// </summary>
        private StageScene stageScene;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        public Animator Tutorial_Animator;

        /// <summary>
        /// 現在表示しているページのインデックスを参照する変数
        /// </summary>
        private int currentPage_Index;
        /// <summary>
        /// チュートリアルを終了するときに呼ばれるIDを参照する変数
        /// </summary>
        public static readonly int Tutorial_EndTrigger = Animator.StringToHash("OnTutorial_End");

        /// <summary>
        /// メインステージ管理を行うオブジェクト名を参照する変数
        /// </summary>
        private string stageSceneName = "SceneRoot";

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // --- コンポーネントの登録 ---
            // アニメーションを登録
            Tutorial_Animator = GetComponent<Animator>();
            // シーン内からメインステージ管理クラスを探して取得
            stageScene = GameObject.Find(stageSceneName).GetComponent<StageScene>();

            nextButton.onClick.AddListener(OnNextButtonClicked);// 次へボタンが押された時の処理を登録

            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipButtonClicked);// スキップボタンが押された時の処理を登録
            }

            HidePages();
        }

        /// <summary>
        /// 次へボタンが押された時に呼ばれる関数
        /// </summary>
        public void OnNextButtonClicked()
        {
            // もし現在のページより多くページが残っている場合
            if (currentPage_Index < tutorial_Pages.Length)
            {
                tutorial_Pages[currentPage_Index].SetActive(false);// 現在のページは消す
            }

            currentPage_Index++;// 次のページへ進む

            // もし次めくったページより多くページが残っている場合
            if (currentPage_Index < tutorial_Pages.Length)
            {
                tutorial_Pages[currentPage_Index].SetActive(true);// めくったページを表示
            }
            else
            {
                StartCoroutine(stageScene.Tutorial_OutroCoroutine());// チュートリアルを終了する
                DisableTutorialInteraction();
            }
        }

        /// <summary>
        /// スキップボタンが押された時に呼ばれる関数
        /// </summary>
        public void OnSkipButtonClicked()
        {
            DisableTutorialInteraction();

            // チュートリアル終了処理を開始
            StartCoroutine(stageScene.Tutorial_OutroCoroutine());
        }

        /// <summary>         
        /// UIの表示をやめる関数         
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
        /// チュートリアルのページ非表示を行う関数         
        /// </summary>         
        public void HidePages()
        {
            // すべてのページを参照するループ
            for (int i = 0; i < tutorial_Pages.Length; i++)
            {
                tutorial_Pages[i].SetActive(false);// ページを非表示にする
            }
        }

        /// <summary>         
        /// チュートリアルの最初のページ表示を行う関数         
        /// </summary>         
        public void ShowFirstPage()
        {
            // すべてのページを参照するループ
            for (int i = 0; i < tutorial_Pages.Length; i++)
            {
                tutorial_Pages[i].SetActive(i == 0);// 最初のページのみ表示
            }

            // 現在ページを先頭にリセット
            currentPage_Index = 0;
        }

        /// <summary>
        /// チュートリアルUIの操作を無効化する（ボタンの重複押下防止）
        /// </summary>
        private void DisableTutorialInteraction()
        {
            if (nextButton != null) nextButton.interactable = false;
            if (skipButton != null) skipButton.interactable = false;
        }
    }
}