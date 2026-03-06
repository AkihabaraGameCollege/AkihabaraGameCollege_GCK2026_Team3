using UnityEngine;
using UnityEngine.UI;
using ForestDraw.Player.Combat;
using ForestDraw.Combat;

namespace ForestDraw
{
    /// <summary>
    /// デッキに登録したカードを表示するUIクラス
    /// </summary>
    public class HandCardUI : MonoBehaviour
    {
        /// <summary>
        /// カードのイラストを表示する画像の変数
        /// </summary>
        [SerializeField]
        private Image cardImage;

        /// <summary>
        /// ボタンコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Button clickButton;

        /// <summary>
        /// カードのデータを保持する変数
        /// </summary>
        private CardData myCardData;

        private PlayerCost playerCost;

        private GameObject target;
        private AttackParams attack;
        private RecoverParams recover;
        private SupportParams support;

        /// <summary>
        /// セットアップの関数
        /// </summary>
        /// <param name="data"></param>
        public void Setup(CardData data, GameObject gameObject)
        {
            myCardData = data;
            target = gameObject;
            playerCost = target.GetComponent<PlayerCost>();

            attack = myCardData.attackParams;
            recover = myCardData.recoverParams;
            support = myCardData.supportParams;

            // もしカードのイラストが存在する場合
            if (cardImage != null && data.cardImage != null)
            {
                cardImage.sprite = data.cardImage;// カードのイラストをUIにセット
            }

            // クリックした時の処理を登録
            clickButton.onClick.RemoveAllListeners();// 念のためリセット
            clickButton.onClick.AddListener(OnUseCard);// カードを使用する関数を登録
        }

        /// <summary>
        /// 使うカードをクリックしたときの処理
        /// </summary>
        private void OnUseCard()
        {
            Debug.Log(myCardData.cardName + " を使用しました！");
            // カードタイプに応じて表示
            switch (myCardData.cardType)
            {
                case CardType.Attack:
                    Attack();
                    break;

                case CardType.Recovery:
                    Recovery();
                    break;

                case CardType.Support:
                    Support();
                    break;
            }
        }
        private void Attack()
        {
            switch (myCardData.effectType)
            {
                case CardEffectType.DamageSingle:

                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(attack.damage + "単体攻撃");
                    PlayerAttack.AttackNearest(target.transform.position, attack.damage);
                    break;

                case CardEffectType.DamageLine:
                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(attack.damage + "直線攻撃");
                    PlayerAttack.AttackLine(target.transform.position, attack.lineWidth, attack.damage, attack.lineLength);
                    break;

                case CardEffectType.DamageArea:
                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(attack.damage + "円形攻撃");
                    PlayerAttack.AttackCircle(target.transform.position + new Vector3(0, 0, 30), attack.areaRadius, attack.damage);
                    break;

                case CardEffectType.DamageAllOnScreen:
                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(attack.damage + "全体攻撃");
                    PlayerAttack.AttackAll(attack.damage);
                    break;
            }
        }
        private void Recovery()
        {
            switch (myCardData.effectType)
            {
                case CardEffectType.Heal:

                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(recover.healAmount + "HP回復");
                    target.GetComponent<IHealable>()?.Heal(recover.healAmount);
                    break;

                case CardEffectType.CostRecover:
                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(recover.costRecoverAmount + "コスト回復");
                    playerCost.RecoverCost(recover.costRecoverAmount);
                    break;

                case CardEffectType.CostRegen:
                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(recover.intervalReduction + "ごとにコスト回復");
                    playerCost.ReduceRecoverInterval(recover.intervalReduction, myCardData.buffDuration);
                    break;
            }
        }
        private void Support()
        {
            switch (myCardData.effectType)
            {
                case CardEffectType.DamageReduction:

                    if (!playerCost.UseCost(myCardData.cost)) return;
                    Debug.Log(support.damageReduction + "ダメージ軽減");
                    target.GetComponent<PlayerHealth>()?.ApplyDamageReduction(support.damageReduction, myCardData.buffDuration);
                    break;
            }
        }
    }
}