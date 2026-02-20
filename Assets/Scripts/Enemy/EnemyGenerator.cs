using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int enemiesPerWave = 3; 
    [SerializeField] private float spawnOffsetRange = 5f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] waypoints;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true) // ñ≥å¿ÉãÅ[Év
        {
            SpawnWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-spawnOffsetRange, spawnOffsetRange),
                0f,
                Random.Range(-spawnOffsetRange, spawnOffsetRange)
            ) + enemyPrefab.transform.position;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            EnemyMove move = enemy.GetComponent<EnemyMove>();
            move.SetPath(waypoints);
        }
    }

}
