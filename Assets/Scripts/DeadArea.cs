using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理の名前空間を追加
using TMPro; // TextMeshProの名前空間を追加
using UnityEngine.EventSystems;

public class DeadArea : MonoBehaviour
{
    // ゲームオーバーUIのゲームオブジェクト
    //[SerializeField] GameObject GameOverUI;
    [SerializeField]
    private UI_Anim ui_anim;
    [SerializeField]
    GameObject CheckpointUI;

    // ゲームオーバータイムのテキストメッシュプロ
    [SerializeField]
    TextMeshProUGUI gameOverTimeText;

    // サウンドマネージャーの参照
    [SerializeField]
    private SoundManager soundManager;

    // GameDirectoryの参照
    private GameDirectory gameDirectory;

    // PauseManagerの参照
    private PauseManager pauseManager;

    // Checkpointの参照
    private Checkpoint checkpoint;

    //最初にセレクトするボタン
    public GameObject firstSelectedButton;

    //チェックポイント到達後選択するボタン
    public GameObject checkSelectedButton;

    //PlayerはDeadAreaを当たったか確認で、2回度を当たらないように
    private bool isDead;

    private void Start()
    {
        // ゲーム開始時にゲームオーバーUIを非表示にする
        //GameOverUI.SetActive(false);

        //PlayerはまだDeadAreaを当たってない
        isDead = false;

        // SoundManagerを取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();
        // GameDirectoryを取得
        gameDirectory = Object.FindFirstObjectByType<GameDirectory>();
        // PauseManagerを取得
        pauseManager = Object.FindFirstObjectByType<PauseManager>();
        // Checkpointの取得
        checkpoint = Object.FindFirstObjectByType<Checkpoint>();

        // マウスカーソルを表示
        Cursor.visible = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        // **ゲームクリアしていたら何もしない**
        if (pauseManager != null && pauseManager.IsGameClear)
        {
            return;
        }

        // プレイヤーがデッドエリアに入った場合、ゲームオーバーUIを表示し、SEを再生する
        if (collider.gameObject.tag == "Player" && !isDead)
        {
            //PlayerはまだDeadAreaを当たった
            isDead = true;

            // ゲームオーバータイムを表示
            if (gameDirectory != null && gameOverTimeText != null)
            {
                gameOverTimeText.text = gameDirectory.timerText.text;
            }

            //ゲームオーバーUIを表示し
            if (ui_anim != null)
            {
                ui_anim.OpenGameoverUI();
            }
            //チェックポイントに到達していれば
            if(checkpoint != null && checkpoint.GetComponent<Checkpoint>().isCheckepoint || CheckpointUI.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(checkSelectedButton);
            }
            else // そうでなければ
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
            Cursor.visible = true; // マウスカーソルを表示
            if (soundManager != null)
            {
                soundManager.PlayGameOverAudio();
            }

            // プレイヤー操作を無効にする
            PlayerController playerController = collider.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // GameDirectoryのSetGameOverメソッドを呼び出す
            if (gameDirectory != null)
            {
                gameDirectory.SetGameOver();
            }

            //ポーズを無効化
            if (pauseManager != null)
            {
                pauseManager.SetGameOverPause();
            }
        }
    }

    // タイトルシーンに戻るメソッドを追加
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene"); // タイトルシーンをロード
        Cursor.visible = true; // マウスカーソルを表示
    }

    //RetryかCheckpointを戻るとき使う
    public void Rerieve()
    {
        isDead = false;
    }
}
