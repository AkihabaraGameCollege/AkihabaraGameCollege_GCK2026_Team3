using UnityEngine;

// 敵キャラクターの基本的なHP管理を行うクラス
public class Enemy : MonoBehaviour
{
    // 最大体力
    public int maxHp = 1000;

    // 現在の体力
    public int hp = 1000;

    void Start()
    {
        // ゲーム開始時に現在HPを最大HPに設定
        hp = maxHp;
    }

    // ダメージを受ける処理
    public void TakeDamage(int amount)
    {
        // 指定されたダメージ分だけHPを減らす
        hp -= amount;

        // HPが0以下になったら死亡処理
        if (hp <= 0)
        {
            Die();
        }
    }

    // 敵の死亡処理
    void Die()
    {
        // このゲームオブジェクトを削除（シーンから消える）
        Destroy(gameObject);
    }
}