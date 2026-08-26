using UnityEngine;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// カード使用時の処理を実行・管理するクラス
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
        /// カードを実行する関数
        /// </summary>
        /// <param name="card">使用するカードのデータ</param>
        /// <param name="context">カード使用に必要なコンテキスト情報</param>
        /// <returns>実行に成功した場合はtrue、失敗した場合はfalse</returns>
        public static bool Execute(CardData card, CardUseContext context)
        {
            // 不足している参照を自動取得する関数を呼び出す
            ResolveDependencies(context);

            // もし体力管理クラスが存在しない場合
            if (context.TreeHealthClass == null)
            {
                return false;
            }

            // カード使用時のSEを再生
            AudioSetting.Instance.CardSE(card.usedSE);

            // カードタイプに応じた処理の分岐
            switch (card.cardType)
            {
                case CardType.Attack:

                    // 攻撃カードの処理を実行する関数を呼び出す
                    ExecuteAttack(card, context.ExecuteCardTargetTransform);

                    break;

                case CardType.Recovery:

                    // 回復カードの処理を実行する関数を呼び出す
                    ExecuteRecovery(card, context.TreeHealthClass, context.PlayerCostClass);

                    break;

                case CardType.Support:

                    // サポートカードの処理を実行する関数を呼び出す
                    ExecuteSupport(card, context.TreeHealthClass);

                    break;

                case CardType.Utility:

                    // 実用カードの処理を実行する関数を呼び出す
                    ExecuteUtility(card, context.BattleCardManagerClass);

                    break;

                default:

                    return false;
            }

            return true;
        }

        /// <summary>
        /// コンテキスト内で不足している参照を補完する関数
        /// </summary>
        /// <param name="context">カード使用に必要なコンテキスト情報</param>
        private static void ResolveDependencies(CardUseContext context)
        {
            // もし体力管理クラスが空の場合
            if (context.TreeHealthClass == null)
            {
                // 該当のクラスを探す
                context.TreeHealthClass = GameObject.FindAnyObjectByType<TreeHealth>();
            }

            // もしコスト管理クラスが空の場合
            if (context.PlayerCostClass == null)
            {
                // 該当のクラスを探す
                context.PlayerCostClass = GameObject.FindAnyObjectByType<PlayerCost>();
            }

            // もし戦闘カードマネージャーが空の場合
            if (context.BattleCardManagerClass == null)
            {
                // 該当のクラスを探す
                context.BattleCardManagerClass = GameObject.FindAnyObjectByType<BattleCardManager>();
            }
        }

        /// <summary>
        /// 攻撃カードの処理を行う関数
        /// </summary>
        /// <param name="card">使用するカードのデータ</param>
        /// <param name="origin">攻撃の発生起点となる座標</param>
        private static void ExecuteAttack(CardData card, Vector3 origin)
        {
            // 攻撃カードのパラメーターを参照する変数を定義
            var attack = card.attackParams;

            // もし単体攻撃以外の場合
            if (card.effectType != CardEffectType.DamageSingle)
            {
                // カード使用時のエフェクトを再生する関数を呼び出す
                StageScene.Instance.PlayCardEffect(attack.attackEffect, card.useDuration);
            }

            // エフェクトタイプに応じた処理の分岐
            switch (card.effectType)
            {
                case CardEffectType.DamageSingle:

                    // 最も近い敵1体にダメージを与える関数を呼び出す
                    PlayerAttack.AttackNearest(origin, attack.damage, card.useDuration, attack.attackEffect);

                    break;

                case CardEffectType.DamageLine:

                    // 前方直線範囲の敵にダメージを与える関数を呼び出す
                    PlayerAttack.AttackLine(origin, attack.damage, attack.lineWidth, attack.lineLength, card.useDuration);

                    break;

                case CardEffectType.DamageArea:

                    // 円範囲の敵にダメージを与える関数を呼び出す
                    PlayerAttack.AttackCircle(origin, attack.areaRadius, attack.areaRange, attack.damage, card.useDuration);

                    break;

                case CardEffectType.DamageAllOnScreen:

                    // 画面内すべての敵にダメージを与える関数を呼び出す
                    PlayerAttack.AttackAll(attack.damage, card.useDuration);

                    break;

                case CardEffectType.DamageSingleAreaStop:

                    // 単体の敵を中心に円形ダメージを与えて敵の動きを止める関数を呼び出す
                    PlayerAttack.AttackSingleAreaStop(origin, attack.areaRadius, attack.damage, card.useDuration, attack.stopDuration, attack.attackEffect);

                    break;

                default:

                    break;
            }
        }

        /// <summary>
        /// 回復カードの処理を行う関数
        /// </summary>
        /// <param name="card">使用するカードのデータ</param>
        /// <param name="playerHealth">プレイヤーの体力管理クラス</param>
        /// <param name="playerCost">プレイヤーのコスト管理クラス</param>
        private static void ExecuteRecovery(CardData card, TreeHealth playerHealth, PlayerCost playerCost)
        {
            // 回復カードのパラメーターを参照する変数を定義
            var recover = card.recoverParams;

            // カード使用時のエフェクトを再生する関数を呼び出す
            StageScene.Instance.PlayCardEffect(recover.SupportEffect, card.useDuration);

            // もし体力管理クラスまたはコスト管理クラスが存在しない場合
            if (playerHealth == null || playerCost == null)
            {
                return;
            }

            // エフェクトタイプに応じた処理の分岐
            switch (card.effectType)
            {
                case CardEffectType.Heal:

                    // プレイヤーのHPを回復する関数を呼び出す
                    playerHealth.Heal(recover.healAmount);
                    // 回復エフェクトを表示するUI管理クラスの関数を呼び出す
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.heal_EffectNumber);

                    break;

                case CardEffectType.CostRecover:

                    // コストを即時回復する関数を呼び出す
                    playerCost.RecoverCost(recover.costRecoverAmount);
                    // 回復エフェクトを表示するUI管理クラスの関数を呼び出す
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.heal_EffectNumber);

                    break;

                case CardEffectType.CostRegen:

                    // コスト回復速度を一定時間強化する関数を呼び出す
                    playerCost.ReduceRecoverInterval(recover.intervalReduction, card.buffDuration);
                    // 回復エフェクトを表示するUI管理クラスの関数を呼び出す
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.heal_EffectNumber);

                    break;

                default:

                    break;
            }
        }

        /// <summary>
        /// サポートカードの処理を行う関数
        /// </summary>
        /// <param name="card">使用するカードのデータ</param>
        /// <param name="playerHealth">プレイヤーの体力管理クラス</param>
        private static void ExecuteSupport(CardData card, TreeHealth playerHealth)
        {
            // サポートカードのパラメーターを参照する変数を定義
            var support = card.supportParams;

            // カード使用時のエフェクトを再生する関数を呼び出す
            StageScene.Instance.PlayCardEffect(support.SupportEffect, card.useDuration);

            // もし体力管理クラスが存在しない場合
            if (playerHealth == null)
            {
                return;
            }

            // エフェクトタイプに応じた処理の分岐
            switch (card.effectType)
            {
                case CardEffectType.DamageReduction:

                    // 一定時間ダメージを軽減する関数を呼び出す
                    playerHealth.ApplyDamageReduction(support.damageReduction, card.buffDuration);
                    // シールドエフェクトを表示するUI管理クラスの関数を呼び出す
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.shieldEffectNumber);

                    break;

                case CardEffectType.BuffNext:

                    // 次の攻撃のダメージ倍率を強化する関数を呼び出す
                    PlayerAttack.SetNextAttackMultiplier(support.attackMultiplier);
                    // バフエフェクトを表示するUI管理クラスの関数を呼び出す
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.buffEffectNumber);

                    break;

                default:

                    break;
            }
        }

        /// <summary>
        /// ユーティリティカードの処理を行う関数
        /// </summary>
        /// <param name="card">使用するカードのデータ</param>
        /// <param name="cardManager">戦闘カードマネージャー</param>
        private static void ExecuteUtility(CardData card, BattleCardManager cardManager)
        {
            // 実用カードのパラメーターを参照する変数を定義
            var utility = card.utilityParams;

            // カード使用時のエフェクトを再生する関数を呼び出す
            StageScene.Instance.PlayCardEffect(utility.SupportEffect, card.useDuration);

            // もし戦闘カードマネージャーが存在しない場合
            if (cardManager == null)
            {
                return;
            }

            // エフェクトタイプに応じた処理の分岐
            switch (card.effectType)
            {
                case CardEffectType.Draw:

                    // カードをドローする関数を呼び出す
                    cardManager.DrawCards(utility.drawCount);
                    // バフエフェクトを表示するUI管理クラスの関数を呼び出す
                    SupportEffectUI_Manager.Instance.ShowSupportEffectUI(SupportEffectUI_Manager.Instance.buffEffectNumber);

                    break;

                default:

                    break;
            }
        }
    }
}