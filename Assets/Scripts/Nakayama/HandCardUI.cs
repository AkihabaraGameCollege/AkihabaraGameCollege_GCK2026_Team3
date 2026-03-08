using ForestDraw;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))] // ★付け忘れ防止
public class HandCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image cardImage;

    private CardData myCardData;
    private BattleCardManager battleManager;

    // ドラッグ制御用の変数
    private Transform originalParent;
    private GameObject placeholder; // ★追加: 隙間をキープする身代わり
    private Canvas mainCanvas;      // ★追加: 座標計算用のCanvas

    private void Start()
    {
        // 親を遡って一番大元のCanvasを取得しておく
        mainCanvas = GetComponentInParent<Canvas>();
    }

    public void Setup(CardData data, BattleCardManager manager)
    {
        myCardData = data;
        battleManager = manager;
        if (cardImage != null && data.cardImage != null)
        {
            cardImage.sprite = data.cardImage;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // 1. 隙間をキープするための「透明な身代わり（Placeholder）」を作る
        placeholder = new GameObject("Placeholder");
        RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
        placeholderRect.SetParent(originalParent, false);
        placeholderRect.SetSiblingIndex(transform.GetSiblingIndex());

        // 身代わりのサイズを、今のカードと全く同じサイズに設定する
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        RectTransform myRect = GetComponent<RectTransform>();
        le.preferredWidth = myRect.rect.width;
        le.preferredHeight = myRect.rect.height;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        // 2. カード本体をCanvas直下に移動して、レイアウトの支配から解放する
        transform.SetParent(mainCanvas.transform, true);
        transform.SetAsLastSibling(); // 一番手前に表示させる

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 3. 【最重要】Screen Space - Camera など、どんなCanvas設定でも
        // マウスのピクセル位置を、正確にUI用のワールド座標に変換して追従させる
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            mainCanvas.GetComponent<RectTransform>(),
            eventData.position,
            mainCanvas.worldCamera,
            out Vector3 globalMousePos
        );

        transform.position = globalMousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 4. 発動チェック：カードの座標ではなく「マウスの画面上のY座標」で判定する
        if (eventData.position.y > Screen.height * 0.4f)
        {
            // 使用成功！
            battleManager.UseCard(myCardData, gameObject);
            Destroy(placeholder); // 身代わりを消す
        }
        else
        {
            // キャンセル：身代わりが置いてある場所に戻る
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder); // 身代わりを消す
        }
    }
}