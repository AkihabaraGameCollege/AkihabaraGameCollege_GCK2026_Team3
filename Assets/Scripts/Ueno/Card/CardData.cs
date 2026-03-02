using UnityEngine;

[CreateAssetMenu(menuName = "Card/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public CardEffectType effectType;

    public int cost;

    public int minDamage;
    public int maxDamage;

    public int healAmount;
    public int costAmount;

    public float duration; // バフ時間

    public bool isOneShot;
}