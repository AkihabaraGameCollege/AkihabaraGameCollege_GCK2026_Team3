using UnityEngine;
using System.Collections;

public class TeleportEntrance : MonoBehaviour
{
    // テレポート先の出口オブジェクト
    public GameObject teleportExit;
    // クールダウン時間
    public float cooldownTime = 1.0f;
    // クールダウン中かどうかのフラグ
    private bool isOnCooldown = false;
    // サウンドマネージャーの参照
    private SoundManager soundManager;

    void Start()
    {
        // サウンドマネージャーをシーンから探して取得
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーがトリガーに入ったとき、クールダウン中でなければテレポートを開始
        if (teleportExit != null && other.CompareTag("Player") && !isOnCooldown)
        {
            StartCoroutine(TeleportWithCooldown(other));
        }
    }

    private IEnumerator TeleportWithCooldown(Collider player)
    {
        // クールダウンを開始
        isOnCooldown = true;
        // プレイヤーをテレポート先に移動
        player.transform.position = teleportExit.transform.position;
        // テレポート先のコライダーを一時的に無効化
        teleportExit.GetComponent<Collider>().enabled = false;

        // テレポート効果音を再生
        if (soundManager != null)
        {
            soundManager.PlayTeleportAudio();
        }

        // クールダウン時間を待つ
        yield return new WaitForSeconds(cooldownTime);

        // プレイヤーがトリガーから離れるまで待つ
        while (Vector3.Distance(player.transform.position, teleportExit.transform.position) < 1.0f)
        {
            yield return null;
        }

        // クールダウンを終了
        isOnCooldown = false;
        // テレポート先のコライダーを再度有効化
        teleportExit.GetComponent<Collider>().enabled = true;
    }
}
