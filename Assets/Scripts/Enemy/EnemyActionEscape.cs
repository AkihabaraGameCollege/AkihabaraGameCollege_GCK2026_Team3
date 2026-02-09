using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActionEscape : EnemyAction
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private StatusManager statusManager; // 💡 持ち主登録に必要

    [Header("Escape Settings")]
    [SerializeField] float escapeDistance = 10f;
    [SerializeField] float escapeDuration = 3.0f; // 💡 少し長くするとレーザーがいっぱい出るで

    [Header("Laser Settings")]
    [SerializeField] GameObject laserPrefab;   // 💡 プレイヤーのレーザープレハブをセット
    [SerializeField] Transform laserSpawner;   // 💡 発射口（エネミーの前に配置した空のオブジェクト）
    [SerializeField] float fireInterval = 0.5f; // 💡 何秒ごとに撃つか

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        statusManager = GetComponent<StatusManager>(); // 自分自身のStatusManagerを取得
    }

    public override IEnumerator Execute()
    {
        if (rb != null) rb.isKinematic = true;
        if (agent == null || Target == null) yield break;

        agent.enabled = true;

        // 🏃 逃走地点の計算
        Vector3 escapeDir = transform.position - Target.position;
        Vector3 escapePos = transform.position + escapeDir.normalized * escapeDistance;
        agent.SetDestination(escapePos);

        // 💡 逃げながらレーザーを撃つループ
        float timer = 0;
        float shotTimer = 0;

        while (timer < escapeDuration)
        {
            // ターゲット（プレイヤー）の方を向きながら逃げる（後ろ向きに逃げる感じ）
            // もし前を向いて逃げたいなら、この回転処理は消してもOK
            Vector3 lookDir = Target.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

            // 🔫 一定間隔でレーザー発射
            shotTimer += Time.deltaTime;
            if (shotTimer >= fireInterval)
            {
                FireLaser();
                shotTimer = 0;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        StopAgent();
    }

    // 💡 プレイヤーのFire()を参考にした発射処理
    private void FireLaser()
    {
        if (laserPrefab != null && laserSpawner != null)
        {
            GameObject laser = Instantiate(laserPrefab, laserSpawner.position, laserSpawner.rotation);

            // 💡 課題の「ダメージを受ける動作」のために持ち主を登録
            DamageSource source = laser.GetComponent<DamageSource>();
            if (source != null && statusManager != null)
            {
                source.Initialize(statusManager);
            }
        }
    }

    public override void Stop()
    {
        StopAgent();
    }

    private void StopAgent()
    {
        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.enabled = false;
        }
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }
    }
}