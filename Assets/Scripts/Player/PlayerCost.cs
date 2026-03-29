using UnityEngine;
using System;
using System.Collections;

namespace ForestDraw.Player.Combat
{
    /// <summary>
    /// プレイヤーのコスト管理
    /// </summary>
    public class PlayerCost : MonoBehaviour
    {
        [SerializeField] private int maxCost = 8;                // 最大コスト
        [SerializeField] private float baseRecoverInterval = 3f; // 基本回復間隔

        private float recoverInterval; // 現在の回復間隔
        private int currentCost;       // 現在コスト
        private float timer;           // 回復用タイマー

        public int CurrentCost => currentCost;

        // UI更新用イベント
        public event Action<int, float> OnCostChanged;

        /// <summary>
        /// アニメーターを参照する変数（中山が編集）
        /// </summary>
        private Animator animator = null;

        /// <summary>
        /// アニメーターの名前を参照する変数（中山が編集）
        /// </summary>
        public string animatorName = null;

        /// <summary>
        /// 使用コストが足りないときに呼ばれるIDの変数（中山が編集）
        /// </summary>
        private static readonly int costNotEnoughTrigger = Animator.StringToHash("NotEnough");

        private void Start()
        {
            animator = GameObject.Find(animatorName).GetComponent<Animator>();// シーン内からアニメーターを探して取得（中山が編集）

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

            // コスト回復
            if (timer >= recoverInterval)
            {
                timer = 0f;
                RecoverCost(1);
            }
        }

        // コスト使用
        public bool UseCost(int cost)
        {
            if (currentCost < cost)
            {
                animator.SetTrigger(costNotEnoughTrigger);// アニメーションを再生してプレイヤーに通知（中山が編集）
                return false;
            }

            currentCost -= cost;
            OnCostChanged?.Invoke(currentCost, 0);

            return true;
        }

        // コスト回復
        public void RecoverCost(int amount)
        {
            currentCost = Mathf.Min(currentCost + amount, maxCost);
            OnCostChanged?.Invoke(currentCost, 0);
        }

        // 一定時間回復間隔を短縮
        public void ReduceRecoverInterval(float amount, float duration)
        {
            StartCoroutine(RecoverIntervalBuff(amount, duration));
        }

        private IEnumerator RecoverIntervalBuff(float amount, float duration)
        {
            recoverInterval = amount;

            yield return new WaitForSeconds(duration);

            recoverInterval = baseRecoverInterval;
        }
    }
}