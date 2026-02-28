using UnityEngine;

/// <summary>
/// 敵の攻撃処理の基底クラス。
/// 実際の攻撃内容は継承先クラスで実装する。
/// </summary>
public abstract class EnemyAttackBase : MonoBehaviour
{
    // =========================
    // 設定値
    // =========================

    protected float attackInterval = 1.5f; // 攻撃間隔（秒）
    protected int attackDamage = 10;       // 攻撃ダメージ量

    // =========================
    // 内部状態管理用
    // =========================

    protected float attackTimer;   // 次の攻撃までの経過時間
    protected bool canAttack = false; // 攻撃可能状態かどうか
    protected GameObject target;   // 攻撃対象

    // =========================
    // 外部から攻撃対象を設定
    // =========================
    public void SetTarget(GameObject t)
    {
        target = t;
    }
    // =========================
    // 外部から移動パラメータを設定
    // =========================
    public void Initialize(EnemyData data)
    {
        attackDamage = data.attackDamage;
        attackInterval = data.attackInterval;
    }

    // =========================
    // 初期処理
    // =========================
    protected virtual void Start()
    {
        // EnemyMoveのゴール到達イベントを購読
        // ゴール到達時に攻撃を開始する
        EnemyMove move = GetComponent<EnemyMove>();
        move.OnReachGoal += StartAttack;
    }

    // =========================
    // 毎フレーム処理
    // =========================
    protected virtual void Update()
    {
        // 攻撃可能状態でなければ何もしない
        if (!canAttack) return;

        // 経過時間を加算
        attackTimer += Time.deltaTime;

        // 攻撃間隔を超えたら攻撃実行
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;   // タイマーをリセット
            PerformAttack();    // 継承先の攻撃処理
        }
    }

    // =========================
    // 攻撃開始処理
    // =========================
    protected void StartAttack()
    {
        // 攻撃可能状態にする
        canAttack = true;
    }

    // =========================
    // 継承先の攻撃処理
    // =========================
    protected abstract void PerformAttack();
}