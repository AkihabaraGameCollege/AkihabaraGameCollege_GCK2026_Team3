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
        /// コスト通知UIのアニメーターを参照する変数（中山が編集）
        /// </summary>
        public Animator costNoticeAnimator;

        /// <summary>
        /// プレイヤー通知UIを参照する変数
        /// </summary>
        public NoticeTextUI noticeTextUI;

        /// <summary>
        /// コストに関する通知UIオブジェクトを参照する変数
        /// </summary>
        public GameObject costNotice;

        /// <summary>
        /// 使用コストが足りないときに呼ばれるIDの変数（中山が編集）
        /// </summary>
        private static readonly int costNotEnoughTrigger = Animator.StringToHash("NotEnough");

        /// <summary>
        /// アニメーション中かどうかのフラグ変数
        /// </summary>
        private bool _isAnimating = false;

        /// <summary>
        /// アニメーションの時間を参照する変数
        /// </summary>
        private float _animationTime = 1.0f;

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

            // コスト回復
            if (timer >= recoverInterval)
            {
                timer = 0f;
                RecoverCost(1);
            }
        }

        /// <summary>
        /// コスト使用を行う関数
        /// </summary>
        /// <param name="cost"></param>
        /// <returns></returns>
        public bool UseCost(int cost)
        {
            // コストが足りない場合は使用できない
            if (currentCost < cost)
            {
                // --- コスト不足の通知を行うコルーチンを開始 ---
                StartCoroutine(CostNoticeCoroutine());
                return false;
            }
            else
            {
                // --- コストを使用 ---
                currentCost -= cost;
                OnCostChanged?.Invoke(currentCost, 0);
                return true;
            }
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

        /// <summary>
        /// コスト通知の演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator CostNoticeCoroutine()
        {
            // もしアニメーション中の場合
            if (_isAnimating)
            {
                yield break;
            }

            // --- アニメーション処理 ---
            // アニメーション中フラグを立てる
            _isAnimating = true;
            // コスト不足の通知UIを表示
            noticeTextUI.TargetShow(costNotice);
            // アニメーションを再生してプレイヤーに通知
            costNoticeAnimator.SetTrigger(costNotEnoughTrigger);
            // 指定時間待機
            yield return new WaitForSeconds(_animationTime);
            // UIを非表示にする
            noticeTextUI.Hide();
            // アニメーション中フラグをリセット
            _isAnimating = false;
        }
    }
}