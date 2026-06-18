using UnityEngine;

public class FireSlopeDamage : MonoBehaviour
{
    [Header("火によるHP減少量")]
    [SerializeField]
    private float fireHPDecreaseAmount = 50f;  // 火に当たった瞬間のHP減少量

    // プレイヤーがトリガーに入った瞬間に呼び出される
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // HP を減らす
                playerController.HPFillAmount -= fireHPDecreaseAmount;

                // HP UIを更新
                if (playerController.hpUIImage != null)
                {
                    playerController.hpUIImage.fillAmount = playerController.HPFillAmount / 180f;
                }

                // HP が 0 以下になったらゲームオーバー
                if (playerController.HPFillAmount <= 0 && !playerController.isGameOver)
                {
                    playerController.isGameOver = true;
                    playerController.HandleGameOver();
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