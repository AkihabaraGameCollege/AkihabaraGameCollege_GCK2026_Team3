using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// 手持ちのカードのUIを管理するクラス
    /// </summary>
    public class HandCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// カードの画像を表示するためのImageコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Image cardImage = null;

        /// <summary>
        /// カードのデータを保持する変数
        /// </summary>
        private CardData myCardData;

        /// <summary>
        /// カードの使用判定や、使用後の処理を行うためのマネージャーへの参照変数
        /// </summary>
        private BattleCardManager battleManager;

        /// <summary>
        /// 元にの親（HandArea）を記憶しておくための変数
        /// </summary>
        private Transform originalParent;

        /// <summary>
        /// 元の並び順を記憶しておくための変数
        /// </summary>
        private int originalSiblingIndex;

        /// <summary>
        /// セットアップ用の関数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        public void Setup(CardData data, BattleCardManager manager)
        {
            myCardData = data;
            battleManager = manager;

            // もしカード画像がセットされいる場合
            if (cardImage != null && data.cardImage != null)
            {
                cardImage.sprite = data.cardImage;
            }
        }

        /// <summary>
        /// ドラッグ開始の瞬間に呼ばれる関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 元の親（HandArea）と、並び順を記憶しておく
            originalParent = transform.parent;// ドラッグ開始の瞬間の親（HandArea）を記憶しておく
            originalSiblingIndex = transform.GetSiblingIndex();// ドラッグ開始の瞬間の並び順を記憶しておく

            // HorizontalLayoutGroup の影響を一時的に外すため、親を一番上のCanvasに変更する
            transform.SetParent(transform.root);// ドラッグ中はCanvas直下に移動させることで、レイアウトの影響を受けずに自由に動かせるようにする
            transform.SetAsLastSibling(); // ドラッグ中のカードが他のUIの下に隠れないように最前面へ

            GetComponent<CanvasGroup>().blocksRaycasts = false;// ドラッグ中はカードがレイキャストをブロックしないようにする
        }

        /// <summary>
        /// ドラッグ中（指を動かしている間）に毎フレーム呼ばれる関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;// ドラッグ中はカードの位置を常に指の位置に合わせる
        }

        /// <summary>
        /// ドラッグ終了の瞬間に呼ばれる関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;// ドラッグが終わったらカードがレイキャストをブロックするように戻す

            // もしカードの高さが画面の40%より高い位置にある場合
            if (transform.position.y > Screen.height * 0.4f)
            {
                battleManager.UseCard(myCardData, gameObject);
            }
            else
            {
                // キャンセル: 高さが足りなかったら元の手札の場所・順番に戻す
                transform.SetParent(originalParent);// 元の親（HandArea）に戻す
                transform.SetSiblingIndex(originalSiblingIndex);// 元の並び順に戻す
            }
        }
    }
}