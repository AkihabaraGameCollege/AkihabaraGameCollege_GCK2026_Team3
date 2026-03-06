// File: CardSlotUI.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// UI component for a card in hand. Handles click/drag to play.
    /// </summary>
    public class CardSlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private Image artworkImage;
        [SerializeField] private Text costText;
        [SerializeField] private Button button;

        private CardData data;
        private CardManager manager;
        private GameObject dragPreview;
        [SerializeField] private GameObject dragPreviewPrefab;

        public void Setup(CardData card, CardManager mgr)
        {
            data = card;
            manager = mgr;
            if (artworkImage != null) artworkImage.sprite = card.artwork;
            if (costText != null) costText.text = card.cost.ToString();
            if (button != null) button.onClick.AddListener(OnClickPlay);
        }

        private void OnClickPlay()
        {
            // select this card in manager for use; actual play occurs on clicking target
            manager.SelectCard(data);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            manager.SelectCard(data);
            if (dragPreviewPrefab != null)
            {
                dragPreview = Instantiate(dragPreviewPrefab, transform.root);
                var img = dragPreview.GetComponentInChildren<Image>();
                if (img != null) img.sprite = data.artwork;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragPreview != null)
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 5f));
                dragPreview.transform.position = wp;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // perform drop: cast ray from pointer to world and try to play card
            if (dragPreview != null) Destroy(dragPreview);

            Vector3 screenPos = eventData.position;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 500f))
            {
                var enemy = hit.collider.GetComponentInParent<EnemyController>();
                manager.PlayCard(data, enemy, hit.point);
            }
            else
            {
                // drop on empty space - use world point on plane at y=0
                Plane p = new Plane(Vector3.up, Vector3.zero);
                if (p.Raycast(ray, out float enter))
                {
                    Vector3 worldPoint = ray.GetPoint(enter);
                    manager.PlayCard(data, null, worldPoint);
                }
            }
            manager.DeselectCard();
        }
    }

}
