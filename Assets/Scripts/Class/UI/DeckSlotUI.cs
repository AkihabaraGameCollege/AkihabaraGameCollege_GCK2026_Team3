using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// デッキスロットUI機能を管理するクラス
    /// </summary>
    public class DeckSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// スロットに表示するカードの画像を指定する変数
        /// </summary>
        [SerializeField]
        private Image _card_Image;

        /// <summary>
        /// ボタンコンポーネントの変数
        /// </summary>
        [SerializeField]
        private Button _slotButton;

        /// <summary>
        /// カードデータがないときに表示するビジュアルの変数
        /// </summary>
        private CardData _currentCardData;

        /// <summary>
        /// デッキマネージャーの変数
        /// </summary>
        private DeckManager _deckManager;

        /// <summary>
        /// このスロット内カードUIの詳細表示座標を参照する変数
        /// </summary>
        private Vector3 _showDetail_Position;

        /// <summary>
        /// カード詳細表示UIのY軸座標追加量を参照する変数
        /// </summary>
        [SerializeField]
        private float _addDetail_ShowPositionY = 1f;

        /// <summary>
        /// このスロットを初期設定する関数
        /// </summary>
        /// <param name="manager"></param>
        public void Setup(DeckManager manager)
        {
            // デッキマネージャーをセット
            _deckManager = manager;

            // もしスロットにボタンコンポーネントがアタッチされている場合
            if (_slotButton != null)
            {
                // ボタンがクリックされたときにカードをデッキから外す関数を登録
                _slotButton.onClick.AddListener(OnClickRemove);
            }
        }

        /// <summary>
        /// カードデータをスロットにセットする関数
        /// </summary>
        /// <param name="cardData"></param>
        public void SetCard(CardData cardData)
        {
            // --- カードデータをセット ---
            _currentCardData = cardData;
            _card_Image.sprite = cardData.cardImage;
            _card_Image.enabled = true;
        }

        /// <summary>
        /// スロットをクリアする関数
        /// </summary>
        public void ClearSlot()
        {
            // --- カードデータをクリア ---
            _currentCardData = null;
            _card_Image.enabled = false;
        }

        /// <summary>
        /// デッキからカードを削除する関数
        /// </summary>
        private void OnClickRemove()
        {
            // もしスロットにカードデータが存在していて、デッキマネージャーもセットされている場合
            if (_currentCardData != null && _deckManager != null)
            {
                // デッキマネージャーからカードを削除
                _deckManager.RemoveFromDeck(_currentCardData);
            }
        }

        /// <summary>
        /// マウスカーソルがカードに入った瞬間に呼び出す関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // もしデータが空の場合
            if (_currentCardData == null)
            {
                return;
            }

            // --- 詳細表示座標の設定 ---
            // 現在の座標を代入
            _showDetail_Position = this.transform.position;
            // Y軸を調整
            _showDetail_Position.y += _addDetail_ShowPositionY;

            // カード詳細を表示
            CardDetailViewer.instance.ShowDetail(_currentCardData.cardDetail_Image, _showDetail_Position);
        }

        /// <summary>
        /// マウスカーソルがカードから出た瞬間に呼び出す関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // もしデータが空の場合
            if (_currentCardData == null)
            {
                return;
            }

            // カード詳細を非表示
            CardDetailViewer.instance.HideDetail();
        }
    }
}