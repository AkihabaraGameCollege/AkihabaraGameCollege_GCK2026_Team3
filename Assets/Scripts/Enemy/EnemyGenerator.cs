using System.Collections;
using UnityEngine;
using ForestDraw.Enemy.Attack;
using ForestDraw.Enemy.Data;
using ForestDraw.Enemy.Components;

/// <summary>
/// 敵を一定間隔で生成するクラス
/// ・Wave形式で敵を生成
/// ・移動パラメータを外部から注入
/// ・攻撃ターゲットを設定
/// </summary>
namespace ForestDraw.Enemy.Spawner
{
    public class EnemyGenerator : MonoBehaviour
    {
        // =========================
        // ▼ 生成設定
        // =========================
        [Header("生成設定")]

        [SerializeField] private float spawnInterval = 5f;      // 敵を生成する間隔（秒）
        [SerializeField] private int enemiesPerWave = 3;        // 1回の生成で出す敵の数
        [SerializeField] private float spawnOffsetRange = 5f;   // 生成位置のランダム範囲

        // =========================
        // ▼ 参照設定
        // =========================
        [Header("参照設定")]

        [SerializeField] private GameObject enemyPrefab;        // 生成する敵Prefab
        [SerializeField] private Transform[] waypoints;         // 敵の移動ルート
        [SerializeField] private GameObject player;             // 攻撃対象（例：Player）
        [SerializeField] private EnemyData enemyData;           // 生成する敵データ
        // =========================
        // 初期処理
        // =========================
        private void Start()
        {
            // 敵生成ループ開始
            StartCoroutine(SpawnLoop());
        }

        // =========================
        // 敵生成ループ（無限）
        // =========================
        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                Spawn();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        // =========================
        // 敵生成処理
        // =========================
        private void Spawn()
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                // 生成位置をランダムにずらす
                Vector3 spawnPosition = transform.position + enemyPrefab.transform.position
                    + new Vector3(
                    Random.Range(-spawnOffsetRange, spawnOffsetRange),
                    0f,
                    Random.Range(-spawnOffsetRange, spawnOffsetRange)
                );

                // 敵を生成
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // =========================
                // EnemyMove の設定
                // =========================
                EnemyMove move = enemy.GetComponent<EnemyMove>();
                if (move != null)
                {
                    // 移動速度とランダム半径を外部から注入
                    move.Initialize(enemyData);

                    // 移動ルート設定
                    move.SetPath(waypoints);
                }
                // =========================
                // EnemyHealth の設定
                // =========================
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.Initialize(enemyData);
                }
                // =========================
                // EnemyAttack の設定
                // =========================
                EnemyAttackBase attack = enemy.GetComponent<EnemyAttackBase>();
                if (attack != null)
                {
                    // 攻撃対象を設定
                    attack.Initialize(enemyData);
                    attack.SetTarget(player);
                }
            }
        }
    }
}