using ForestDraw.Enemy.Data;
using System.Collections;
using UnityEngine;

namespace ForestDraw.Enemy.Spawner
{
    /// <summary>
    /// 敵を一定間隔で生成するクラス。
    /// ・Wave形式で敵を生成する
    /// ・各コンポーネントへデータを注入する
    /// ・攻撃対象を設定する
    /// </summary>
    public class EnemyGenerator : MonoBehaviour
    {
        // ===== 生成設定 =====
        [Header("生成設定")]
        [SerializeField] private float spawnInterval = 5f;    // 生成間隔（秒）
        [SerializeField] private int enemiesPerWave = 3;      // 1Waveあたりの生成数
        [SerializeField] private float spawnOffsetRange = 5f; // 生成位置のランダム範囲

        // ===== 参照設定 =====
        [Header("参照設定")]
        [SerializeField] private GameObject enemyPrefab;      // 敵Prefab
        [SerializeField] private Transform waypointRoot;      // 移動ルート
        [SerializeField] private Transform player;           // 攻撃対象
        [SerializeField] private EnemyData enemyData;         // 敵のステータスデータ

        private void Start()
        {
            // 生成ループ開始
            StartCoroutine(SpawnLoop());
        }

        /// <summary>
        /// 一定間隔で敵を生成し続ける
        /// </summary>
        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                Spawn();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        /// <summary>
        /// 1Wave分の敵を生成する
        /// </summary>
        private void Spawn()
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                EnemyManager.Instance.AddEnemy(enemy);

                EnemyInitialize(enemy);
            }
        }

        /// <summary>
        /// ランダムな生成位置を取得する
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            return transform.position + new Vector3(
                Random.Range(-spawnOffsetRange, spawnOffsetRange),
                0f,
                Random.Range(-spawnOffsetRange, spawnOffsetRange)
            );
        }

        /// <summary>
        /// エネミーコンポーネントの初期化
        /// </summary>
        private void EnemyInitialize(GameObject enemy)
        {
            if (!enemy.TryGetComponent<EnemyController>(out var controller)) return;

            Transform[] waypoints = new Transform[waypointRoot.childCount];
            for (int i = 0; i < waypointRoot.childCount; i++)
            {
                waypoints[i] = waypointRoot.GetChild(i);
            }

            controller.Initialize(waypoints, player, enemyData);
        }
    }
}