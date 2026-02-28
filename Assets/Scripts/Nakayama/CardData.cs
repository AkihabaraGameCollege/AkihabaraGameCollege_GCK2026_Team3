using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// カードのデータを管理するクラス
    /// </summary>
    [CreateAssetMenu]
    public class CardData : ScriptableObject
    {
        /// <summary>
        /// カードの名前の変数
        /// </summary>
        public string cardName;
        /// <summary>
        /// カードの画像の変数
        /// </summary>
        public Sprite cardImage;
    }
}