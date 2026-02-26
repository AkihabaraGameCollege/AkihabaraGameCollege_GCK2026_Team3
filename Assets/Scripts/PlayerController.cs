using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのカード操作・ライフ管理を行うコントローラ
public class PlayerController : MonoBehaviour
{
    [Header("ライフ設定")]
    [SerializeField, InspectorName("最大ライフ")] int maxLife = 8; // ライフ最大値（要件: 8）
    [SerializeField, InspectorName("開始ライフ")] int startLife = 8;
    int currentLife;

    int currentShield = 0;

    // ライフ変更通知（UI購読用）: 引数 (currentLife, maxLife)
    public event Action<int, int> OnLifeChanged;
    // シールド変更通知: 引数 (currentShield)
    public event Action<int> OnShieldChanged;

    // 手札変更通知（UI購読用）
    public event Action OnHandChanged;

    [Header("カード設定")]
    [Tooltip("自動でカードを1枚引く間隔（秒）")]
    [SerializeField, InspectorName("自動ドロー間隔（秒）")] float autoDrawInterval = 3.0f;

    [Tooltip("手札の最大枚数（要件: 8）")]
    [SerializeField, InspectorName("手札の最大枚数")] int maxHandSize = 8;

    // デッキ / 手札 / 捨て山（シンプル実装）
    [SerializeField, InspectorName("デッキ（カード一覧）")] List<Card> deck = new List<Card>();
    List<Card> hand = new List<Card>();
    List<Card> discard = new List<Card>();

    // --- 支援／バフ関連 ---
    // 次に使う攻撃カードに乗る倍率（1.0 がデフォルト）
    float nextAttackMultiplier = 1.0f;

    // 次に使う攻撃の基礎ダメージ上書き（-1 = なし）
    int nextAttackBaseOverride = -1;

    // コスト回復に対する一時倍率（エネルギー実装があれば利用）
    float costRecoveryMultiplier = 1.0f;
    Coroutine costRecoveryCoroutine = null;

    // デバッグ用にInspectorで簡易カードを作るためのフラグ
    [Header("デバッグ / テスト設定")]
    [SerializeField, InspectorName("テスト用デッキを自動作成")] bool createTestDeck = false;
    [SerializeField, InspectorName("テストデッキ枚数")] int testDeckSize = 10;

    // -----------------------
    // ターゲット管理
    // -----------------------
    // 現在プレイヤーがロックしているターゲット（EnemyのTransform）
    private Transform currentTarget;
    public Transform CurrentTarget => currentTarget;

    // ターゲット変更通知（UIやエフェクト向け）
    public event Action<Transform> OnTargetChanged;

    // 明示的にターゲットをセットする（敵以外は無視）
    public bool SetTarget(Transform t)
    {
        if (t == null)
        {
            ClearTarget();
            return false;
        }

        // 敵であることを簡易判定（StatusManager がついているか、タグが Enemy）
        if (t.GetComponent<StatusManager>() == null && !t.CompareTag("Enemy"))
        {
            return false;
        }

        currentTarget = t;
        OnTargetChanged?.Invoke(currentTarget);
        return true;
    }

    // ターゲット解除
    public void ClearTarget()
    {
        currentTarget = null;
        OnTargetChanged?.Invoke(null);
    }

    // 指定距離内の最寄りの Enemy を探して返す（見つからなければ null）
    public Transform FindNearestEnemy(float maxDistance = 50f)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform best = null;
        float bestDist = maxDistance;

        Vector3 myPos = transform.position;
        foreach (var go in enemies)
        {
            if (go == null) continue;
            float d = Vector3.Distance(myPos, go.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = go.transform;
            }
        }

