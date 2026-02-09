using UnityEngine;

// Criticalに関するEnum
public enum CriticalType
{
    Normal,          // 通常
    Critical,        // 黄クリ
    SuperCritical,   // 橙クリ (今回ロジック対応)
    HyperCritical    // 赤クリ (今回ロジック対応)
}

// 「ダメージの発生源」として、攻撃の威力を計算し、当たり判定を制御するクラス
public class DamageSource : MonoBehaviour
{
    // 持ち主のStatusManagerを保持する変数
    private StatusManager ownerStatus;

    [Header("Attack Specs")]
    [SerializeField] private float damageMultiplier = 1.0f;
    [SerializeField] private float criticalMultiplier = 2.0f;

    // 💡 親を探す処理（近接攻撃などのため）
    void Start()
    {
        // まだ持ち主が登録されていなければ、親から探す
        if (ownerStatus == null)
        {
            ownerStatus = GetComponentInParent<StatusManager>();
        }
    }

    // 飛び道具（レーザー）用：生成時に外（EnemyActionEscapeなど）から教える
    public void Initialize(StatusManager owner)
    {
        this.ownerStatus = owner;
    }

    // 💡 当たり判定のメイン処理
    private void OnTriggerEnter(Collider other)
    {
        // 1. そもそも自分が誰に撃たれたか（ownerStatus）が分からなければ無視
        if (ownerStatus == null) return;

        // 2. 【重要】当たった相手が自分と同じ「タグ」なら、味方とみなして無視する
        // プレイヤーが撃った弾ならPlayerタグを無視、エネミーが撃った弾ならEnemyタグを無視！
        if (other.CompareTag(ownerStatus.tag))
        {
            return;
        }

        // 3. 当たった相手に「StatusManager」がついているか確認（ダメージを与えられる相手か）
        StatusManager targetStatus = other.GetComponent<StatusManager>();
        if (targetStatus != null)
        {
            // クリティカルの種類を受け取るための変数
            CriticalType type;
            // ダメージ計算を実行
            int damage = CalculateDamage(out type);

            // 相手のStatusManagerにダメージを伝える（メソッド名はプロジェクトに合わせてな）
            // もしTakeDamageメソッドがなければ、ここで targetStatus.HP -= damage; などを書く
            Debug.Log($"{ownerStatus.tag}の攻撃が{other.tag}にヒット！ ダメージ：{damage} ({type})");

            // 4. 当たったらレーザー（自分自身）を消す
            Destroy(gameObject);
        }
    }

    // 持ち主の場所を教えるプロパティ
    public Transform OwnerTransform
    {
        get
        {
            if (ownerStatus != null)
            {
                return ownerStatus.transform;
            }
            else
            {
                return this.transform; // 持ち主がいなければ自分（罠など）
            }
        }
    }

    // クリティカルと最終ダメージを計算するメソッド
    public int CalculateDamage(out CriticalType type)
    {
        type = CriticalType.Normal; // 必ず初期値を設定

        // 持ち主がいない場合の安全策
        if (ownerStatus == null) return 0;

        float finalDamage = ownerStatus.CurrentAttack * damageMultiplier;

        float remainingRate = ownerStatus.CurrentCritRate;
        int critCount = 0;

        // 100%を超える確率を考慮してループ処理
        while (remainingRate > 0f)
        {
            if (remainingRate >= 1.0f)
            {
                critCount++; // 100%分確定
                remainingRate -= 1.0f;
            }
            else
            {
                // 端数の確率判定
                if (Random.value < remainingRate) critCount++;
                break;
            }
        }

        // Crit回数に応じてタイプと倍率を決定
        switch (critCount)
        {
            case 0:
                type = CriticalType.Normal;
                break;
            case 1:
                type = CriticalType.Critical;
                finalDamage *= criticalMultiplier;
                break;
            case 2:
                type = CriticalType.SuperCritical;
                finalDamage *= criticalMultiplier * 2f;
                break;
            default: // 3回以上
                type = CriticalType.HyperCritical;
                finalDamage *= criticalMultiplier * 3f;
                break;
        }

        return Mathf.RoundToInt(finalDamage);
    }
}