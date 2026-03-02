using UnityEngine;

// ScriptableObjectとしてカードデータを作成できるようにする属性
// Unityの「Create > Ueno > CardData」からアセット作成可能
[CreateAssetMenu(menuName = "Ueno/CardData", fileName = "CardData")]
public class CardData : ScriptableObject
{
    // カードの名前（ゲーム内表示用）
    public string cardName;

    // カードの種類（攻撃・防御など）
    public CardType cardType;

    // カードの効果タイプ（ダメージ系・回復系など）
    public CardEffectType effectType;

    // カードの説明文（Inspector上で複数行入力できる）
    [TextArea]
    public string description;

    // カードを使用するためのコスト
    public int cost;

    // 最小ダメージ値（ランダム計算用）
    public int minDamage;

    // 最大ダメージ値（ランダム計算用）
    public int maxDamage;

    // カードのイラスト画像
    public Sprite artwork;
}