using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// 手持ちのカードUIを管理するクラス
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class HandCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// カード画像の変数
        /// </summary>
        [SerializeField]
        private Image cardImage = null;

        /// <summary>
        /// 自分のカードデータの変数
        /// </summary>
        private CardData myCardData;

        /// <summary>
        /// 手持ちカードの管理クラスの変数
        /// </summary>
        private BattleCardManager battleManager;

        /// <summary>
        /// 元の親オブジェクトの変数
        /// </summary>
        private Transform originalParent;

        /// <summary>
        /// 隙間をキープする身代わりの変数
        /// </summary>
        private GameObject placeholder;

        /// <summary>
        /// 座標計算用のCanvasの変数
        /// </summary>
        private Canvas mainCanvas;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            mainCanvas = GetComponentInParent<Canvas>();// 親を遡って一番大元のCanvasを取得しておく
        }

        /// <summary>
        /// カードデータをセットアップの関数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        public void Setup(CardData data, BattleCardManager manager)
        {
            myCardData = data;
            battleManager = manager;

            // もしカード画像オブジェクトとデータの画像がある場合
            if (cardImage != null && data.cardImage != null)
            {
                cardImage.sprite = data.cardImage;// オブジェクトの画像にデータの画像を設定
            }
        }

        /// <summary>
        /// ドラッグ開始時の関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = transform.parent;// 元の親オブジェクトの場所を設定

            // 隙間をキープするための「透明な身代わり（Placeholder）」を作る
            placeholder = new GameObject("Placeholder");
            RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
            placeholderRect.SetParent(originalParent, false);
            placeholderRect.SetSiblingIndex(transform.GetSiblingIndex());// 

            // 身代わりのサイズを、今のカードと全く同じサイズに設定する
            LayoutElement le = placeholder.AddComponent<LayoutElement>();
            RectTransform myRect = GetComponent<RectTransform>();
            le.preferredWidth = myRect.rect.width;
            le.preferredHeight = myRect.rect.height;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            // カード本体をCanvas直下に移動して、レイアウトの支配から解放する
            transform.SetParent(mainCanvas.transform, true);
            transform.SetAsLastSibling(); // 一番手前に表示させる

            GetComponent<CanvasGroup>().blocksRaycasts = false;// カードの配置をブロックしないようにする
        }

        public void OnDrag(PointerEventData eventData)
        {
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

            // 発動チェック：カードの座標ではなく「マウスの画面上のY座標」で判定する
            if (eventData.position.y > Screen.height * 0.2f)
            {
                // 使用成功！
                bool flag = battleManager.UseCard(myCardData, gameObject);

                if (flag)
                {
                    CanUse();
                }
                else
                {
                    CanNotUse();
                }
            }
            else
            {
                CanNotUse();
            }
        }

        private void CanUse()
        {
            Destroy(placeholder); // 身代わりを消す
        }

        private void CanNotUse()
        {
            // キャンセル：身代わりが置いてある場所に戻る
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder); // 身代わりを消す
        }
    }
}