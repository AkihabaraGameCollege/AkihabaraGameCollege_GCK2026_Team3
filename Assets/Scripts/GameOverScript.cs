using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    // ★1：プレイヤーのHPスクリプトをここに入れる（見張る対象）
    // 「PlayerHP」のところは、君が作ったスクリプトの名前に書き換えてな！
    public PlayerController playerScript;

    // ★2：出したいゲームオーバーUI（パネルとか画像）をここに入れる
    public GameObject GameoverUI;

    int maxHp = 3;

    [SerializeField]
    private Button button = null;

    void Start()
    {
        // ゲームが始まった瞬間は、UIを隠しておく（オフにする）
        GameoverUI.SetActive(false);

        button.onClick.AddListener(Damage);
    }

    void Update()
    {
        // もし、プレイヤーのHPが 0 以下になったら...
        // 「playerScript.hp」の「hp」は、君のスクリプトで使ってる変数名に合わせてな！
        if (maxHp <= 0)
        {
            // ゲームオーバーUIを表示する（オンにする）！
            GameoverUI.SetActive(true);

            // ついでにゲームの時間を止めると、もっとゲームオーバーっぽくなるで
            Time.timeScale = 0f;
        }
    }

    private void Damage()
    {
        maxHp--;
    }
}