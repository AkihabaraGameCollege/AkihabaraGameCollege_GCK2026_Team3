using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// 💡 継承：EnemyActionを親にする
public class EnemyActionEscapeFire : EnemyAction
{
    private NavMeshAgent agent;
    private StatusManager statusManager;

    [Header("Escape Settings")]
    [SerializeField] float escapeDistance = 10f;
    [SerializeField] float escapeDuration = 3.0f;

    [Header("Laser Settings")]
    [SerializeField] GameObject laserPrefab;   // プレイヤーのレーザープレハブ
    [SerializeField] Transform laserSpawner;   // 発射口
    [SerializeField] float fireInterval = 0.5f; // レーザーを打つ間隔（秒）

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        statusManager = GetComponent<StatusManager>();
    }

    public override IEnumerator Execute()
    {
        if (agent == null || Target == null) yield break;
        agent.enabled = true;

        // 🏃 プレイヤーの反対方向へ逃げる地点を計算
        Vector3 escapeDir = (transform.position - Target.position).normalized;
        Vector3 escapePos = transform.position + escapeDir * escapeDistance;
        agent.SetDestination(escapePos);

        float timer = 0;
        float shotTimer = 0;

        // 💡 逃げている間（escapeDurationの間）ループする
        while (timer < escapeDuration)
        {
            timer += Time.deltaTime;
            shotTimer += Time.deltaTime;

            // 💡 等間隔でレーザーを発射
            if (shotTimer >= fireInterval)
            {
                FireLaser();
                shotTimer = 0;
            }

            yield return null;
        }

        agent.ResetPath();
        agent.enabled = false;
    }

    // 💡 レーザー発射処理（プレイヤーのFireを参考に実装）
    private void FireLaser()
    {
        if (laserPrefab != null && laserSpawner != null)
        {
            GameObject laser = Instantiate(laserPrefab, laserSpawner.position, laserSpawner.rotation);
            DamageSource source = laser.GetComponent<DamageSource>();
            if (source != null && statusManager != null)
            {
                source.Initialize(statusManager);
            }
        }
    }

    public override void Stop()
    {
        if (agent != null) agent.enabled = false;
    }
}