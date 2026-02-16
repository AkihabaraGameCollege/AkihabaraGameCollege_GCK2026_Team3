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

    int currentShield = 0;

    // ライフ変更通知（UI購読用）: 引数 (currentLife, maxLife)
    public event Action<int, int> OnLifeChanged;
    // シールド変更通知: 引数 (currentShield)
    public event Action<int> OnShieldChanged;

    [Header("Card system")]
    [Tooltip("自動でカードを1枚引く間隔（秒）")]
    [SerializeField] float autoDrawInterval = 3.0f;

    [Tooltip("手札の最大枚数（要件: 8）")]
    [SerializeField] int maxHandSize = 8;

    // デッキ / 手札 / 捨て山（シンプル実装）
    [SerializeField] List<Card> deck = new List<Card>();
    List<Card> hand = new List<Card>();
    List<Card> discard = new List<Card>();

    // --- 支援／バフ関連 ---
    // 次に使う攻撃カードに乗る倍率（1.0 がデフォルト）
    float nextAttackMultiplier = 1.0f;

    // コスト回復に対する一時倍率（エネルギー実装があれば利用）
    float costRecoveryMultiplier = 1.0f;
    Coroutine costRecoveryCoroutine = null;

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

        // まずシールドがあれば吸収
        if (currentShield > 0)
        {
            currentShield = Mathf.Max(0, currentShield - 1);
            OnShieldChanged?.Invoke(currentShield);
            return;
        }

        currentLife = Mathf.Max(0, currentLife - 1);
        OnLifeChanged?.Invoke(currentLife, maxLife);

        if (currentLife <= 0)
        {
            // ゲームオーバーは GameManager 経由で
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }
    }

    // 追加: プレイヤーのライフを指定分消費する（カード効果等で使用）
    // 戻り値: true = 消費成功、false = ライフ不足で失敗（消費なし）
    public bool ConsumeLife(int amount)
    {
        if (amount <= 0) return true;
        if (currentLife - amount < 0) return false;

        currentLife = Mathf.Max(0, currentLife - amount);
        OnLifeChanged?.Invoke(currentLife, maxLife);

        if (currentLife <= 0)
        {
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }

        return true;
    }

    // 追加: 現在ライフを公開
    public int CurrentLife => currentLife;

    // --- 支援関連 API ---

    // 次の攻撃に乗る倍率を追加（乗算）
    public void ApplyNextAttackMultiplier(float multiplier)
    {
        if (multiplier <= 0f) return;
        nextAttackMultiplier *= multiplier;
    }

    // AttackCard 側が呼んで倍率を取得しリセットする
    public float GetAndConsumeNextAttackMultiplier()
    {
        float m = nextAttackMultiplier;
        nextAttackMultiplier = 1.0f;
        return m;
    }

    // シールド追加
    public void AddShield(int amount)
    {
        if (amount <= 0) return;
        currentShield += amount;
        OnShieldChanged?.Invoke(currentShield);
    }

    // ライフ回復（ヒール）
    public void HealLife(int amount)
    {
        if (amount <= 0) return;
        currentLife = Mathf.Min(maxLife, currentLife + amount);
        OnLifeChanged?.Invoke(currentLife, maxLife);
    }

    // コスト回復力バフ（duration 秒だけ multiplier を掛ける）
    public void ApplyCostRecoveryBuff(float multiplier, float duration)
    {
        if (multiplier <= 0f || duration <= 0f) return;
        if (costRecoveryCoroutine != null) StopCoroutine(costRecoveryCoroutine);
        costRecoveryCoroutine = StartCoroutine(CostRecoveryBuffRoutine(multiplier, duration));
    }

    IEnumerator CostRecoveryBuffRoutine(float multiplier, float duration)
    {
        float prev = costRecoveryMultiplier;
        costRecoveryMultiplier *= multiplier;
        // TODO: イベントで UI 更新が必要なら通知
        yield return new WaitForSeconds(duration);
        costRecoveryMultiplier = prev;
        costRecoveryCoroutine = null;
        // TODO: イベントで UI 更新が必要なら通知
    }

    // 外部から現在のコスト回復倍率を取得する API（エネルギー実装があれば利用）
    public float CurrentCostRecoveryMultiplier => costRecoveryMultiplier;

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

    // 新規: 手札へ直接カードを追加（重複コピーや支援効果からの追加に使用）
    // 戻り値: true = 追加成功、false = 手札が満杯
    public bool AddCardToHand(Card card)
    {
        if (card == null) return false;
        if (hand.Count >= maxHandSize) return false;
        hand.Add(card);
        // TODO: UI更新
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