using ForestDraw.Combat;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float rotateSpeed;

    public void Init(Transform target, float spd, float rotSpd)
    {
        this.target = target;
        speed = spd;
        rotateSpeed = rotSpd;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // ターゲット方向
        Vector3 dir = (target.position - transform.position).normalized;

        // 向きをゆっくりターゲットに向ける（ホーミング感）
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );

        // 前進
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) Destroy(gameObject);
    }
}