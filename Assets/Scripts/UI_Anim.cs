using JetBrains.Annotations;
using UnityEngine;

public class UI_Anim : MonoBehaviour
{
    Animator animator;

    // 初期化処理
    public void Start()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
    }

    // ゲームオーバーUIを開く
    public void OpenGameoverUI()
    {
        // "Gameover"パラメータをtrueに設定
        animator.SetBool("Gameover", true);
    }

    // ゲームオーバーUIを閉じる
    public void CloseGameoverUI()
    {
        // "Gameover"パラメータをfalseに設定
        animator.SetBool("Gameover", false);
    }

    // ゲームクリアUIを開く
    public void OpenGameclearUI()
    {
        // "Gameclear"トリガーを発動
        animator.SetTrigger("Gameclear");
    }
}
