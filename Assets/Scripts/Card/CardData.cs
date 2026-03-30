using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// カードの基本データを保持する ScriptableObject
    /// カードの種類・効果タイプ・パラメータなどを管理する
    /// </summary>
    [CreateAssetMenu(menuName = "Game/CardData", fileName = "CardData")]
    public class CardData : ScriptableObject
    {
        /// <summary>カードID（管理用）</summary>
        public string cardId;

        /// <summary>カード名</summary>
        public string cardName;

        /// <summary>カードの種類（攻撃 / 回復 / サポートなど）</summary>
        public CardType cardType;

        /// <summary>カードの効果タイプ</summary>
        public CardEffectType effectType;

        /// <summary>カード説明文</summary>
        [TextArea] public string description;

        /// <summary>使用コスト</summary>
        public int cost;

        /// <summary>カードの使用時間</summary>
        public float useDuration;

        /// <summary>カード画像</summary>
        public Sprite cardImage;
        /// <summary>カード詳細の画像</summary>（中山が編集）
        public Sprite cardDetail_Image;

        /// <summary>カード使用時のSE</summary>
        public AudioClip usedSE;

        /// <summary>カード使用時のエフェクト</summary>
        public GameObject usedEffect;

        /// <summary>バフ系効果の持続時間</summary>
        public int buffDuration;

        /// <summary>攻撃カード用のパラメータ</summary>
        public AttackParams attackParams;

        /// <summary>回復カード用のパラメータ</summary>
        public RecoverParams recoverParams;

        /// <summary>サポートカード用のパラメータ</summary>
        public SupportParams supportParams;

        /// <summary>サポートカード用のパラメータ</summary>
        public UtilityParams utilityParams;

        /// <summary>
        /// ScriptableObject読み込み時に各パラメータを初期化
        /// nullの場合のみ生成する
        /// </summary>
        private void OnEnable()
        {
            attackParams ??= new AttackParams();
            supportParams ??= new SupportParams();
            recoverParams ??= new RecoverParams();
            utilityParams ??= new UtilityParams();
        }
    }

    /// <summary>
    /// 攻撃カード用パラメータ
    /// </summary>
    [System.Serializable]
    public class AttackParams
    {
        /// <summary>基本ダメージ</summary>
        public int damage;

        /// <summary>直線攻撃の長さ</summary>
        public float lineLength;

        /// <summary>直線攻撃の横幅</summary>
        public float lineWidth;

        /// <summary>円範囲攻撃の半径</summary>
        public float areaRadius;

        /// <summary>円範囲攻撃の前距離</summary>
        public float areaRange;
    }

    /// <summary>
    /// 回復カード用パラメータ
    /// </summary>
    [System.Serializable]
    public class RecoverParams
    {
        /// <summary>回復量</summary>
        public int healAmount;

        /// <summary>コスト回復量</summary>
        public int costRecoverAmount;

        /// <summary>コスト回復間隔短縮量</summary>
        public int intervalReduction;
    }

    /// <summary>
    /// サポートカード用パラメータ
    /// </summary>
    [System.Serializable]
    public class SupportParams
    {
        /// <summary>被ダメージ軽減率（％）</summary>
        public float damageReduction;

        /// <summary>次の攻撃のダメージ倍率</summary>
        public float attackMultiplier;
    }

    /// <summary>
    /// ユーティリティカード用パラメータ
    /// </summary>
    [System.Serializable]
    public class UtilityParams
    {
        /// <summary> カードを引く枚数 /// </summary>
        public int drawCount;
    }
}