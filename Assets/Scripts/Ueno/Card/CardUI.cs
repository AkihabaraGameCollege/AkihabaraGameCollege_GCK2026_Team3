using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour
{
    public CardData cardData;
    bool dragging = false;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (dragging)
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = 10f;
            transform.position = Camera.main.ScreenToWorldPoint(mouse);
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                TryPlay();
                transform.position = startPos;
            }
        }
    }

    void OnMouseDown()
    {
        dragging = true;
    }

    void TryPlay()
    {
        if (cardData == null) return;
        bool ok = CardManager.Instance.PlayCard(cardData);
        if (!ok)
        {
            Debug.Log("Cannot play card: " + cardData.cardName);
        }
    }
}
