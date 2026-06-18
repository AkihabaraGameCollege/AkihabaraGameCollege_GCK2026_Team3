using UnityEngine;
using ForestDraw.Combat;

namespace ForestDraw.Enemy.Attack
{
    /// <summary>
    /// 敵の追尾弾クラス。
    /// ・ターゲットに向かって移動する
    /// ・衝突時にダメージを与える
    /// ・ターゲット消滅時は自動で破棄する
    /// </summary>
    public class EnemyBullet : MonoBehaviour
    {
        // ===== 設定値 =====
        private float speed = 10f;   // 弾の移動速度
        private int damage;          // 与えるダメージ
        private Transform target;    // 追尾対象

        /// <summary>
        /// 弾の初期設定を行う
        /// </summary>
        /// <param name="t">ターゲット</param>
        /// <param name="attackdamage">ダメージ量</param>
        /// <param name="s">移動速度</param>
        public void Initialize(Transform t, int attackdamage, float s)
        {
            target = t;
            damage = attackdamage;
            speed = s;
        }

        private void Update()
        {
            // ターゲットが存在しない場合は自身を削除
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // ターゲット方向へ移動
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Playerタグを持つオブジェクトに命中した場合
            if (other.CompareTag("Player"))
            {
                // ダメージを与えて弾を削除
                other.GetComponent<IDamageable>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}