using System.Collections.Generic;
using UnityEngine;
using ForestDraw.Combat;
using ForestDraw.Enemy;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// 攻撃対象となる敵を検索するクラス
    /// EnemyManagerに登録されている敵リストを元に
    /// 条件に合うIDamageableを取得する
    /// </summary>
    public static class TargetFinder
    {
        /// <summary>
        /// 指定座標から最も近い敵を1体取得
        /// </summary>
        public static IDamageable FindNearest(Vector3 origin, GameObject effect, float duration)
        {
            GameObject nearest = null;
            float minDist = Mathf.Infinity;

            foreach (var enemy in EnemyManager.instance.Enemies)
            {
                // 距離計算（平方距離で計算してパフォーマンス向上）
                float dist = (enemy.transform.position - origin).sqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }
            StageScene.Instance.PlayCardEffect(effect,nearest.transform, duration);
            // IDamageableを取得して返す
            return nearest?.GetComponent<IDamageable>();
        }

        /// <summary>
        /// プレイヤーの前方(Z方向)にある直線範囲の敵を取得
        /// width = X方向の許容幅
        /// </summary>
        public static List<IDamageable> FindLine(Vector3 origin, float width, float length)
        {
            List<IDamageable> targets = new();

            // ==================================================
            // ▼ DEBUG : Attack Range Visualization
            // ==================================================
#if UNITY_EDITOR

            Vector3 leftStart = origin + new Vector3(-width, 0, 0);
            Vector3 rightStart = origin + new Vector3(width, 0, 0);

            Vector3 leftEnd = leftStart + Vector3.forward * length;
            Vector3 rightEnd = rightStart + Vector3.forward * length;

            Debug.DrawLine(leftStart, leftEnd, Color.red, 1f);
            Debug.DrawLine(rightStart, rightEnd, Color.red, 1f);
            Debug.DrawLine(leftEnd, rightEnd, Color.red, 1f);

#endif
            // ==================================================
            // ▲ DEBUG END
            // ==================================================

            foreach (var enemy in EnemyManager.instance.Enemies)
            {
                Vector3 diff = enemy.transform.position - origin;

                // プレイヤーより前方のにいて、横幅以内にいる敵
                if (diff.z > 0 && diff.z <= length && Mathf.Abs(diff.x) <= width)
                {
                    var d = enemy.GetComponent<IDamageable>();
                    if (d != null) targets.Add(d);
                }
            }

            return targets;
        }

        /// <summary>
        /// 指定半径内の敵をすべて取得（円形範囲攻撃）
        /// </summary>
        public static List<IDamageable> FindCircle(Vector3 origin, float radius, float forward)
        {
            List<IDamageable> targets = new();
            Vector3 center = origin + new Vector3(0,0, forward);
            float radiusSq = radius * radius;

            // ==================================================
            // ▼ DEBUG : Circle Range Visualization
            // ==================================================
#if UNITY_EDITOR

            int segments = 20;
            Vector3 prev = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2 / segments;

                Vector3 next = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                Debug.DrawLine(prev, next, Color.blue, 1f);
                prev = next;
            }

#endif
            // ==================================================
            // ▲ DEBUG END
            // ==================================================

            foreach (var enemy in EnemyManager.instance.Enemies)
            {
                // 半径内にいるか判定
                if ((enemy.transform.position - center).sqrMagnitude <= radiusSq)
                {
                    var d = enemy.GetComponent<IDamageable>();
                    if (d != null) targets.Add(d);
                }
            }

            return targets;
        }

        /// <summary>
        /// すべての敵を取得（全体攻撃用）
        /// </summary>
        public static List<IDamageable> FindAll()
        {
            List<IDamageable> targets = new();

            foreach (var enemy in EnemyManager.instance.Enemies)
            {
                var d = enemy.GetComponent<IDamageable>();
                if (d != null) targets.Add(d);
            }
            Debug.Log(targets+ "allTarget");
            return targets;
        }
    }
}