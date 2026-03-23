using UnityEngine;
using System.Collections;

public class AccelPoint : MonoBehaviour
{
    // Z方向の加速値
    [SerializeField]
    private int ZSprint = 1;
    // X方向の加速値
    [SerializeField]
    private int XSprint = 1;
    // Y方向の加速値
    [SerializeField]
    private int YSprint = 1;

    // SoundManagerの参照
    private SoundManager soundManager;

    // エラーを出すため削除
    /*
    private void Start()
    {
        // シーン内のSoundManagerを検索して取得
        soundManager = FindObjectOfType<SoundManager>();
    }
    */

    private void OnTriggerEnter(Collider Player)
    {
        // プレイヤーのrigitbodyを取得する
        Rigidbody rb = Player.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // プレイヤーの力をゼロにする
            rb.linearVelocity = Vector3.zero;

            // 加速する
            Vector3 force = new Vector3(XSprint, YSprint, ZSprint);
            rb.AddForce(force, ForceMode.VelocityChange);

            // ジャンプSEを一度だけ再生
            if (soundManager != null)
            {
                soundManager.PlayJumpAudio();
            }
        }
    }
}
