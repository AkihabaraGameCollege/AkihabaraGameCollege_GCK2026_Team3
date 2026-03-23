using ForestDraw.Combat;
using UnityEngine;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// プレイヤーの攻撃処理をまとめたクラス
    /// TargetFinderで敵を取得し、IDamageableへダメージを与える
    /// </summary>
    public static class PlayerAttack
    {
        /// <summary>
        /// 次の攻撃に適用するダメージ倍率（初期値は1倍）
        /// </summary>
        private static float nextAttackMultiplier = 1f;
        /// <summary>
        /// 一番近い敵1体にダメージを与える（単体攻撃）
        /// </summary>
        public static void AttackNearest(Vector3 origin, int damage, float duration)
        {
            var target = TargetFinder.FindNearest(origin);

            Debug.Log("単体攻撃");
            int finalDamage = ApplyMultiplier(damage);
            DealDamageOverTime(target, finalDamage, duration);
        }

        /// <summary>
        /// プレイヤー前方の直線範囲にいる敵にダメージ（直線攻撃）
        /// </summary>
        public static void AttackLine(Vector3 origin, int damage, float width, float length, float duration)
        {
            var targets = TargetFinder.FindLine(origin, width, length);
            int finalDamage = ApplyMultiplier(damage);

            foreach (var target in targets)
            {
                Debug.Log("直線攻撃");
                DealDamageOverTime(target, finalDamage, duration);
            }
        }

        /// <summary>
        /// 指定半径内の敵すべてにダメージ（円範囲攻撃）
        /// </summary>
        public static void AttackCircle(Vector3 origin, float radius, int damage, float duration)
        {
            var targets = TargetFinder.FindCircle(origin, radius);
            int finalDamage = ApplyMultiplier(damage);

            foreach (var target in targets)
            {
                Debug.Log("円形範囲攻撃");
                DealDamageOverTime(target, finalDamage, duration);
            }
        }

        /// <summary>
        /// すべての敵にダメージ（全体攻撃）
        /// </summary>
        public static void AttackAll(int damage, float duration)
        {
            var targets = TargetFinder.FindAll();
            int finalDamage = ApplyMultiplier(damage);

            foreach (var target in targets)
            {
                Debug.Log("全体攻撃");
                DealDamageOverTime(target, finalDamage, duration);
            }
        }

        /// <summary>
        /// 次の攻撃のダメージ倍率を設定する
        /// </summary>
        public static void SetNextAttackMultiplier(float multiplier)
        {
            nextAttackMultiplier = multiplier;
        }

        /// <summary>
        /// ダメージに倍率を適用し、適用後は倍率をリセットする
        /// </summary>
        private static int ApplyMultiplier(int damage)
        {
            Debug.Log("攻撃が" + nextAttackMultiplier + "倍");
            int result = Mathf.RoundToInt(damage * nextAttackMultiplier);
            nextAttackMultiplier = 1f;
            return result;
        }

        /// <summary>
        /// ダメージを徐々に与える
        /// </summary>
        private static void DealDamageOverTime(IDamageable target, int damage, float duration)
        {
            StageScene.Instance.StartCoroutine(DamageCoroutine(target, damage, duration));
        }

        private static System.Collections.IEnumerator DamageCoroutine(IDamageable target, int totalDamage, float duration)
        {
            int ticks = 10; // 分割数（滑らかさ）
            float interval = duration / ticks;

            int damagePerTick = Mathf.CeilToInt((float)totalDamage / ticks);

            for (int i = 0; i < ticks; i++)
            {
                if (target == null) yield break;

                target.TakeDamage(damagePerTick);
                yield return new WaitForSeconds(interval);
            }
        }
    }
}