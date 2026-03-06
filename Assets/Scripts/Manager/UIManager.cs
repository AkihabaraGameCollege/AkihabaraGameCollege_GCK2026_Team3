// File: UIManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// Handles UI elements for life, cost, hand display and target marker.
    /// Also spawns floating damage popups.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.25f;

        [Header("Player UI")]
        [SerializeField] private Text lifeText;
        [SerializeField] private Text costText;

        [Header("Hand UI")]
        [SerializeField] private Transform handContainer;
        [SerializeField] private GameObject cardSlotPrefab;

        [Header("Targeting")]
        [SerializeField] private GameObject targetMarkerPrefab;
        private GameObject targetMarkerInstance;

        [Header("Popups")]
        [SerializeField] private DamagePopup damagePopupPrefab;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void UpdateLife(int current, int max)
        {
            if (lifeText != null) lifeText.text = $"HP: {current}/{max}";
        }

        public void UpdateCost(int current, int max)
        {
            if (costText != null) costText.text = $"Cost: {current}/{max}";
        }

        public void UpdateHandUI(List<CardData> hand, CardManager manager)
        {
            if (handContainer == null || cardSlotPrefab == null) return;
            // clear old
            foreach (Transform t in handContainer) Destroy(t.gameObject);

            // instantiate slots
            foreach (var card in hand)
            {
                var go = Instantiate(cardSlotPrefab, handContainer);
                var slot = go.GetComponent<CardSlotUI>();
                if (slot != null) slot.Setup(card, manager);
            }
        }

        public void HighlightSelectedCard(CardData card)
        {
            // simple: could add outline or scale; omitted for simplicity
        }

        public void SetTargetMarker(Transform target)
        {
            if (target == null)
            {
                if (targetMarkerInstance != null) Destroy(targetMarkerInstance);
                targetMarkerInstance = null;
                return;
            }
            if (targetMarkerInstance == null && targetMarkerPrefab != null)
            {
                targetMarkerInstance = Instantiate(targetMarkerPrefab);
            }
            if (targetMarkerInstance != null)
            {
                targetMarkerInstance.transform.position = target.position + Vector3.up * 2f;
            }
        }

        public void SpawnDamagePopup(int amount, Vector3 worldPos, CriticalType type)
        {
            if (damagePopupPrefab == null) return;
            var p = Instantiate(damagePopupPrefab, worldPos, Quaternion.identity);
            p.Setup(amount, type);
        }

        /// <summary>
        /// Fade in a UI GameObject that has or will get a CanvasGroup.
        /// If no CanvasGroup exists, one will be added.
        /// </summary>
        public void FadeInUI(GameObject ui, float duration = -1f)
        {
            if (ui == null) return;
            if (duration <= 0f) duration = fadeDuration;
            var cg = ui.GetComponent<CanvasGroup>();
            if (cg == null) cg = ui.AddComponent<CanvasGroup>();
            ui.SetActive(true);
            StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, duration));
        }

        private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
        {
            cg.alpha = from;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            cg.alpha = to;
        }

        public void ShowFeedback(string message)
        {
            Debug.Log("UI Feedback: " + message);
        }
    }

}
