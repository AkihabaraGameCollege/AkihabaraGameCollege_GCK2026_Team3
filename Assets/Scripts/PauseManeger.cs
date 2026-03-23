using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseUI; // ポーズメニューのUIオブジェクト
    private bool isPaused = false; // ゲームがポーズ中かどうか
    private bool isGameOver = false; // ゲームオーバー中かどうか
    private bool isGameClear = false; // ゲームクリア中かどうか
    [SerializeField]
    private string sceneToLoad; // リトライ時にロードするシーン名

    private GameDirectory gameDirectory; // GameDirectoryのインスタンス
    private SoundManager soundManager; // SoundManagerのインスタンス

    //最初にセレクトするボタン
    public GameObject firstSelectedButton;
    private void Awake()
    {
        gameDirectory = Object.FindFirstObjectByType<GameDirectory>(); // GameDirectoryのインスタンスを取得
        soundManager = Object.FindFirstObjectByType<SoundManager>(); // SoundManagerのインスタンスを取得
    }

    // isPausedプロパティを公開
    public bool IsPaused => isPaused;
    // ゲームオーバー状態の確認用
    public bool IsGameOver => isGameOver;
    // ゲームクリア状態の確認用
    public bool IsGameClear => isGameClear;

    // ゲームオーバー時に呼び出されるメソッド
    public void SetGameOverPause()
    {
        isGameOver = true;
    }

    // ゲームクリア時に呼び出されるメソッド
    public void SetGameClearPause()
    {
        isGameClear = true;
    }

    // ポーズ状態を切り替えるメソッド
    public void TogglePause()
    {
        // **ゲームクリア中ならポーズを無効化**
        if (isGameOver || isGameClear) return;

        isPaused = !isPaused; // ポーズ状態を反転

        if (isPaused)
        {
            Time.timeScale = 0f; // ポーズ：時間を止める
            PauseUI.SetActive(true); // ポーズメニューを表示
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            if (gameDirectory != null)
            {
                gameDirectory.SetGamePause(true); // ゲームポーズを設定
            }
        }
        else
        {
            Time.timeScale = 1f; // 再開：時間を元に戻す
            PauseUI.SetActive(false); // ポーズメニューを非表示
            if (gameDirectory != null)
            {
                gameDirectory.SetGamePause(false); // ゲームポーズを解除
            }
        }
    }

    // 再開ボタンが押されたときにポーズを解除
    public void ResumeGame()
    {
        if (soundManager != null)
        {
            soundManager.PlayClickAudio(); // クリック音を再生
        }
        TogglePause(); // ポーズを解除
    }

    // リトライボタンが押されたときにステージをリロード
    public void RetryStage()
    {
        if (soundManager != null)
        {
            soundManager.PlayClickAudio(); // クリック音を再生
        }
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad); // 設定されたシーンをロード
            Time.timeScale = 1f; // 時間を元に戻す
            if (gameDirectory != null)
            {
                gameDirectory.SetGamePause(false); // ゲームポーズを解除
            }
        }
        else
        {
            Debug.LogError("シーン名が設定されていません！");
        }
    }

    // タイトルボタンが押されたときにタイトルシーンをロード
    public void OnClickTitleButton()
    {
        if (soundManager != null)
        {
            soundManager.PlayClickAudio(); // クリック音を再生
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene"); // タイトルシーンをロード
        Cursor.visible = true; // マウスカーソルを表示
    }

    // ポーズ状態をリセットする
    public void ResetPause()
    {
        isPaused = false;
        isGameOver = false;
        isGameClear = false;
        Time.timeScale = 1f;
        if (PauseUI != null)
        {
            PauseUI.SetActive(false);
        }
        if (gameDirectory != null)
        {
            gameDirectory.SetGamePause(false); // ゲームポーズを解除
        }
    }
}
