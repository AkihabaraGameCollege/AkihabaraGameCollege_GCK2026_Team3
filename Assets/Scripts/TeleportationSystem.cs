using UnityEngine;

public class TeleportationSystem : MonoBehaviour
{
    // テレポートの入り口オブジェクト
    public GameObject teleportEntrance;
    // テレポートの出口オブジェクト
    public GameObject teleportExit;

    void Start()
    {
        // 入り口と出口が設定されているか確認
        if (teleportEntrance != null && teleportExit != null)
        {
            // 入り口オブジェクトにアタッチされているTeleportEntranceスクリプトを取得
            TeleportEntrance entranceScript = teleportEntrance.GetComponent<TeleportEntrance>();
            if (entranceScript != null)
            {
                // 入り口スクリプトに出口オブジェクトを設定
                entranceScript.teleportExit = teleportExit;
            }

            // 出口オブジェクトにアタッチされているTeleportExitスクリプトを取得
            TeleportExit exitScript = teleportExit.GetComponent<TeleportExit>();
            if (exitScript != null)
            {
                // 出口スクリプトに入り口オブジェクトを設定
                exitScript.teleportEntrance = teleportEntrance;
            }
        }
    }
}
