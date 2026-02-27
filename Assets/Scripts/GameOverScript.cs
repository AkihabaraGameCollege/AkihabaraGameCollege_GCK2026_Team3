using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    // 監視対象を PlayerController から StatusManager に置き換え
    [Tooltip("監視するプレイヤーの StatusManager をセットしてください")]
    public StatusManager playerStatus;

    // 表示するゲームオーバーUI（パネルなど）
    public GameObject GameoverUI;

    [SerializeField]
    private Button button = null;

    void Start()
    {
        // UI がアサインされていれば非表示にする
        if (GameoverUI != null) GameoverUI.SetActive(false);

        // ボタンがあればダメージ処理を登録（デバッグ用）
        if (button != null) button.onClick.AddListener(Damage);
    }

    void Update()
    {
        // playerStatus を監視して HP が 0 以下になったらゲームオーバー処理
        if (playerStatus == null) return;

        if (playerStatus.CurrentHp <= 0f)
        {
            if (GameoverUI != null) GameoverUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // デバッグ用：ボタンでプレイヤーにダメージを与える
    private void Damage()
    {
        if (playerStatus == null) return;

        // StatusManager のシグネチャに合わせてダメージを与える
        // public void Damage(int damage, Vector3 hitPos, CriticalType type, Transform attacker)
        playerStatus.Damage(1, playerStatus.transform.position, CriticalType.Normal, null);
    }
}