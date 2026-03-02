using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour
{
    public CardData cardData;
    bool dragging = false;
    Vector3 startPos;

    // 追加: 選択表示用
    bool selected = false;
    Vector3 originalScale;

    void Start()
    {
        startPos = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (dragging)
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = 10f;
            transform.position = Camera.main.ScreenToWorldPoint(mouse);
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                TryPlay();
                transform.position = startPos;
            }
        }
    }

    // 既存: マウスクリック（レガシー）
    void OnMouseDown()
    {
        dragging = true;
    }

    // public メソッド: TextMeshPro ボタンの onClick に割り当てる
    // ボタンを「押す」ごとに選択のトグルを行う（選択→クリックで PlayFromButton を呼ぶ、等の運用が可能）
    public void OnTMPButtonToggleSelect()
    {
        if (!selected) Select();
        else Deselect();
    }

    // public メソッド: ボタンから直接カードをプレイしたい場合に割り当てる
    public void PlayFromButton()
    {
        // 選択状態を解除して位置を戻す（必要なら挙動は調整）
        Deselect();
        TryPlay();
        transform.position = startPos;
    }

    // 選択表示（必要ならエフェクトを拡張）
    public void Select()
    {
        selected = true;
        transform.localScale = originalScale * 1.08f;
        // TODO: 色変更や枠表示などを追加して視覚化を強化可能
    }

    public void Deselect()
    {
        selected = false;
        transform.localScale = originalScale;
    }

    void TryPlay()
    {
        if (cardData == null) return;
        bool ok = CardManager.Instance.PlayCard(cardData);
        if (!ok)
        {
            Debug.Log("Cannot play card: " + cardData.cardName);
        }
        else
        {
            // プレイ成功時は選択解除／視覚リセット
            Deselect();
        }
    }
}
