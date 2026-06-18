using System.Collections.Generic;

namespace ForestDraw
{
    /// <summary>
    /// デッキの保存データを管理するクラス
    /// </summary>
    [System.Serializable]
    public class DeckSaveData
    {
        /// <summary>
        /// セーブするデッキのカードIDを管理する変数
        /// </summary>
        public List<string> savedCardIds = new List<string>();
    }
}