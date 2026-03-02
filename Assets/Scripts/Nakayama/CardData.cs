using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// カードのデータを管理するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]
    public class CardData : ScriptableObject
    {
        /// <summary>
        /// カードのIDを管理する変数
        /// </summary>
        public string cardName;

        /// <summary>
        /// カードの説明を管理する変数
        /// </summary>
        public Sprite cardImage;
    }
}