using UnityEngine;

public class kariEnemy : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;

    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        // プレイヤーへの方向ベクトルを計算
        Vector3 direction = player.position - transform.position;
        direction.y = 0;  // 高さ方向を無視（地面上での追尾）
        // プレイヤーの方向に回転
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        // プレイヤーの方向に移動
        Vector3 moveDirection = direction.normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + moveDirection);
    }
    
}
