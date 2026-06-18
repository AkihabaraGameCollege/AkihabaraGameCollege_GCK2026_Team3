using System.Collections.Generic;
using UnityEngine;

namespace ForestDraw
{
    /// <summary>
    /// デッキUIの管理クラス
    /// </summary>
    public class DeckUI_Manager : MonoBehaviour
    {
        /// <summary>
        /// デッキの管理クラス（DeckManager）を参照するための変数
        /// </summary>
        [SerializeField]
        private DeckManager deckManager;

        /// <summary>
        /// デッキスロットUIのリスト変数
        /// </summary>
        [SerializeField]
        private List<DeckSlotUI> deckSlots;

        /// <summary>
        /// 初期化処理の関数
        /// </summary>
        private void Start()
        {
            // スロットをセットアップするループ
            foreach (var slot in deckSlots)
            {
                slot.Setup(deckManager);// デッキマネージャーをスロットにセットアップする
                slot.ClearSlot();// 最初は全部空にする
            }

            deckManager.OnDeckChanged += UpdateDeckUI;// デッキが変更されたときにUIを更新する関数を登録する

            UpdateDeckUI();// 最初のUI更新
        }

        /// <summary>
        /// オブジェクト破棄時の処理の関数
        /// </summary>
        private void OnDestroy()
        {
            // もしDeckManagerが存在する場合
            if (deckManager != null)
            {
                deckManager.OnDeckChanged -= UpdateDeckUI;// 登録を解除する（メモリリーク防止）
            }
        }

        /// <summary>
        /// デッキUIを更新する関数
        /// </summary>
        private void UpdateDeckUI()
        {
            // 9つのスロットの切り替えをループ
            for (int i = 0; i < deckSlots.Count; i++)
            {
                // もしデッキにカードが存在する場合
                if (i < deckManager.currentDeck.Count)
                {
                    deckSlots[i].SetCard(deckManager.currentDeck[i]);// デッキのカードをスロットにセットする
                }
                else
                {
                    deckSlots[i].ClearSlot();// デッキにカードが存在しない場合はスロットを空にする
                }
            }
        }
    }
}