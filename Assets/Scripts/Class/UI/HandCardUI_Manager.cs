using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// 手持ちのカードUIを管理するクラス
    /// </summary>
    public class HandCardUI_Manager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// カード画像の変数
        /// </summary>
        [SerializeField]
        private Image _card_Image;

        /// <summary>
        /// 自分のカードデータの変数
        /// </summary>
        private CardData _myCardData;

        /// <summary>
        /// 元の親オブジェクトの変数
        /// </summary>
        private Transform _original_Parent;

        /// <summary>
        /// 隙間をキープする身代わりの変数
        /// </summary>
        private GameObject _placeHolder;

        /// <summary>
        /// 座標計算用のCanvasの変数
        /// </summary>
        private Canvas _mainCanvas;

        /// <summary>
        /// マウスのY座標が画面の何％より上にあるかを判定するための変数
        /// </summary>
        private float _screenHeightMultiplier = 0.2f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // 親を遡って一番大元のCanvasを取得しておく
            _mainCanvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// カードデータをセットアップの関数
        /// </summary>
        /// <param name="data"></param>
        public void Setup(CardData data)
        {
            // カードデータを保持する
            _myCardData = data;

            // もしカード画像オブジェクトとデータの画像がある場合
            if (_card_Image != null && data.cardImage != null)
            {
                // オブジェクトの画像にデータの画像を設定
                _card_Image.sprite = data.cardImage;
            }
        }

        /// <summary>
        /// マウスカーソルがカードに入った瞬間に呼び出す関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // カード詳細画像を表示する
            CardDetailViewer.instance.ShowDetail(_myCardData.cardDetail_Image, this.transform.position);
        }

        /// <summary>
        /// マウスカーソルがカードから出た瞬間に呼び出す関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // カード詳細画像を非表示にする
            CardDetailViewer.instance.HideDetail();
        }

        /// <summary>
        /// ドラッグ開始時の処理を行う関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 元の親オブジェクトの場所を設定
            _original_Parent = transform.parent;

            // --- 隙間をキープするための「透明な身代わり（Placeholder）」を作る ---
            _placeHolder = new GameObject("PlaceHolder");
            RectTransform placeholderRect = _placeHolder.AddComponent<RectTransform>();
            placeholderRect.SetParent(_original_Parent, false);
            placeholderRect.SetSiblingIndex(transform.GetSiblingIndex());

            // --- 身代わりのサイズを、今のカードと全く同じサイズに設定する ---
            LayoutElement le = _placeHolder.AddComponent<LayoutElement>();
            RectTransform myRect = GetComponent<RectTransform>();
            le.preferredWidth = myRect.rect.width;
            le.preferredHeight = myRect.rect.height;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            // カード本体をCanvas直下に移動して、レイアウトの支配から解放する
            transform.SetParent(_mainCanvas.transform, true);
            // 一番手前に表示させる
            transform.SetAsLastSibling();

            // カードの配置をブロックしないようにする
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        /// <summary>
        /// ドラッグ中処理を行う関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            // --- マウスのピクセル位置を、正確にUI用のワールド座標に変換して追従させる ---
            RectTransformUtility.ScreenPointToWorldPointInRectangle
                (
                _mainCanvas.GetComponent<RectTransform>(),
                eventData.position,
                _mainCanvas.worldCamera,
                out Vector3 global_MousePosition
            );

            // マウスのワールド座標に追従させる
            transform.position = global_MousePosition;
        }

        /// <summary>
        /// ドラッグ終了時の処理を行う関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // カードの配置をブロックするようにする
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            // もしマウスのY座標が画面の20％より上にある場合
            if (eventData.position.y > Screen.height * _screenHeightMultiplier)
            {
                // 使用成功か失敗かを判定する関数を呼び出す
                bool flag = BattleCardManager.Instance.UseCard(_myCardData, gameObject);

                // もし成功の場合
                if (flag)
                {
                    // 身代わりを消す処理を呼び出す
                    CanUse();
                }
                else
                {
                    // 元の位置に戻す処理を呼び出す
                    CanNotUse();
                }
            }
            else
            {
                // 元の位置に戻す処理を呼び出す
                CanNotUse();
            }
        }

        /// <summary>
        /// カード使用が成功した場合の処理を行う関数
        /// </summary>
        private void CanUse()
        {
            // 身代わりを消す
            Destroy(_placeHolder); 
        }

        /// <summary>
        /// カード使用が失敗した場合の処理を行う関数
        /// </summary>
        private void CanNotUse()
        {
            // 元の親オブジェクトに戻す
            transform.SetParent(_original_Parent, false);
            // 身代わりの位置に戻す
            transform.SetSiblingIndex(_placeHolder.transform.GetSiblingIndex());
            // 身代わりを消す
            Destroy(_placeHolder); 
        }
    }
}