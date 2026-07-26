using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForestDraw
{
    /// <summary>
    /// カードのUIを管理するクラス
    /// </summary>
    public class CardUI_Manager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// カードの見た目を管理するための変数
        /// </summary>
        [SerializeField]
        private Image _card_Image;

        /// <summary>
        /// カードがクリックされたときの処理を管理するための変数
        /// </summary>
        [SerializeField]
        private Button _clickButton;

        /// <summary>
        /// カードのデータを参照する変数
        /// </summary>
        public CardData MyCardData;

        /// <summary>
        /// デッキの管理クラスを参照するための変数
        /// </summary>
        private DeckManager _deckManager;

        /// <summary>
        /// 生成されたカードUIを初期化するための関数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        public void Setup(CardData data, DeckManager manager)
        {
            // --- 各変数を保存 ---
            // カードのデータを保存
            MyCardData = data;
            // デッキマネージャーを保存
            _deckManager = manager;

            // もしカードの画像が存在する場合
            if (_card_Image != null && data.cardImage != null)
            {
                // カードの画像を設定
                _card_Image.sprite = data.cardImage;
            }

            // カードがクリックされたときの処理を登録
            _clickButton.onClick.AddListener(OnClickCard);

            // デッキが変更されたときにカードの表示を更新する関数を登録
            _deckManager.OnDeckChanged += UpdateVisibility;
            // 最初の表示更新
            UpdateVisibility();
        }

        /// <summary>
        /// カードの表示を更新する関数
        /// </summary>
        private void OnDestroy()
        {
            // デッキ管理クラスがある場合
            if (_deckManager != null)
            {
                // 登録を解除する（メモリリーク防止）
                _deckManager.OnDeckChanged -= UpdateVisibility;
            }
        }

        /// <summary>
        /// カードがクリックされたときの処理を管理する関数
        /// </summary>
        private void OnClickCard()
        {
            // デッキマネージャーの関数を呼び出して、カードをデッキに追加
            _deckManager.AddToDeck(MyCardData);
        }

        /// <summary>
        /// カードの表示を更新する関数
        /// </summary>
        private void UpdateVisibility()
        {
            // もしカードのデータやデッキマネージャーが存在しない場合
            if (MyCardData == null || _deckManager == null)
            {
                return;
            }

            // カードがすでにデッキに含まれているかどうかをチェック
            bool isAlready_InDeck = _deckManager.currentDeck.Contains(MyCardData);
            // もしカードがデッキに含まれている場合は非表示にする
            gameObject.SetActive(!isAlready_InDeck);
        }

        /// <summary>
        /// マウスカーソルがカードに入った瞬間に呼び出す関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // もしデータが空の場合
            if (MyCardData == null)
            {
                return;
            }

            // カード詳細を表示
            CardDetailViewer.instance.ShowDetail(MyCardData.cardDetail_Image, this.transform.position);
            }

        /// <summary>
        /// マウスカーソルがカードから出た瞬間に呼び出す関数
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // もしデータが空の場合
            if (MyCardData == null)
            {
                return;
            }

            // カード詳細を非表示
            CardDetailViewer.instance.HideDetail();
        }
    }
}