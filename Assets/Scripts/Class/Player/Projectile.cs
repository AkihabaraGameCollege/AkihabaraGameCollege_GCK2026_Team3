using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float rotateSpeed = 10;

    public void Init(Transform target)
    {
        this.target = target;
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