using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLaserPlayerTarget : MonoBehaviour
{
    Vector3 acceleration; // レーザーの加速度
    Vector3 velocity; // レーザーの速度
    Vector3 position; // レーザーの位置
    Transform target; // レーザーのターゲット

    [SerializeField][Tooltip("着弾時間")] float period = 1f;
    [SerializeField][Tooltip("着弾時差")] float deltaPeriod = 0.5f;
    [SerializeField][Tooltip("x軸初速")] float x_initial_v = 10f;
    [SerializeField][Tooltip("y軸初速")] float y_initial_v = 10f;
    [SerializeField][Tooltip("z軸初速")] float z_initial_v = 10f;

    [SerializeField][Tooltip("タゲロス時の飛行時間")] float lingerTime = 2f;
    private bool targetLost = false;

    void Start()
    {
        // 💡 ここが修正ポイント！Enemyタグではなく「Player」タグを探す
        GameObject playerObject = GameObject.FindWithTag("Player");

        // ターゲットが見つからなかった場合（プレイヤー死亡時など）
        if (playerObject == null)
        {
            Destroy(gameObject);
            return;
        }

        if (target == null)
        {
            // PlayerのTransformを取得
            target = playerObject.GetComponent<Transform>();
        }

        // 初期位置を設定
        position = transform.position;

        // 初期速度をランダムに設定
        velocity = new Vector3(Random.Range(-x_initial_v, x_initial_v),
                                Random.Range(0, y_initial_v),
                                Random.Range(-z_initial_v, z_initial_v));

        // 着弾時間をランダムに変動させる
        period += Random.Range(-deltaPeriod, deltaPeriod);
    }

    // 💡 外部（EnemyActionなど）からターゲットを上書きする場合
    public void SetTarget(Transform newTarget)
    {
        this.target = newTarget;
    }

    void Update()
    {
        if (target != null)
        {
            acceleration = Vector3.zero;
            Vector3 diff = target.position - position;

            // 物理計算でターゲットに必中させる加速度を算出
            acceleration += (diff - velocity * period) * 2f / (period * period);

            period -= Time.deltaTime;

            if (period < 0f)
            {
                Destroy(gameObject);
                return;
            }
        }
        else if (!targetLost)
        {
            targetLost = true;
            StartCoroutine(DestroyLaser(lingerTime));
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        transform.position = position;
    }

    IEnumerator DestroyLaser(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}