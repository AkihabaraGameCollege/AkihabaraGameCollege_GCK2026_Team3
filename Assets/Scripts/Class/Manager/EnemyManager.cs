using System.Collections.Generic;
using UnityEngine;
using System;

namespace ForestDraw.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        /// <summary>
        /// 敵が死んだときに発火するイベントを参照する変数
        /// </summary>
        public Action OnEnemyDie;

        public static EnemyManager Instance { get; private set; }

        [SerializeField] int maxEnemyCount = 150;

        public bool CanGenerate { get; private set; } = true;

        public List<GameObject> Enemies { get; } = new();

        /// <summary>
        /// // 死亡した敵の数を参照する変数
        /// </summary>
        public int DeadEnemyCount = 0; 

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddEnemy(GameObject enemy, string name)
        {
            Enemies.Add(enemy);
            CheckEnemyCount();
            EnemyEncountNotice.Instance.NoticeEnemyEncount(name);
        }

        public void RemoveEnemy(GameObject enemy)
        {
            // 敵の死亡した数を加算
            DeadEnemyCount++;

            Enemies.Remove(enemy);
            CheckEnemyCount();
        }

        void CheckEnemyCount()
        {
            CanGenerate = Enemies.Count < maxEnemyCount;
        }

        /// <summary>
        /// 敵が死んだときの処理を呼び出す関数
        /// </summary>
        public void EnemyDiedProcess()
        {
            // 敵が死んだら、イベントを発火させる
            OnEnemyDie?.Invoke();
        }
    }
}