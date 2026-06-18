using UnityEngine;

/// <summary>
/// カードの効果タイプを定義する列挙型
/// カードがどのような効果を持つかを分類するために使用する
/// </summary>
public enum CardEffectType
{
    /// <summary>単体の敵にダメージを与える</summary>
    DamageSingle,

    /// <summary>一直線上の敵にダメージを与える</summary>
    DamageLine,

    /// <summary>範囲内の敵にダメージを与える</summary>
    DamageArea,

    /// <summary>画面内すべての敵にダメージを与える</summary>
    DamageAllOnScreen,

    /// <summary>コストを回復する</summary>
    CostRecover,

    /// <summary>コスト回復速度を上げる（回復間隔短縮）</summary>
    CostRegen,

    /// <summary>プレイヤーの体力を回復する</summary>
    Heal,

    /// <summary>カードを追加で引く</summary>
    Draw,

    /// <summary>次に使用する攻撃を強化する</summary>
    BuffNext,

    /// <summary>ダメージを防ぐシールドを付与する</summary>
    Shield,

    /// <summary>受けるダメージを軽減する</summary>
    DamageReduction
}