using UnityEngine;

/// <summary>
/// カードの大まかな種類を定義する列挙型
/// カードを役割ごとに分類するために使用する
/// </summary>
public enum CardType
{
    /// <summary>攻撃系カード（敵にダメージを与える）</summary>
    Attack,

    /// <summary>回復系カード（HPやリソースを回復する）</summary>
    Recovery,

    /// <summary>支援系カード（強化・防御などの補助効果）</summary>
    Support,

    /// <summary>特殊カード（ドローなどの特殊効果）</summary>
    Utility
}