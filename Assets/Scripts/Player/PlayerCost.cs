    using UnityEngine;
    using System;
    using System.Collections;

    namespace ForestDraw.Player.Combat
    {
        public class PlayerCost : MonoBehaviour
        {
            [SerializeField] private int maxCost = 8;
            [SerializeField] private float baseRecoverInterval = 3f;

            private float recoverInterval;
            private int currentCost;
            private float timer;

            public int CurrentCost => currentCost;

            // UI通知
            public event Action<int, float> OnCostChanged;

            private void Start()
            {
                currentCost = 0;
                recoverInterval = baseRecoverInterval;

                OnCostChanged?.Invoke(currentCost, 0);
            }

            private void Update()
            {
                timer += Time.deltaTime;

                // 回復進行度
                float progress = timer / recoverInterval;

                OnCostChanged?.Invoke(currentCost, progress);

                if (timer >= recoverInterval)
                {
                    timer = 0f;
                    RecoverCost(1);
                }
            }

            public bool UseCost(int cost)
            {
                if (currentCost < cost) return false;

                currentCost -= cost;
                OnCostChanged?.Invoke(currentCost, 0);

                return true;
            }

            public void RecoverCost(int amount)
            {
                currentCost = Mathf.Min(currentCost + amount, maxCost);
                OnCostChanged?.Invoke(currentCost, 0);
            }

            public void ReduceRecoverInterval(float amount, float duration)
            {
                StartCoroutine(RecoverIntervalBuff(amount, duration));
            }

            private IEnumerator RecoverIntervalBuff(float amount, float duration)
            {
                recoverInterval = Mathf.Max(0.5f, recoverInterval - amount);
                yield return new WaitForSeconds(duration);
                recoverInterval = baseRecoverInterval;
            }
        }
    }