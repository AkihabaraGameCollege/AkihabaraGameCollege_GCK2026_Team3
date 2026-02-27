using UnityEngine;
using System.Collections; // これを忘れると動かへんから注意！

public class ClearScript : MonoBehaviour
{
    // ここに表示したいUIをドラッグ＆ドロップする
    public GameObject VictoryUI;

    [SerializeField]
    private float time = 1;


    void Start()
    {
        // ゲームが始まったら、タイマーをスタートさせる命令や
        StartCoroutine(ShowUIRoutine());
    }

    // これがタイマーの本体（コルーチン）
    IEnumerator ShowUIRoutine()
    {
        // 指定した秒数だけ「待て」をする
        yield return new WaitForSeconds(time);

        // 待ち時間が終わったら、UIを表示（アクティブ）にする
        if (VictoryUI != null)
        {
            VictoryUI.SetActive(true);
        }
    }
}