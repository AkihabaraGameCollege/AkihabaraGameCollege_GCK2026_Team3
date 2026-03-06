using UnityEngine;

// カードの効果タイプを定義する列挙型（enum）
// どんな効果を持つカードかを分類するために使用する
public enum CardEffectType
{
    // 単体の敵にダメージを与える
    DamageSingle,

    // 一直線上の敵にダメージを与える
    DamageLine,

    // 範囲内の敵にダメージを与える
    DamageArea,

    // 画面内すべての敵にダメージを与える
    DamageAllOnScreen,

    // コスト（エネルギーなど）を回復する
    CostRecover,

    // 回復速度アップ（間隔短縮）
    CostRegen,

    // プレイヤーの体力を回復する
    Heal,

    // カードを追加で引く
    Draw,

    // 次に使用するカードを強化する
    BuffNext,

    // 一定量のダメージを防ぐシールドを付与する
    Shield,

    // ダメージ軽減
    DamageReduction
}