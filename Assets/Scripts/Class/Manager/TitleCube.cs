using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCube : MonoBehaviour
{
    // 上下に動かすスピード
    [SerializeField]
    public float speed = 2f;

    // 上下に動かす幅
    [SerializeField]
    public float amplitude = 0.15f;

    // y座標を保存する変数
    public float nowPosi;

    void Start()
    {
        // 初期のy座標を取得し、保存
        nowPosi = this.transform.position.y;
    }

    void Update()
    {
        // オブジェクトを上下に動かす
        transform.position = new Vector3(
            transform.position.x,
            nowPosi + Mathf.Sin(Time.time * speed) * amplitude,
            transform.position.z
        );
    }
}
