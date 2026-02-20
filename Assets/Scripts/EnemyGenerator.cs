using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // 敵のプレハブ
    [SerializeField] private float spawnInterval = 0.2f; // 敵の出現間隔
    [SerializeField] private int enemiesToSpawn = 5; // 各フェーズで出現する敵の数
    [SerializeField] private Transform[] waypoints;


    [SerializeField] private float spawnOffsetRange = 5f; // 生成位置のランダムなオフセット範囲

    private bool isSpawning = false; // 敵の生成が開始されているかどうかのフラグ

    [Header("Difficulty")]
    [SerializeField] int spawnLevel = 1; // Step16 これを変えれば強さが変わる


    private void Start()
    {
        isSpawning = true;              //生成開始フラグを入れる、生成中は再接触してもSpawnされない
        StartCoroutine(SpawnEnemies()); // コルーチンを開始
    }

    private IEnumerator SpawnEnemies()
    {

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // 自身の位置を基準にランダムにオフセット
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-spawnOffsetRange, spawnOffsetRange),
                0f, 
                Random.Range(-spawnOffsetRange, spawnOffsetRange)
            ) + enemyPrefab.transform.position;

            // 敵をスポーンポイントにスポーンさせる
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            EnemyMove follower = enemy.GetComponent<EnemyMove>();
            follower.SetPath(waypoints);
            // Step16 レベルを適用
            StatusManager status = enemyPrefab.GetComponent<StatusManager>();
            if (status != null)
            {
                status.SetLevel(spawnLevel);
            }

            // 指定された間隔で次の敵をスポーンさせる
            yield return new WaitForSeconds(spawnInterval);
        }

        // 生成が完了したらオブジェクトを破壊
        Destroy(gameObject);
    }

}
