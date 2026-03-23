using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class GameDirectory : MonoBehaviour
{
    [Header("ゲームタイムテキスト設定")]
    [SerializeField]
    public TextMeshProUGUI timerText; // TextMeshProの参照を取得
    [Header("チェックポイントタイムテキスト設定")]
    [SerializeField]
    private TextMeshProUGUI checkpointTimerText; // 新たなTextMeshProの参照を取得
    [SerializeField]
    private int playStage;

    [HideInInspector]
    public float elapsedTime = 0f; // 経過時間を保持する変数
    private float checkpointTime = 0f; // チェックポイント到達時の時間を保持する変数
    private bool isBlinking = false; // チェックポイントタイムテキストが点滅中かどうかを示すフラグ
    private float updateInterval = 0.1f; // タイマー更新の間隔
    private float nextUpdate = 0f; // 次のタイマー更新時間

    private Vector3 checkpointPosition; // チェックポイントの位置を保持する変数
    private GameObject player; // プレイヤーの参照を保持する変数
    private bool isGameClear = false; // ゲームクリア状態を示すフラグ
    private bool isGameOver = false; // ゲームオーバー状態を示すフラグ
    private bool isGamePaused = false; // ゲームポーズ状態を示すフラグ

    void Awake()
    {
        // シングルトンインスタンスの初期化を削除
    }

    void Start()
    {
        InitializeCheckpointTimerText(); // チェックポイントタイムテキストを初期化
        Cursor.visible = false; // ゲーム開始時にマウスカーソルを非表示にする
        player = GameObject.FindGameObjectWithTag("Player"); // プレイヤーオブジェクトを取得
    }

    void Update()
    {
        if (!isGameClear && !isGameOver && !isGamePaused)
        {
            UpdateElapsedTime(); // 経過時間を更新
            if (Time.time >= nextUpdate)
            {
                UpdateTimerText(elapsedTime); // タイマーのテキストを更新
                nextUpdate = Time.time + updateInterval; // 次の更新時間を設定
            }

            if (SceneManager.GetActiveScene().name == "TitleScene")
            {
                Cursor.visible = true;
            }
            else
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    Cursor.visible = true;
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Cursor.visible = false;
                }
            }
        }
    }

    void InitializeCheckpointTimerText()
    {
        checkpointTimerText.text = "-'--,---"; // 初期表示
    }

    void UpdateElapsedTime()
    {
        elapsedTime += Time.deltaTime; // フレームごとの経過時間を加算
    }

    void UpdateTimerText(float time)
    {
        timerText.text = FormatTime(time); // フォーマットされた時間をテキストに設定
    }

    void UpdateCheckpointTimerText(float time)
    {
        checkpointTimerText.text = FormatTime(time); // フォーマットされた時間をテキストに設定
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F); // 分を計算
        int seconds = Mathf.FloorToInt(time % 60F); // 秒を計算
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F); // ミリ秒を計算
        return string.Format("{0:0}'{1:00},{2:000}", minutes, seconds, milliseconds); // フォーマットされた文字列を返す
    }

    public void OnCheckpointReached(Vector3 position)
    {
        checkpointTime = elapsedTime; // チェックポイント到達時の時間を設定
        checkpointPosition = position; // チェックポイントの位置を設定
        UpdateCheckpointTimerText(checkpointTime); // チェックポイントタイムのテキストを更新

        // プレイヤーの耐久値を全回復
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.HPFillAmount = 180f;
            if (playerController.hpUIImage != null)
            {
                playerController.hpUIImage.fillAmount = playerController.HPFillAmount / 180f;
            }
        }

        if (!isBlinking)
        {
            StartCoroutine(BlinkCheckpointTimerText()); // チェックポイントタイムテキストを点滅させるコルーチンを開始
        }
    }

    public void ReturnToCheckpoint()
    {
        elapsedTime = checkpointTime; // 経過時間をチェックポイント到達時の時間にリセット
        UpdateTimerText(elapsedTime); // タイマーのテキストを更新
        player.transform.position = checkpointPosition; // プレイヤーの位置をチェックポイントの位置にリセット

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.ResetState(); // プレイヤーの状態をリセット
            playerController.StopAllMove();//プレイヤーの動きをリセット
        }

        isGameClear = false;
        isGameOver = false;

        if (checkpointTimerText != null)
        {
            UpdateCheckpointTimerText(checkpointTime);
        }
    }

    private IEnumerator BlinkCheckpointTimerText()
    {
        isBlinking = true; // 点滅中フラグを設定
        for (int i = 0; i < 6; i++) // 2～3秒間点滅させるために6回繰り返す
        {
            checkpointTimerText.enabled = !checkpointTimerText.enabled; // テキストの表示/非表示を切り替え
            yield return new WaitForSeconds(0.5f); // 0.5秒待機
        }
        checkpointTimerText.enabled = true; // 点滅終了後にテキストを表示状態に戻す
        isBlinking = false; // 点滅中フラグを解除
    }

    public void StopElapsedTime()
    {
        isGameClear = true;
    }

    public void SetGameOver()
    {
        isGameOver = true;
        Cursor.visible = true; // ゲームオーバー時にマウスカーソルを表示
    }

    public void SetGameClear()
    {
        SaveManager saveManager = new SaveManager();
        saveManager.SaveRecordTime(playStage, elapsedTime);
        isGameClear = true;
        Cursor.visible = true; // ゲームクリア時にマウスカーソルを表示
    }

    public void SetGamePause(bool isPaused)
    {
        isGamePaused = isPaused;
        Cursor.visible = isPaused; // ポーズ時にマウスカーソルを表示
    }
}
