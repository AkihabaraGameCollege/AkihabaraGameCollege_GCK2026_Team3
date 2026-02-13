using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのカード操作・ライフ管理を行うコントローラ
public class PlayerController : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] int maxLife = 8; // ライフ最大値（要件: 8）
    [SerializeField] int startLife = 8;
    int currentLife;

    // ライフ変更通知（UI購読用）: 引数 (currentLife, maxLife)
    public event Action<int, int> OnLifeChanged;

    [Header("Card system")]
    [Tooltip("自動でカードを1枚引く間隔（秒）")]
    [SerializeField] float autoDrawInterval = 3.0f;

    [Tooltip("手札の最大枚数（要件: 8）")]
    [SerializeField] int maxHandSize = 8;

    // デッキ / 手札 / 捨て山（シンプル実装）
    [SerializeField] List<Card> deck = new List<Card>();
    List<Card> hand = new List<Card>();
    List<Card> discard = new List<Card>();

    // デバッグ用にInspectorで簡易カードを作るためのフラグ
    [Header("Debug / Test")]
    [SerializeField] bool createTestDeck = false;
    [SerializeField] int testDeckSize = 10;

    void Start()
    {
        // ライフ初期化
        currentLife = Mathf.Clamp(startLife, 0, maxLife);
        OnLifeChanged?.Invoke(currentLife, maxLife);

        // テストデッキを必要なら作成
        if (createTestDeck && deck.Count == 0)
        {
            CreateTestDeck(testDeckSize);
        }

        // デッキをシャッフル
        ShuffleDeck();

        // 自動ドロー開始
        StartCoroutine(AutoDrawRoutine());
    }

    // ライフを減らす（敵からの攻撃で呼ぶ）
    public void ReceiveEnemyAttack()
    {
        if (currentLife <= 0) return;

        currentLife = Mathf.Max(0, currentLife - 1);
        OnLifeChanged?.Invoke(currentLife, maxLife);

        if (currentLife <= 0)
        {
            // ゲームオーバーは GameManager 経由で
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }
    }

    // --- カード管理 ---

    // 手札から指定カードを使用して効果を発動する（ターゲットは敵Transform）
    // UI側からドラッグで敵にドロップする等の際に呼び出す想定
    public bool PlayCard(Card card, Transform target)
    {
        if (card == null) return false;
        if (!hand.Contains(card)) return false;

        // カード効果の適用（簡易）
        switch (card.type)
        {
            case CardType.Damage:
                if (target != null)
                {
                    var tm = target.GetComponent<StatusManager>();
                    if (tm != null)
                    {
                        // StatusManager.Damage(int damage, Vector3 hitPos, CriticalType type, Transform attacker)
                        tm.Damage(card.power, transform.position, CriticalType.Normal, this.transform);
                    }
                }
                break;

            case CardType.RecoverCost:
                // コスト回復などの処理。ゲーム内のエナジー管理と接続してください。
                // （このサンプルではログ出力）
                Debug.Log($"カード効果: コスト回復 {card.power}");
                break;

            case CardType.Buff:
                // 一時バフなど。プロジェクトのステータス管理と接続してください。
                Debug.Log($"カード効果: バフ {card.power}");
                break;
        }

        // カード使用後は手札から外し、捨てずに「デッキに戻す」要件に合わせてデッキの底に返却
        hand.Remove(card);
        AddCardToBottomOfDeck(card);

        // TODO: UI更新イベントをここで呼ぶ（OnHandChanged など）
        return true;
    }

    // 手札にカードを引く（UIや自動で呼ぶ）
    public Card DrawCard()
    {
        if (hand.Count >= maxHandSize) return null;

        if (deck.Count == 0)
        {
            // デッキが空なら捨て山を戻してシャッフル
            if (discard.Count > 0)
            {
                deck.AddRange(discard);
                discard.Clear();
                ShuffleDeck();
            }
            else
            {
                // 引くカードがない
                return null;
            }
        }

        Card c = deck[0];
        deck.RemoveAt(0);
        hand.Add(c);

        // TODO: UI更新（手札表示）をここで通知
        return c;
    }

    // カードをデッキ最下部に追加（使用後に戻す要件）
    void AddCardToBottomOfDeck(Card card)
    {
        if (card == null) return;
        deck.Add(card);
    }

    // ランダムシャッフル（Fisher–Yates）
    void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var tmp = deck[k];
            deck[k] = deck[n];
            deck[n] = tmp;
        }
    }

    // 自動ドローコルーチン（3秒ごとに1枚、手札上限8で停止）
    IEnumerator AutoDrawRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoDrawInterval);
            if (hand.Count < maxHandSize)
            {
                DrawCard();
            }
        }
    }

    // --- ヘルパー / デバッグ ---

    // テスト用：単純なカードを作ってデッキに追加
    void CreateTestDeck(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var c = new Card
            {
                id = $"C_{i}",
                displayName = $"Damage {i % 3 + 1}",
                type = CardType.Damage,
                cost = 1,
                power = (i % 3) + 1
            };
            deck.Add(c);
        }
    }

    // 手札・デッキ情報を取得（UIバインド用）
    public IReadOnlyList<Card> GetHand() => hand.AsReadOnly();
    public IReadOnlyList<Card> GetDeck() => deck.AsReadOnly();

    // カードをUIからキャンセルしたときなど、手札から捨てる／捨て山へ入れる処理
    public void DiscardFromHand(Card card)
    {
        if (card == null) return;
        if (!hand.Contains(card)) return;
        hand.Remove(card);
        discard.Add(card);
        // TODO: UI更新
    }

    // カードデータ定義（簡易）
    [Serializable]
    public class Card
    {
        public string id;
        public string displayName;
        public CardType type;
        public int cost;
        public int power;
    }

    public enum CardType
    {
        Damage,
        RecoverCost,
        Buff
    }
}