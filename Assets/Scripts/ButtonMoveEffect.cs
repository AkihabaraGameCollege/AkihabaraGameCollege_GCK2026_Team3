using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonMoveEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public float speed = 1.0f; // 移動速度
    public float amplitude = 10.0f; // 振幅
    public bool scaleSetting = false; // 大きさを変更するか
    public float scale = 1.0f; // 大きさ

    private Vector3 startPosition; // 初期位置
    private Vector3 startScale; // 初期スケール
    private bool isMouseOver = false; // マウスオーバー状態
    private bool isSelected = false; // 選択状態

    // StartはMonoBehaviourが作成された後、最初のUpdateの前に一度だけ呼び出されます
    void Start()
    {
        startPosition = transform.localPosition; // 初期位置を保存
        startScale = transform.localScale; // 初期スケールを保存
    }

    // Updateはフレームごとに一度呼び出されます
    void Update()
    {
        if (isMouseOver || isSelected)
        {
            // マウスオーバーまたは選択状態の場合、ボタンを移動させる
            float x = Mathf.Sin(Time.time * speed) * amplitude;
            float y = Mathf.Cos(Time.time * speed) * amplitude;
            transform.localPosition = startPosition + new Vector3(x, y, 0);
            OnScaleSet(); // スケール設定を適用
        }
        else
        {
            // それ以外の場合、初期位置と初期スケールに戻す
            transform.localPosition = startPosition;
            transform.localScale = startScale;
        }
    }

    // マウスがボタンに入ったときに呼び出されます
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    // マウスがボタンから出たときに呼び出されます
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    // ボタンが選択されたときに呼び出されます
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    // ボタンの選択が解除されたときに呼び出されます
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }

    // スケール設定を適用するメソッド
    private void OnScaleSet()
    {
        if (scaleSetting)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
