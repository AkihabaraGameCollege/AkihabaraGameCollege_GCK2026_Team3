using UnityEngine;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// カード使用時の処理を管理するクラス
    /// </summary>
    public static class CardUseExecutor
    {
        /// <summary>
        /// カード使用時のコンテキスト情報をまとめるクラス
        /// </summary>
        public class CardUseContext
        {
            /// <summary>
            /// プレイヤーのコスト管理クラスを参照する変数
            /// </summary>
            public PlayerCost PlayerCostClass;
            /// <summary>
            /// プレイヤーの体力管理クラスを参照する変数
            /// </summary>
            public TreeHealth TreeHealthClass;
            /// <summary>
            /// 戦闘カードマネージャーを参照する変数
            /// </summary>
            public BattleCardManager BattleCardManagerClass;
            /// <summary>
            /// カード使用対象の位置を示す変数
            /// </summary>
            public Vector3 ExecuteCardTargetTransform;
        }

        /// <summary>
        /// カードを実行する
        /// </summary>
        public static bool Execute(CardData card, CardUseContext context)
        {
            if (context.TreeHealthClass == null)
            {
                // シーン内から TreeHealth コンポーネントを探して割り当てる
                context.TreeHealthClass = GameObject.FindAnyObjectByType<TreeHealth>();

                if (context.TreeHealthClass == null)
                {
                    return false;
                }
            }

            AudioSetting.Instance.CardSE(card.usedSE);
            
            switch (card.cardType)
            {
                case CardType.Attack:
                    ExecuteAttack(card, context.ExecuteCardTargetTransform);
                    break;

                case CardType.Recovery:
                    ExecuteRecovery(card, context.TreeHealthClass, context.PlayerCostClass);
                    break;

                case CardType.Support:
                    ExecuteSupport(card, context.TreeHealthClass);
                    break;

                case CardType.Utility:
                    ExecuteUtility(card, context.BattleCardManagerClass);
                    break;
            }

            return true;
        }

        /// <summary>
        /// 攻撃カードの処理
        /// </summary>
        private static void ExecuteAttack(CardData card, Vector3 origin)
        {
            var attack = card.attackParams;
                    if (card.effectType != CardEffectType.DamageSingle) StageScene.Instance.PlayCardEffect(attack.attackEffect, card.useDuration);

            switch (card.effectType)
            {
                case CardEffectType.DamageSingle:
                    // 最も近い敵1体にダメージ
                    PlayerAttack.AttackNearest(origin, attack.damage, card.useDuration, attack.attackEffect);
                    break;

                case CardEffectType.DamageLine:
                    // 前方直線範囲の敵にダメージ
                    PlayerAttack.AttackLine(origin, attack.damage, attack.lineWidth, attack.lineLength, card.useDuration);
                    break;

                case CardEffectType.DamageArea:
                    // 円範囲の敵にダメージ
                    PlayerAttack.AttackCircle(origin, attack.areaRadius, attack.areaRange, attack.damage, card.useDuration);
                    break;

                case CardEffectType.DamageAllOnScreen:
                    // 画面内すべての敵にダメージ
                    PlayerAttack.AttackAll(attack.damage, card.useDuration);
                    break;

                case CardEffectType.DamageSingleAreaStop:
                    // 単体の敵を中心に円形ダメージを与えて敵の動きを止める
                    PlayerAttack.AttackSingleAreaStop(origin, attack.areaRadius, attack.damage, card.useDuration, attack.stopDuration, attack.attackEffect);
                    break;
            }
        }

        /// <summary>
        /// 回復カードの処理
        /// </summary>
        private static void ExecuteRecovery(
            CardData card,
            TreeHealth playerHealth,
            PlayerCost playerCost)
        {
            var recover = card.recoverParams;
            StageScene.Instance.PlayCardEffect(recover.SupportEffect, card.useDuration);

            switch (card.effectType)
            {
                case CardEffectType.Heal:
                    // プレイヤーHP回復
                    playerHealth.Heal(recover.healAmount);
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.heal_EffectNumber);// 回復エフェクト表示
                    break;

                case CardEffectType.CostRecover:
                    // コストを即時回復
                    playerCost.RecoverCost(recover.costRecoverAmount);
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.heal_EffectNumber);// 回復エフェクト表示
                    break;

                case CardEffectType.CostRegen:
                    // コスト回復速度を一定時間強化
                    playerCost.ReduceRecoverInterval(recover.intervalReduction, card.buffDuration);
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.heal_EffectNumber);// 回復エフェクト表示
                    break;
            }
        }

        /// <summary>
        /// サポートカードの処理
        /// </summary>
        private static void ExecuteSupport(
            CardData card,
            TreeHealth playerHealth)
        {
            var support = card.supportParams;
            StageScene.Instance.PlayCardEffect(support.SupportEffect, card.useDuration);

            switch (card.effectType)
            {
                case CardEffectType.DamageReduction:
                    // 一定時間ダメージ軽減
                    playerHealth.ApplyDamageReduction(support.damageReduction, card.buffDuration);
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.shieldEffectNumber);// シールドエフェクト表示
                    break;

                case CardEffectType.BuffNext:
                    // 次の攻撃のダメージ倍率を強化
                    PlayerAttack.SetNextAttackMultiplier(support.attackMultiplier);
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.buffEffectNumber);// バフエフェクト表示
                    break;
            }
        }
        /// <summary>
        /// ユーティリティカードの処理
        /// </summary>
        private static void ExecuteUtility(
            CardData card,
            BattleCardManager cardManager)
        {
            var utility = card.utilityParams;
            StageScene.Instance.PlayCardEffect(utility.SupportEffect, card.useDuration);

            switch (card.effectType)
            {
                case CardEffectType.Draw:
                    // カードをドロー
                    cardManager.DrawCards(utility.drawCount);
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.buffEffectNumber);// バフエフェクト表示
                    break;
            }
        }
    }
}