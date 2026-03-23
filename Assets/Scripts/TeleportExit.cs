using UnityEngine;
using System.Collections;

public class TeleportExit : MonoBehaviour
{
    // テレポートの入口オブジェクト
    public GameObject teleportEntrance;
    // クールダウン時間
    public float cooldownTime = 1.0f;
    // クールダウン中かどうかのフラグ
    private bool isOnCooldown = false;
    // サウンドマネージャーの参照
    private SoundManager soundManager;

    void Start()
    {
        // サウンドマネージャーをシーン内から探して取得
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーがトリガーに入ったかつクールダウン中でない場合にテレポートを開始
        if (teleportEntrance != null && other.CompareTag("Player") && !isOnCooldown)
        {
            StartCoroutine(TeleportWithCooldown(other));
        }
    }

    private IEnumerator TeleportWithCooldown(Collider player)
    {
        // クールダウンを開始
        isOnCooldown = true;
        // プレイヤーをテレポートの入口の位置に移動
        player.transform.position = teleportEntrance.transform.position;
        // テレポートの入口のコライダーを無効化
        teleportEntrance.GetComponent<Collider>().enabled = false;

        // テレポート効果音を再生
        if (soundManager != null)
        {
            soundManager.PlayTeleportAudio();
        }

        // クールダウン時間を待つ
        yield return new WaitForSeconds(cooldownTime);

        // プレイヤーがトリガーから離れるまで待つ
        while (Vector3.Distance(player.transform.position, teleportEntrance.transform.position) < 1.0f)
        {
            yield return null;
        }

        // クールダウンを終了
        isOnCooldown = false;
        // テレポートの入口のコライダーを有効化
        teleportEntrance.GetComponent<Collider>().enabled = true;
    }
}
