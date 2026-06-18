using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameClear : MonoBehaviour
{
    // ステージクリアUIのゲームオブジェクト
    //[SerializeField] GameObject StageClearUI;
    [SerializeField]
    UI_Anim uI_Anim;

    //サウンドマネージャーの参照
    [SerializeField]
    private SoundManager soundManager;
    // ゲームクリアタイムのテキストメッシュプロ
    [SerializeField]
    TextMeshProUGUI gameClearTimeText;

    private GameDirectory gameDirectory;

    //最初にセレクトするボタン
    public GameObject firstSelectedButton;

    private bool isTriggered = false; // 一度だけトリガーするためのフラグ

    private void Start()
    {
        // ゲーム開始時にステージクリアUIを非表示にする
        //StageClearUI.SetActive(false);
        // SoundManagerを取得
        soundManager = FindFirstObjectByType<SoundManager>();
        // GameDirectoryを取得
        gameDirectory = Object.FindFirstObjectByType<GameDirectory>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        // すでにトリガー済みなら何もしない
        if (isTriggered) return;

        // プレイヤーがトリガーに入った場合、ステージクリアUIを表示し、SEを再生する
        if (collider.gameObject.tag == "Player")
        {
            isTriggered = true; // 最初の一回だけ処理する

            uI_Anim.OpenGameclearUI();
            Cursor.visible = true; // マウスカーソルを表示
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);//ボタンをセレクトさせる
            if (soundManager != null)
            {
                soundManager.PlayGameClearAudio();
            }

            // GameDirectoryのインスタンスを取得し、経過時間を停止する
            if (gameDirectory != null)
            {
                gameDirectory.StopElapsedTime();
                gameDirectory.SetGameClear(); // ゲームクリアを設定
                gameClearTimeText.text = gameDirectory.FormatTime(gameDirectory.elapsedTime);
            }
            //ポーズを無効化
            PauseManager pauseManager = Object.FindFirstObjectByType<PauseManager>();
            if (pauseManager != null)
            {
                pauseManager.SetGameClearPause();
            }
        }
    }
}
