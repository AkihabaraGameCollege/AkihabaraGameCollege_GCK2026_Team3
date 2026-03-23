using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理の名前空間を追加
using TMPro; // TextMeshProの名前空間を追加
using UnityEngine.EventSystems;

public class BlackHole : MonoBehaviour
{
    [Header("引力設定")]
    [SerializeField]
    private float pullForce = 10f; // 引力の強さ

    [Header("引力半径設定")]
    [SerializeField]
    private float pullRadius = 5f; // 引力が及ぶ半径

    // ゲームオーバーUIのゲームオブジェクト
    [SerializeField]
    private UI_Anim ui_anim;

    // ゲームオーバータイムのテキストメッシュプロ
    [SerializeField]
    private TextMeshProUGUI gameOverTimeText;

    // サウンドマネージャーの参照
    [SerializeField]
    private SoundManager soundManager;

    // GameDirectoryの参照
    private GameDirectory gameDirectory;

    // PauseManagerの参照
    private PauseManager pauseManager;

    //最初にセレクトするボタン
    public GameObject firstSelectedButton;

    //PlayerはDeadAreaを当たったか確認で、2回度を当たらないように
    private bool isDead;

    private void Start()
    {
        //PlayerはまだDeadAreaを当たってない
        isDead = false;

        // SoundManagerを取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();
        // GameDirectoryを取得
        gameDirectory = Object.FindFirstObjectByType<GameDirectory>();
        // PauseManagerを取得
        pauseManager = Object.FindFirstObjectByType<PauseManager>();

        // マウスカーソルを表示
        Cursor.visible = true;
    }

    private void Update()
    {
        // 毎フレーム引力を適用
        ApplyGravitationalPull();
    }

    // 引力を適用する関数
    private void ApplyGravitationalPull()
    {
        // 指定した半径内の全てのコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(transform.position, pullRadius);
        foreach (Collider collider in colliders)
        {
            // "Player"タグを持つオブジェクトに対してのみ引力を適用
            if (collider.CompareTag("Player"))
            {
                // Rigidbodyコンポーネントを取得
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // ブラックホールの中心方向へのベクトルを計算
                    Vector3 direction = (transform.position - collider.transform.position).normalized;
                    // ブラックホールからの距離を計算
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    // 距離に応じて力を調整
                    float force = pullForce * (1 - (distance / pullRadius));
                    // 力を適用
                    rb.AddForce(direction * force, ForceMode.Acceleration);
                }
            }
        }
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

            //ゲームオーバーUIを表示し
            if (ui_anim != null)
            {
                ui_anim.OpenGameoverUI();
            }
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
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
                // ゲームオーバータイムを表示
                gameOverTimeText.text = gameDirectory.FormatTime(gameDirectory.elapsedTime);
            }

            //ポーズを無効化
            if (pauseManager != null)
            {
                pauseManager.SetGameOverPause();
            }

            // タイムスケールを変更しない
            // Time.timeScale = 0f;
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
        Time.timeScale = 1f; // タイムスケールを元に戻す

        // ポーズ状態をリセット
        if (pauseManager != null)
        {
            pauseManager.ResetPause();
        }
    }
}
