using UnityEngine;
using System.Collections.Generic;
using ForestDraw.Combat;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// プレイヤーの攻撃処理をまとめたクラス
    /// TargetFinderで敵を取得し、IDamageableへダメージを与える
    /// </summary>
    public static class PlayerAttack
    {
        /// <summary>
        /// 一番近い敵1体にダメージを与える（単体攻撃）
        /// </summary>
        public static void AttackNearest(Vector3 origin, int damage)
        {
            var target = TargetFinder.FindNearest(origin);

            target?.TakeDamage(damage);
        }

        /// <summary>
        /// プレイヤー前方の直線範囲にいる敵にダメージ（直線攻撃）
        /// </summary>
        public static void AttackLine(Vector3 origin, float width, int damage,float length)
        {
            var targets = TargetFinder.FindLine(origin, width, length);

            foreach (var target in targets)
            {
                target.TakeDamage(damage);
            }
        }

        /// <summary>
        /// 指定半径内の敵すべてにダメージ（円範囲攻撃）
        /// </summary>
        public static void AttackCircle(Vector3 origin, float radius, int damage)
        {
            var targets = TargetFinder.FindCircle(origin, radius);

            foreach (var target in targets)
            {
                target.TakeDamage(damage);
            }
        }

        /// <summary>
        /// すべての敵にダメージ（全体攻撃）
        /// </summary>
        public static void AttackAll(int damage)
        {
            var targets = TargetFinder.FindAll();

            foreach (var target in targets)
            {
                target.TakeDamage(damage);
            }
        }
    }
}