using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using ForestDraw.Combat;

namespace ForestDraw.Enemy.Components
{
    /// <summary>
    /// 敵のHPを管理するコンポーネント。
    /// ダメージ処理、無敵時間制御、死亡通知を行う。
    /// </summary>
    public class EnemyHealth : MonoBehaviour, IDamageable, IEnemyComponent
    {
        // ===== 状態 =====
        private int health;
        private int maxHealth;
        private bool canTakeDamage = true;

        // ===== 設定 =====
        private float damageInterval = 1f;

        // ===== UI参照 =====
        [SerializeField] private Image hpFillImage;

        /// <summary>
        /// HPが0になったときに発火する
        /// </summary>
        public event Action Died;

        /// <summary>
        /// ScriptableObjectから初期ステータスを設定する
        /// </summary>
        public void Initialize(EnemyInitContext context)
        {
            damageInterval = context.data.takeDamageInterval;
            maxHealth = context.data.maxHealth;
            health = maxHealth;
            UpdateHPBar();
        }

        /// <summary>
        /// ダメージを適用する
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (!canTakeDamage || health <= 0) return;

            health = Mathf.Max(health - amount, 0);
            UpdateHPBar();

            if (health > 0)
            {
                StartInvincibility();
                return;
            }

            Die();
        }

        private void StartInvincibility()
        {
            canTakeDamage = false;
            StartCoroutine(DamageCooldown());
        }

        private IEnumerator DamageCooldown()
        {
            yield return new WaitForSeconds(damageInterval);
            canTakeDamage = true;
        }

        private void Die()
        {
            EnemyManager.Instance.RemoveEnemy(gameObject);
            Died?.Invoke();
        }

        private void UpdateHPBar()
        {
            if (hpFillImage == null || maxHealth <= 0) return;

            hpFillImage.fillAmount = (float)health / maxHealth;
        }
    }
}