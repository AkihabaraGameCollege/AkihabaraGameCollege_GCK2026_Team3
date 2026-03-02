using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    public CardData data;
    public CardManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.UseCard(data);
    }
}