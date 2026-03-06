// File: GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForestDraw
{
    /// <summary>
    /// Overall game flow manager: handles victory / gameover states and tracks enemies.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private GameObject victoryUI;
        [SerializeField] private GameObject gameOverUI;

        private int enemyCount = 0;
        private bool gameEnded = false;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            enemyCount = FindObjectsOfType<EnemyController>().Length;
        }

        public void NotifyEnemyDefeated(EnemyController enemy)
        {
            enemyCount--;
            if (enemyCount <= 0) StartCoroutine(VictorySequence());
        }

        public void GameOver()
        {
            if (gameEnded) return;
            gameEnded = true;
            if (gameOverUI != null)
            {
                UIManager.Instance?.FadeInUI(gameOverUI, 0.25f);
            }
            StartCoroutine(EndAndReturn());
        }

        private IEnumerator VictorySequence()
        {
            if (gameEnded) yield break;
            gameEnded = true;
            if (victoryUI != null)
            {
                UIManager.Instance?.FadeInUI(victoryUI, 0.25f);
            }
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("StageSelect");
        }

        private IEnumerator EndAndReturn()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("StageSelect");
        }
    }
}
