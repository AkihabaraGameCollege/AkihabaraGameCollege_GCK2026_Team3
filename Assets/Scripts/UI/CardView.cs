using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

// 単一カード表示を行うコンポーネント（TextMeshPro と Button を前提）
// UIのボタンから PlayerController.PlayCard を呼び出すためのラッパー
[RequireComponent(typeof(Button))]
public class CardView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text powerText;
    [SerializeField] TMP_Text costText;

    PlayerController.Card boundCard;
    PlayerController ownerPlayer;

    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void Bind(PlayerController player, PlayerController.Card card)
    {
        // 既存のオーナーがいればイベント解除
        if (ownerPlayer != null)
        {
            ownerPlayer.OnLifeChanged -= OnOwnerLifeChanged;
            ownerPlayer.OnHandChanged -= OnOwnerHandChanged;
        }

        ownerPlayer = player;
        boundCard = card;
        if (nameText != null) nameText.text = card != null ? card.displayName : "";
        if (powerText != null) powerText.text = card != null ? card.power.ToString() : "";
        if (costText != null) costText.text = card != null ? card.cost.ToString() : "";

        // プレイヤーの状態変化でボタン状態を更新するためにイベント購読
        if (ownerPlayer != null)
        {
            ownerPlayer.OnLifeChanged += OnOwnerLifeChanged;
            ownerPlayer.OnHandChanged += OnOwnerHandChanged;
        }

        UpdateInteractable();
    }

    void OnOwnerLifeChanged(int currentLife, int maxLife)
    {
        UpdateInteractable();
    }

    void OnOwnerHandChanged()
    {
        UpdateInteractable();
    }

    void UpdateInteractable()
    {
        if (btn == null) return;
        bool inHand = (boundCard != null && ownerPlayer != null && ownerPlayer.GetHand().Contains(boundCard));
        bool hasLife = (boundCard != null && ownerPlayer != null) ? ownerPlayer.CurrentLife >= boundCard.cost : false;
        btn.interactable = inHand && hasLife;
    }

    void OnClick()
    {
        if (ownerPlayer == null || boundCard == null) return;

        // UIから使用する際はターゲットを player.CurrentTarget に渡す
        ownerPlayer.PlayCard(boundCard, ownerPlayer.CurrentTarget);
    }

    void OnDestroy()
    {
        if (ownerPlayer != null)
        {
            ownerPlayer.OnLifeChanged -= OnOwnerLifeChanged;
            ownerPlayer.OnHandChanged -= OnOwnerHandChanged;
        }
    }
}
