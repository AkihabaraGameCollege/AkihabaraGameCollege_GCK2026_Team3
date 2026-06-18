using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [Header("火によるHP減少率")]
    [SerializeField]
    private float fireHPDecreaseRate = 5f;  // 火によるHP減少率

    // プレイヤーがトリガー内に留まっている間に呼び出される
    private void OnTriggerStay(Collider other)
    {
        // トリガーに入ったオブジェクトがプレイヤーかどうかを確認
        if (other.CompareTag("Player"))
        {
            // プレイヤーのコントローラーを取得
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // プレイヤーのHPを減少させる
                playerController.HPFillAmount -= fireHPDecreaseRate * Time.deltaTime;

                // HP UIを更新
                if (playerController.hpUIImage != null)
                {
                    playerController.hpUIImage.fillAmount = playerController.HPFillAmount / 180f;
                }

                // プレイヤーのHPが0以下になった場合、ゲームオーバー処理を実行
                if (playerController.HPFillAmount <= 0 && !playerController.isGameOver)
                {
                    playerController.isGameOver = true;  // ゲームオーバー状態を記録
                    playerController.HandleGameOver();  // ゲームオーバー処理を実行
                }

                // 炎ダメージSEを再生
                SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();
                if (soundManager != null)
                {
                    soundManager.PlayFireDamageAudio();
                }
            }
        }
    }
}
