using UnityEngine;

namespace ForestDraw
{
    [CreateAssetMenu(menuName = "Game/CardData", fileName = "CardData")]
    public class CardData : ScriptableObject
    {
        public string cardId;
        public string cardName;
        public CardType cardType;         // 攻撃 / 防御 / サポート
        public CardEffectType effectType; // ダメージ系 / 回復系 など

        [TextArea] public string description;
        public int cost;
        public Sprite cardImage;
        public int buffDuration;

        // 効果別の値
        public AttackParams attackParams;
        public RecoverParams recoverParams;
        public SupportParams supportParams;
        private void OnEnable()
        {
            if (attackParams == null) attackParams = new AttackParams();
            if (supportParams == null) supportParams = new SupportParams();
            if (recoverParams == null) recoverParams = new RecoverParams();
        }
    }

    // 攻撃カード用パラメータ
    [System.Serializable]
    public class AttackParams
    {
        public int damage;
        public float lineLength;
        public float lineWidth;
        public float areaRadius;
    }

    // 回復カード用パラメータ
    [System.Serializable]
    public class RecoverParams
    {
        public int healAmount;
        public int costRecoverAmount;
        public int intervalReduction;
    }

    // サポートカード用パラメータ
    [System.Serializable]
    public class SupportParams
    {
        public float damageReduction;
    }
}