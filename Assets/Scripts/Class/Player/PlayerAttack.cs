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
        public static void AttackNearest(Vector3 origin, int damage, float duration, GameObject effect)
        {
            var target = TargetFinder.FindNearest(origin, effect, duration);

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
                DealDamageOverTime(target, finalDamage, duration);
            }
        }

        /// <summary>
        /// 指定半径内の敵すべてにダメージ（円範囲攻撃）
        /// </summary>
        public static void AttackCircle(Vector3 origin, float radius, float forward, int damage, float duration)
        {
            var targets = TargetFinder.FindCircle(origin, radius, forward);
            int finalDamage = ApplyMultiplier(damage);

            foreach (var target in targets)
            {
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
                DealDamageOverTime(target, finalDamage, duration);
            }
        }

        /// <summary>
        /// 一番近い敵1体を中心に円形ダメージを与えて敵の動きを止める
        /// </summary>
        public static void AttackSingleAreaStop(Vector3 origin, float radius, int damage, float duration, float stopDuration, GameObject effect)
        {
            // 一番近い敵を取得（ターゲットの中心点）
            var centerTarget = TargetFinder.FindNearest(origin, effect, duration);

            // 敵が見つからなければ処理を終了
            if (centerTarget == null) return;

            // 中心となる敵の座標を取得する
            Vector3 centerPosition = ((Component)centerTarget).transform.position;

            // その座標を中心に円形範囲にいる敵を取得
            var targets = TargetFinder.FindCircle(centerPosition, radius, 0f);

            int finalDamage = ApplyMultiplier(damage);

            // 範囲内の敵全員にダメージと停止効果を与える
            foreach (var target in targets)
            {
                // 徐々にダメージを与える（既存機能）
                DealDamageOverTime(target, finalDamage, duration);

                // 動きを止める処理を適用（後述のメソッドを呼び出す）
                ApplyStopEffect(target, stopDuration);
            }
        }

        /// <summary>
        /// 敵の動きを止める処理
        /// </summary>
        private static void ApplyStopEffect(IDamageable target, float duration)
        {
            // 対象が「動きを止められる」インターフェースを持っていれば実行する
            if (target is IStoppable stoppable)
            {
                stoppable.StopMovement(duration);
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