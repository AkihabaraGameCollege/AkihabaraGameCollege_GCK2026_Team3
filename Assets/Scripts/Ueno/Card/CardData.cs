using UnityEngine;

[CreateAssetMenu(menuName = "Ueno/CardData", fileName = "CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public CardEffectType effectType;
    [TextArea]
    public string description;
    public int cost;
    public int minDamage;
    public int maxDamage;
    public Sprite artwork;
}
