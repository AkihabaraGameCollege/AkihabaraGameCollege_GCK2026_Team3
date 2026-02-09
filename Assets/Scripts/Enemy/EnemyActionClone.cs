using UnityEngine;
using System.Collections;

public class EnemyActionClone : MonoBehaviour
{
    [Header("Clone Settings")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private int cloneCount = 3;
    [SerializeField] private float scatterRange = 10f; // 散らばる広さ
    [SerializeField] private float minDistance = 5f;  // 最低限離れる距離

    [Header("Auto Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f; // 何秒に一回出すか

    void Start()
    {
        // 💡 ゲーム開始時に、自動実行のループ（コルーチン）をスタートさせる
        StartCoroutine(AutoCloneLoop());
    }

    // 💡 これが「数秒に一回」を繰り返す魔法のループや！
    private IEnumerator AutoCloneLoop()
    {
        while (true)
        {
            // spawnInterval（5秒とか）分だけ待つ
            yield return new WaitForSeconds(spawnInterval);

            // 分身を出す処理を実行！
            Execute();
        }
    }

    public void Execute()
    {
        if (clonePrefab == null) return;

        for (int i = 0; i < cloneCount; i++)
        {
            // 💡 自分の周りに重ならないように位置を決める
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float dist = minDistance + Random.Range(0, scatterRange);
            Vector3 spawnOffset = new Vector3(randomDir.x, 0, randomDir.y) * dist;
            Vector3 randomPos = transform.position + spawnOffset;

            // 分身を生成
            GameObject clone = Instantiate(clonePrefab, randomPos, transform.rotation);
            clone.tag = "Enemy";
        }

        Debug.Log(spawnInterval + "秒経ったから分身出したで！");
    }
}