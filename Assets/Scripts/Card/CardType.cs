using UnityEngine;

// カードの大まかな種類を定義する列挙型（enum）
// カードを役割ごとに分類するために使用する
public enum CardType
{
    // 攻撃系カード（敵にダメージを与える）
    Attack,

    // 回復系カード（HPやリソース回復など）
    Recovery,

    // 支援系カード（強化・補助効果など）
    Support,

    // 特殊・補助機能カード（ドロー、特殊効果など）
    Utility
}