        return best;
    }

    // 最寄りのエネミーを選択してターゲットにセット（成功可否を返す）
    public bool SelectNearestEnemy(float maxDistance = 50f)
    {
        var t = FindNearestEnemy(maxDistance);
        if (t != null)
        {
            return SetTarget(t);
        }
        return false;
    }

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

    // 次の攻撃の基礎ダメージを上書きする（SupportCard 等から呼ぶ）
    public void ApplyNextAttackBaseOverride(int baseDamage)
    {
        nextAttackBaseOverride = baseDamage;
    }

    // AttackCard が呼ぶ: 上書き値を取得してリセット
    public int GetAndConsumeNextAttackBaseOverride()
    {
        int v = nextAttackBaseOverride;
        nextAttackBaseOverride = -1;
        return v;
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

        // コスト検査: カードにコストが設定されていれば先に支払う（ライフで支払う設計）
        if (card.cost > 0)
        {
            if (!ConsumeLife(card.cost))
            {
                // UIフィードバックとしてイベントやログで通知。ここではログ出力。
                Debug.Log($"PlayCard: コスト不足のためカード使用キャンセル (cost:{card.cost}, currentLife:{currentLife})");
                return false;
            }
        }

        bool applied = false;

        // カードにプレハブ参照があり、AttackCard コンポーネントを持つならそれを実行
        if (card.prefab != null)
        {
            var go = Instantiate(card.prefab);
            var attackComp = go.GetComponent<AttackCard>();
            var supportComp = go.GetComponent<SupportCard>();

            if (attackComp != null && card.type == CardType.Damage)
            {
                // overrideBaseDamage にカードの power を渡すことでカードデータと挙動を結びつける
                attackComp.Execute(this.transform, target, this, charge: false, lifeSacrifice: 0, lifeSacrificeMultiplier: 1.0f, overrideBaseDamage: card.power);
                applied = true;
            }
            else if (supportComp != null)
            {
                // プレハブに SupportCard があっても、どの効果を使うかはカードの type に依存する。
                // ここでは簡易に type に応じた代表的な効果を呼ぶ（ライフは既に支払われているため lifeCost=0 を渡す）
                switch (card.type)
                {
                    case CardType.RecoverCost:
                        supportComp.ApplyCostRecoveryBuff(this, lifeCost: 0, multiplier: 1.0f + (card.power * 0.1f), duration: 5.0f);
                        applied = true;
                        break;
                    case CardType.Buff:
                        supportComp.BoostNextAttack(this, lifeCost: 0, multiplier: 1.0f + (card.power * 0.1f));
                        applied = true;
                        break;
                    default:
                        Debug.Log("PlayCard: SupportCard prefab だが対応する type がないため効果を適用できませんでした。");
                        applied = false;
                        break;
                }
            }

            // 使い捨てプレハブはシーン上に残さない
            Destroy(go);
        }
        else
        {
            // 既存のシンプルな効果適用（プレハブ未設定）
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
                            applied = true;
                        }
                    }
                    break;

                case CardType.RecoverCost:
                    Debug.Log($"カード効果: コスト回復 {card.power} (プレハブ未設定)");
                    applied = true;
                    break;

                case CardType.Buff:
                    Debug.Log($"カード効果: バフ {card.power} (プレハブ未設定)");
                    applied = true;
                    break;
            }
        }

        if (!applied)
        {
            // 効果未適用の場合はコストを払い戻す（消費前に支払い済みなら戻す）
            if (card.cost > 0)
            {
                // 返却処理: ライフに戻す
                currentLife = Mathf.Min(maxLife, currentLife + card.cost);
                OnLifeChanged?.Invoke(currentLife, maxLife);
            }
            Debug.Log("PlayCard: カード効果の適用に失敗しました。");
            return false;
        }

        // カード使用後は手札から外し、捨てずに「デッキに戻す」要件に合わせてデッキの底に返却
        hand.Remove(card);
        AddCardToBottomOfDeck(card);

        // UI更新イベント
        OnHandChanged?.Invoke();

        return true;
    }

    // 新規: 手札へ直接カードを追加（重複コピーや支援効果からの追加に使用）
    // 戻り値: true = 追加成功、false = 手札が満杯
    public bool AddCardToHand(Card card)
    {
        if (card == null) return false;
        if (hand.Count >= maxHandSize) return false;
        hand.Add(card);
        // UI 更新
        OnHandChanged?.Invoke();
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

        // UI更新（手札表示）をここで通知
        OnHandChanged?.Invoke();
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
                power = (i % 3) + 1,
                prefab = null
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
        // UI更新
        OnHandChanged?.Invoke();
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
        // 追加: プレハブ参照を持たせてカードデータと振る舞いを結び付けられるようにする
        public GameObject prefab;
    }

    public enum CardType
    {
        Damage,
        RecoverCost,
        Buff
    }
}