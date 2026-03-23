using UnityEngine;

public class Floating : MonoBehaviour
{
    public float floatDuration = 5.0f; // 浮く時間
    public float floatHeight = 2.0f; // 浮く高さ
    private bool isFloating = false; // 浮いているかどうかのフラグ
    private float startTime; // 浮き始めた時間
    private Vector3 startPosition; // 浮き始めた位置
    private Rigidbody playerRigidbody; // プレイヤーのRigidbody

    // トリガーに入ったときに呼ばれる
    void OnTriggerEnter(Collider other)
    {
        // プレイヤーがトリガーに入った場合
        if (other.CompareTag("Player"))
        {
            isFloating = true; // 浮き始める
            startTime = Time.time; // 現在の時間を記録
            startPosition = other.transform.position; // プレイヤーの位置を記録
            playerRigidbody = other.GetComponent<Rigidbody>(); // プレイヤーのRigidbodyを取得
        }
    }

    // 毎フレーム呼ばれる
    void Update()
    {
        // 浮いている状態で、プレイヤーのRigidbodyが存在する場合
        if (isFloating && playerRigidbody != null)
        {
            float elapsedTime = Time.time - startTime; // 経過時間を計算
            // 浮く時間が経過していない場合
            if (elapsedTime < floatDuration)
            {
                // 新しいY座標を計算
                float newY = Mathf.Lerp(startPosition.y, startPosition.y + floatHeight, elapsedTime / floatDuration);
                // 新しい位置を計算
                Vector3 newPosition = new Vector3(startPosition.x, newY, startPosition.z);
                // プレイヤーの位置を更新
                playerRigidbody.MovePosition(newPosition);
            }
            else
            {
                isFloating = false; // 浮く時間が経過したら浮くのをやめる
            }
        }
    }
}
