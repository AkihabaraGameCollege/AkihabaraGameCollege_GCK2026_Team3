using UnityEngine;

// プレイヤーのターゲット管理コンポーネント。
// - 他スクリプトから SetTarget / ClearTarget / SelectNearestEnemy を呼べるようにする。
// - シンプルに GameObject.FindGameObjectsWithTag("Enemy") を使って最寄り検索を行う。
public class PlayerTargeting : MonoBehaviour
{
    [SerializeField, Tooltip("現在のターゲット")]
    Transform currentTarget;

    // 現在のターゲットを外から参照できる（読み取り専用）
    public Transform CurrentTarget => currentTarget;

    // ターゲットを設定する
    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    // ターゲットを解除する
    public void ClearTarget()
    {
        currentTarget = null;
    }

    // 指定半径内で最も近い Enemy を選択してターゲットに設定する
    // 見つかれば true、見つからなければ false を返す
    public bool SelectNearestEnemy(float radius)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0) return false;

        Transform nearest = null;
        float bestDistSq = radius * radius;
        Vector3 myPos = transform.position;

        foreach (var go in enemies)
        {
            if (go == null) continue;
            float distSq = (go.transform.position - myPos).sqrMagnitude;
            if (distSq <= bestDistSq)
            {
                if (nearest == null || distSq < (nearest.position - myPos).sqrMagnitude)
                {
                    nearest = go.transform;
                }
            }
        }

        if (nearest != null)
        {
            currentTarget = nearest;
            return true;
        }

        return false;
    }
}