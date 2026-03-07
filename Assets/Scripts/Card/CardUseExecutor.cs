using UnityEngine;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// カード使用時の処理を管理するクラス
    /// カードデータを受け取り、種類に応じた処理を実行する
    /// </summary>
    public static class CardUseExecutor
    {
        /// <summary>
        /// カードを実行する
        /// </summary>
        public static void Execute(CardData card, Transform player)
        {
            // プレイヤー位置を取得（攻撃の基準位置）
            Vector3 origin = player.position;

            // 必要なコンポーネント取得
            if (!player.TryGetComponent<PlayerCost>(out var playerCost)) return;
            if (!player.TryGetComponent<PlayerHealth>(out var playerHealth)) return;

            // コストが足りなければカードを使用できない
            if (!playerCost.UseCost(card.cost)) return;

            // カードタイプごとに処理を分岐
            switch (card.cardType)
            {
                case CardType.Attack:
                    ExecuteAttack(card, origin);
                    break;

                case CardType.Recovery:
                    ExecuteRecovery(card, playerHealth, playerCost);
                    break;

                case CardType.Support:
                    ExecuteSupport(card, playerHealth);
                    break;
            }
        }

        /// <summary>
        /// 攻撃カードの処理
        /// </summary>
        private static void ExecuteAttack(CardData card, Vector3 origin)
        {
            var attack = card.attackParams;

            switch (card.effectType)
            {
                case CardEffectType.DamageSingle:
                    // 最も近い敵1体にダメージ
                    PlayerAttack.AttackNearest(origin, attack.damage);
                    break;

                case CardEffectType.DamageLine:
                    // 前方直線範囲の敵にダメージ
                    PlayerAttack.AttackLine(origin, attack.damage, attack.lineWidth, attack.lineLength);
                    break;

                case CardEffectType.DamageArea:
                    // 円範囲の敵にダメージ
                    PlayerAttack.AttackCircle(origin, attack.areaRadius, attack.damage);
                    break;

                case CardEffectType.DamageAllOnScreen:
                    // 画面内すべての敵にダメージ
                    PlayerAttack.AttackAll(attack.damage);
                    break;
            }
        }

        /// <summary>
        /// 回復カードの処理
        /// </summary>
        private static void ExecuteRecovery(
            CardData card,
            PlayerHealth playerHealth,
            PlayerCost playerCost)
        {
            var recover = card.recoverParams;

            switch (card.effectType)
            {
                case CardEffectType.Heal:
                    // プレイヤーHP回復
                    playerHealth.Heal(recover.healAmount);
                    break;

                case CardEffectType.CostRecover:
                    // コストを即時回復
                    playerCost.RecoverCost(recover.costRecoverAmount);
                    break;

                case CardEffectType.CostRegen:
                    // コスト回復速度を一定時間強化
                    playerCost.ReduceRecoverInterval(recover.intervalReduction, card.buffDuration);
                    break;
            }
        }

        /// <summary>
        /// サポートカードの処理
        /// </summary>
        private static void ExecuteSupport(
            CardData card,
            PlayerHealth playerHealth)
        {
            var support = card.supportParams;

            switch (card.effectType)
            {
                case CardEffectType.DamageReduction:
                    // 一定時間ダメージ軽減
                    playerHealth.ApplyDamageReduction(support.damageReduction, card.buffDuration);
                    break;

                case CardEffectType.BuffNext:
                    // 次の攻撃のダメージ倍率を強化
                    PlayerAttack.SetNextAttackMultiplier(support.attackMultiplier);
                    break;
            }
        }
    }
}