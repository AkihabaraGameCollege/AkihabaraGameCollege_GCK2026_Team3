using System.Collections.Generic;
using UnityEngine;

// 攻撃カード（MonoBehaviourとしてInspectorでパラメータ調整可能）
// 使い方例:
// var card = someAttackCardComponent;
// card.Execute(ownerTransform, singleTargetTransform, playerController, charge: true, lifeSacrifice: 2, lifeSacrificeMultiplier: 1.5f);
public class AttackCard : MonoBehaviour
{
    [Header("基本")]
    [SerializeField] int baseDamage = 5;
    [SerializeField] int cost = 1;
    [SerializeField] bool chargeable = true;
    [Tooltip("チャージ時のダメージ倍率")]
    [SerializeField] float chargeMultiplier = 2.0f;

    [Header("ターゲティング")]
    [Tooltip("true: 単体攻撃（指定ターゲット） / false: 範囲攻撃（線形 + 任意で円形追加）")]
    [SerializeField] bool singleTargetMode = true;

    [Header("線形攻撃(直線範囲)")]
    [Tooltip("直線の長さ")]
    [SerializeField] float linearRange = 8.0f;
    [Tooltip("直線の太さ（SphereCastの半径として使用）")]
    [SerializeField] float linearRadius = 0.5f;

    [Header("追加円形AoE")]
    [Tooltip("直線攻撃に加えて中心点の前方に円形範囲攻撃を行う")]
    [SerializeField] bool addCircularAoE = false;
    [SerializeField] float circularRadius = 3.0f;
    [Tooltip("円形AoEの中心は発射位置の前方にある距離")]
    [SerializeField] float circularOffsetForward = 4.0f;

    [Header("フィルター")]
    [Tooltip("敵レイヤーなどを限定したい場合にセット（未設定ならすべて）")]
    [SerializeField] LayerMask targetLayerMask = ~0;

    // 主な攻撃実行メソッド
    // origin: 発動元のTransform（プレイヤーのTransform等）
    // singleTarget: 単体攻撃時のターゲット（nullなら近傍の最優先を自動選択）
    // owner: プレイヤー側のコントローラ（ライフ消費などに使用）
    // charge: チャージしているかどうか（チャージ可能なカードは倍率が乗る）
    // lifeSacrifice: ライフ消費量（0なら使用しない）
    // lifeSacrificeMultiplier: ライフ消費が成功した場合に乗るダメージ倍率
    public void Execute(Transform origin, Transform singleTarget, PlayerController owner, bool charge = false, int lifeSacrifice = 0, float lifeSacrificeMultiplier = 1.0f)
    {
        if (origin == null || owner == null)
        {
            Debug.LogWarning("AttackCard.Execute: origin や owner が null です。");
            return;
        }

        // 1) チャージ倍率適用
        float finalMultiplier = 1.0f;
        if (charge && chargeable)
        {
            finalMultiplier *= chargeMultiplier;
        }

        // 2) ライフ消費があれば試みる（失敗したらライフ倍率は適用しない）
        if (lifeSacrifice > 0)
        {
            bool consumed = owner.ConsumeLife(lifeSacrifice);
            if (consumed)
            {
                finalMultiplier *= lifeSacrificeMultiplier;
            }
            else
            {
                // ライフ不足 → 消費は行われず、倍率は変わらない
                Debug.Log("AttackCard: ライフ消費に失敗（ライフ不足）");
            }
        }

        // 追加: プレイヤーの持つ「次回攻撃倍率」を取り出して乗算（取り出したらリセット）
        float ownerMultiplier = owner.GetAndConsumeNextAttackMultiplier();
        finalMultiplier *= ownerMultiplier;

        int appliedDamage = Mathf.Max(0, Mathf.RoundToInt(baseDamage * finalMultiplier));

        // 3) ターゲット取得とダメージ適用
        if (singleTargetMode)
        {
            ApplySingleTargetDamage(origin, singleTarget, appliedDamage, owner);
        }
        else
        {
            ApplyAreaDamage(origin, appliedDamage, owner);
        }
    }

    void ApplySingleTargetDamage(Transform origin, Transform singleTarget, int damage, PlayerController owner)
    {
        // 指定ターゲットが null なら、origin から近い StatusManager を探す
        StatusManager targetStatus = null;
        if (singleTarget != null)
        {
            targetStatus = singleTarget.GetComponent<StatusManager>();
        }
        else
        {
            // 近傍探索: small sphere
            Collider[] cols = Physics.OverlapSphere(origin.position, linearRange, targetLayerMask, QueryTriggerInteraction.Collide);
            float bestDist = float.MaxValue;
            foreach (var c in cols)
            {
                var sm = c.GetComponent<StatusManager>();
                if (sm == null) continue;
                float d = Vector3.Distance(origin.position, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    targetStatus = sm;
                }
            }
        }

        if (targetStatus != null)
        {
            targetStatus.Damage(damage, origin.position, CriticalType.Normal, owner.transform);
        }
    }

    void ApplyAreaDamage(Transform origin, int damage, PlayerController owner)
    {
        HashSet<StatusManager> hitSet = new HashSet<StatusManager>();

        // 直線範囲: SphereCastAll を使用して直線上の敵を全て取得（減衰なし）
        Ray ray = new Ray(origin.position, origin.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, linearRadius, linearRange, targetLayerMask, QueryTriggerInteraction.Collide);
        foreach (var h in hits)
        {
            var sm = h.collider.GetComponent<StatusManager>();
            if (sm == null) continue;
            hitSet.Add(sm);
        }

        // 追加円形AoE が有効なら、発射方向前方の一点を中心に OverlapSphere
        if (addCircularAoE)
        {
            Vector3 center = origin.position + origin.forward * circularOffsetForward;
            Collider[] cols = Physics.OverlapSphere(center, circularRadius, targetLayerMask, QueryTriggerInteraction.Collide);
            foreach (var c in cols)
            {
                var sm = c.GetComponent<StatusManager>();
                if (sm == null) continue;
                hitSet.Add(sm);
            }
        }

        // 取得したユニークな敵全てにダメージを与える
        foreach (var sm in hitSet)
        {
            if (sm == null) continue;
            sm.Damage(damage, origin.position, CriticalType.Normal, owner.transform);
        }
    }
}