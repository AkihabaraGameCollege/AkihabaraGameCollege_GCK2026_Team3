// File: UIManager.cs
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ForestDraw
{
    /// <summary>
    /// UIManager: update life/cost, hand UI, damage popups and target selection marker.
    /// Singleton used by other systems.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD")]
        public TextMeshProUGUI lifeText;
        public TextMeshProUGUI costText;

        [Header("Hand UI")]
        public Transform handContainer;
        public GameObject cardSlotPrefab;

        [Header("Targeting")]
        public GameObject targetMarkerPrefab;
        private GameObject currentMarker;
        private EnemyController selectedEnemy;

        [Header("Damage Popup")]
        public GameObject damagePopupPrefab;

        [Header("Game Over / Victory")]
        public GameObject gameOverUI;
        public GameObject victoryUI;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        /// <summary>
        /// Update life display
        /// </summary>
        public void UpdateLife(int current, int max)
        {
            if (lifeText != null) lifeText.text = $"Life: {current}/{max}";
        }

        /// <summary>
        /// Update cost display
        /// </summary>
        public void UpdateCost(int current, int max)
        {
            if (costText != null) costText.text = $"Cost: {current}/{max}";
        }

        /// <summary>
        /// Update hand UI using simple prefab slots.
        /// </summary>
        public void UpdateHand(List<CardData> hand)
        {
            if (handContainer == null || cardSlotPrefab == null) return;
            for (int i = handContainer.childCount - 1; i >= 0; i--) Destroy(handContainer.GetChild(i).gameObject);
            for (int i = 0; i < hand.Count; i++)
            {
                var go = Instantiate(cardSlotPrefab, handContainer);
                var slot = go.GetComponent<CardSlotUI>();
                if (slot != null) slot.Setup(hand[i], i);
            }
        }

        /// <summary>
        /// Show floating damage number above world position.
        /// </summary>
        public void ShowDamageNumber(int amount, Vector3 worldPos)
        {
            if (damagePopupPrefab == null) return;
            var go = Instantiate(damagePopupPrefab, worldPos + Vector3.up * 1.5f, Quaternion.identity);
            var popup = go.GetComponent<DamagePopup>();
            popup?.Setup(amount);
        }

        /// <summary>
        /// Handle click from PlayerController: raycast to select enemy.
        /// </summary>
        public void HandleClick()
        {
            if (Camera.main == null) return;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out var hit, 100f))
            {
                var ec = hit.collider.GetComponent<EnemyController>();
                if (ec != null)
                {
                    SelectEnemy(ec);
                }
            }
        }

        public void SelectEnemy(EnemyController enemy)
        {
            selectedEnemy = enemy;
            if (currentMarker == null && targetMarkerPrefab != null) currentMarker = Instantiate(targetMarkerPrefab);
            if (currentMarker != null && selectedEnemy != null) currentMarker.transform.position = selectedEnemy.transform.position + Vector3.up * 1.5f;
        }

        public EnemyController GetSelectedEnemy() => selectedEnemy;

        public void HideTargetMarker() { if (currentMarker != null) Destroy(currentMarker); selectedEnemy = null; }

        public void ShowGameOverUI()
        {
            if (gameOverUI != null) gameOverUI.SetActive(true);
        }

        public void ShowVictoryUI()
        {
            if (victoryUI != null) victoryUI.SetActive(true);
        }
    }
}
