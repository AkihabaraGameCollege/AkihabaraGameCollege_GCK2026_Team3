using UnityEngine;

/// <summary>
/// 弾クラス
/// ・ターゲットに向かって直進
/// ・衝突時にダメージを与える
/// ・ターゲットが消えたら自動破棄
/// </summary>
public class Bullet : MonoBehaviour
{
    // =========================
    // ▼ 内部パラメータ
    // =========================
    private float speed = 10f;     // 弾の移動速度
    private int damage;            // 与えるダメージ量
    private GameObject target;     // 追尾対象

    // =========================
    // 外部から初期化
    // =========================
    /// <summary>
    /// 弾のステータスを設定する
    /// </summary>
    /// <param name="t">ターゲット</param>
    /// <param name="attackdamage">ダメージ量</param>
    /// <param name="s">移動速度</param>
    public void Initialize(GameObject t, int attackdamage, float s)
    {
        target = t;
        damage = attackdamage;
        speed = s;
    }

    // =========================
    // 毎フレーム処理
    // =========================
    private void Update()
    {
        // ターゲットが消えていたら弾も削除
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // ターゲット方向を計算
        Vector3 dir = (target.transform.position - transform.position).normalized;

        // ターゲットに向かって移動
        transform.position += dir * speed * Time.deltaTime;
    }

    // =========================
    // 衝突判定
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        // Playerタグを持つオブジェクトにヒットした場合
        if (other.CompareTag("Player"))
        {
            // プレイヤーHPにダメージを与える
            other.GetComponent<KariPlayerHealth>()?.TakeDamage(damage);

            // 弾を削除
            Destroy(gameObject);
        }
    }
}