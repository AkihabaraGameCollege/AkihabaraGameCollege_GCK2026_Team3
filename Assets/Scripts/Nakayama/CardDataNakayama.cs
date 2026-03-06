using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// カードのデータを管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]
    public class CardDataNakayama : ScriptableObject
    {
        /// <summary>
        /// カードのIDを管理する変数
        /// </summary>
        public string cardId;

        /// <summary>
        /// カードの名前を管理する変数
        /// </summary>
        public string cardName;

        /// <summary>
        /// カードの説明を管理する変数
        /// </summary>
        public Sprite cardImage;
    }
}