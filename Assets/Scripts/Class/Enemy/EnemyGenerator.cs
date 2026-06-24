using ForestDraw.Enemy.Components;
using ForestDraw.Enemy.Data;
using System.Collections;
using Unity.VisualScripting;
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
        [SerializeField] private Camera UICamera;

        private IEnumerator Start()
        {
            yield return null; // 1フレーム待つ（これで他の全オブジェクトの Awake が終わる）

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
        /// 1Wave分の敵を生成する関数
        /// </summary>
        private void Spawn()
        {
            // もしエネミーがスポーン不可能の場合
            if (!EnemyManager.Instance.CanGenerate)
            {
                return;
            }

            // Wave分ループ
            for (int _i = 0; _i < enemiesPerWave; _i++)
            {
                // ---スポーン前の準備---
                // スポーン場所を取得した上で参照する変数を定義
                Vector3 _spawnPosition = GetSpawnPosition();
                // エネミーのプレハブを生成した上で参照する変数を定義
                GameObject _enemy = Instantiate(enemyPrefab, _spawnPosition, Quaternion.identity);

                // エネミーを追加
                EnemyManager.Instance.AddEnemy(_enemy, enemyData.enemyName);

                // エネミーのコンポーネントを初期化
                EnemyInitialize(_enemy);
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

            controller.Initialize(waypoints, player, enemyData, UICamera);
        }
    }
}