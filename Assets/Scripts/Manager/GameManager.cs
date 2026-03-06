// File: GameManager.cs
using System.Collections;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// GameManager: controls game flow, victory and game over conditions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float endDelay = 1f;

        private int enemiesAlive = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        void Start()
        {
            enemiesAlive = EnemyController.GetAllEnemies().Count;
        }

        public void OnEnemyKilled()
        {
            enemiesAlive = EnemyController.GetAllEnemies().Count;
            if (enemiesAlive == 0)
            {
                StartCoroutine(VictoryRoutine());
            }
        }

        public void GameOver()
        {
            StartCoroutine(GameOverRoutine());
        }

        private IEnumerator VictoryRoutine()
        {
            UIManager.Instance?.ShowVictoryUI();
            yield return new WaitForSeconds(endDelay);
            UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
        }

        private IEnumerator GameOverRoutine()
        {
            UIManager.Instance?.ShowGameOverUI();
            yield return new WaitForSeconds(endDelay);
            UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
        }
    }
}
