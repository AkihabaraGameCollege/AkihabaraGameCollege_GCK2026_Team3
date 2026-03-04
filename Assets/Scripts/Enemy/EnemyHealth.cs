using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForestDraw.Enemy.Data;
using System;
using ForestDraw.Combat;

/// <summary>
/// 敵のHP管理クラス。
/// ダメージ処理・無敵時間・HPバー更新を担当する。
/// </summary>
namespace ForestDraw.Enemy.Components
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        // =========================
        // ▼ 設定値
        // =========================

        private int health; // 現在HP（初期値）
        private int maxHealth;     // 最大HP（開始時に保存）
        private float damageInterval = 1f; // ダメージを受けた後の無敵時間（秒）

        [SerializeField]
        private Image hpFillImage; // HPバーのFill部分（UI）

        // =========================
        // ▼ 内部状態管理
        // =========================

        private bool canTakeDamage = true; // ダメージを受けられる状態かどうか

        // =========================
        // ▼ イベント
        // =========================
        public event Action Died;  // 死亡時に発火

        // =========================
        // 外部から移動パラメータを設定
        // =========================
        public void Initialize(EnemyData data)
        {
            damageInterval = data.takeDamageInterval;
            health = data.maxHealth;
            maxHealth = health;
            UpdateHPBar();
        }
        // =========================
        // ダメージ処理
        // =========================
        public void TakeDamage(int amount)
        {
            // 無敵状態、または既に死亡している場合は処理しない
            if (!canTakeDamage || health <= 0) return;

            // HPを減らす
            health -= amount;

            if (health <= 0)
            {
                // HPが0以下になったら死亡処理
                health = 0;
                UpdateHPBar();
                Die();
            }
            else
            {
                // 生存している場合はHPバー更新
                UpdateHPBar();

                // 一定時間無敵状態にする
                canTakeDamage = false;
                StartCoroutine(DamageCooldown());
            }
        }

        private void Die()
        {
            Died?.Invoke();
            Destroy(gameObject);
        }

        // =========================
        // 無敵時間管理コルーチン
        // =========================
        private IEnumerator DamageCooldown()
        {
            // 指定秒数待機
            yield return new WaitForSeconds(damageInterval);

            // 再びダメージを受けられる状態に戻す
            canTakeDamage = true;
        }

        // =========================
        // HPバー更新処理
        // =========================
        private void UpdateHPBar()
        {
            if (hpFillImage == null) return;
                // 現在HPの割合をFillAmountに反映
                hpFillImage.fillAmount = (float)health / maxHealth;
        }
    }
}