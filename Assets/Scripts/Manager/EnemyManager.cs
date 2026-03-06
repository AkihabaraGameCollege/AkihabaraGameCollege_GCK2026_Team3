using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        public List<GameObject> Enemies { get; } = new();

        private void Awake()
        {
            Instance = this;
        }

        public void AddEnemy(GameObject enemy)
        {
            Enemies.Add(enemy);
        }

        public void RemoveEnemy(GameObject enemy)
        {
            Enemies.Remove(enemy);
        }
    }
}