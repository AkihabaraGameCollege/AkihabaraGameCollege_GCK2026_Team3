using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameDirectory gameDirectory;
    private SoundManager soundManager;
    private PlayerController playerController;
    private UIManeger uiManager;
    private PauseManager pauseManager;
    public bool isCheckepoint;

    void Start()
    {
        // GameDirectoryスクリプトを持つオブジェクトを探す
        gameDirectory = Object.FindFirstObjectByType<GameDirectory>();

        // SoundManagerスクリプトを持つオブジェクトを探す
        soundManager = Object.FindFirstObjectByType<SoundManager>();

        // PlayerControllerスクリプトを持つオブジェクトを探す
        playerController = Object.FindFirstObjectByType<PlayerController>();

        // UIManegerスクリプトを持つオブジェクトを探す
        uiManager = Object.FindFirstObjectByType<UIManeger>();
        pauseManager = Object.FindFirstObjectByType<PauseManager>(); // PauseManagerの取得

        //チェックポイントをまだ当たってない
        isCheckepoint = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーがチェックポイントに触れたとき
        if (other.CompareTag("Player"))
        {
            //２回どにチェックポイントを当たらないように
            if (!isCheckepoint)
            {
                HandleCheckpointReached();
                isCheckepoint = true;
            }
        }
    }

    // チェックポイント到達時の処理を行うメソッド
    private void HandleCheckpointReached()
    {
        if (pauseManager != null)
        {
            pauseManager.ResetPause(); // ポーズをリセット
        }
        // GameDirectoryにチェックポイント到達を通知
        if (gameDirectory != null)
        {
            gameDirectory.OnCheckpointReached(transform.position);
        }

        // SoundManagerが存在する場合、チェックポイント到達音を再生
        if (soundManager != null)
        {
            soundManager.PlayCheckpointAudio();
        }

        // PlayerControllerが存在する場合、耐久値を全回復
        if (playerController != null)
        {
            playerController.RestoreHP();
        }

        // UIManegerが存在する場合、チェックポイントに戻るボタンを表示
        if (uiManager != null)
        {
            uiManager.ShowReturnToCheckpointButton();
        }
    }

}
