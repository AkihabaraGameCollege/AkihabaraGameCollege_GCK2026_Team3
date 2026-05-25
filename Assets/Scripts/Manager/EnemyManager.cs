using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager instance;

        [SerializeField] int maxEnemyCount = 150;

        public bool CanGenerate { get; private set; } = true;

        public List<GameObject> Enemies { get; } = new();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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
            Enemies.Remove(enemy);
            CheckEnemyCount();
        }

        void CheckEnemyCount()
        {
            CanGenerate = Enemies.Count < maxEnemyCount;
        }
    }
}