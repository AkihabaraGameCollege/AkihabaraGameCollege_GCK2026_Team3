using ForestDraw.Combat;
using ForestDraw.Enemy.Attack;
using ForestDraw.Player.Combat;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw.Enemy.Components
{
    /// <summary>
    /// 敵のHPを管理するコンポーネント。
    /// ダメージ処理、無敵時間制御、死亡通知を行う。
    /// </summary>
    public class EnemyHealth : MonoBehaviour, IDamageable, IEnemyComponent, IStoppable
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
        /// ダメージポップアップ管理クラスを参照する変数（中山が追加）
        /// </summary>
        [SerializeField]
        private DamagePopup _damagePopup;

        /// <summary>
        /// HPが0になったときに発火する
        /// </summary>
        public event Action Died;

        /// <summary>
        /// ダメージを受けた時のSEインデックスを参照する変数
        /// </summary>
        private int damageSE_Index = 6;

        private bool isStopped = false;

        // EnemyMove スクリプトの参照を保持する変数
        private EnemyMove enemyMove;

        private EnemyAttackGoal enemyAttackGoal;

        private void Awake()
        {
            // 同じオブジェクトについている EnemyMove を取得
            enemyMove = GetComponent<EnemyMove>();
            // 同じオブジェクトについている EnemyAttackGoal を取得
            enemyAttackGoal = GetComponent<EnemyAttackGoal>();
        }

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

            AudioSetting.Instance.PlaySE(damageSE_Index);// ダメージを受けるSEを再生

            health = Mathf.Max(health - amount, 0);
            UpdateHPBar();

            // --- ダメージポップアップ（中山が追加） ---
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
            var popup = Instantiate(_damagePopup, spawnPos, Quaternion.identity);
            popup.Setup(amount);

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
            EnemyManager.Instance.EnemyDiedProcess();
            EnemyManager.Instance.RemoveEnemy(gameObject);
            Died?.Invoke();
        }

        private void UpdateHPBar()
        {
            if (hpFillImage == null || maxHealth <= 0) return;
            hpFillImage.fillAmount = (float)health / maxHealth;
        }

        // --- IStoppable の実装 ---
        public void StopMovement(float duration)
        {
            // 既に止まっている場合は処理しない（上書きしたい場合はここを調整できます）
            if (!isStopped)
            {
                StartCoroutine(StopCoroutine(duration));
            }
        }

        private IEnumerator StopCoroutine(float duration)
        {
            isStopped = true;

            // EnemyMove が取得できていれば移動を一時停止
            if (enemyMove != null&& enemyAttackGoal != null)
            {
                enemyMove.PauseMove();
                enemyAttackGoal.StopAttack();
            }

            // 指定された時間（duration）待機する
            yield return new WaitForSeconds(duration);

            // 待機後、EnemyMove で移動を再開
            if (enemyMove != null && enemyAttackGoal != null)
            {
                enemyMove.ResumeMove();
                enemyAttackGoal.EnableAttack();
            }

            isStopped = false;
        }
    }
}