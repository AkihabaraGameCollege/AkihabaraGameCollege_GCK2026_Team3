using System.Collections.Generic;
using UnityEngine;

// カードの管理（デッキ・手札・使用処理）を行うクラス
public class CardManager : MonoBehaviour
{
    // シングルトン（どこからでも CardManager.Instance でアクセス可能）
    public static CardManager Instance { get; private set; }

    [Header("Deck")]
    // 山札（ゲーム開始時にセットしておく）
    public List<CardData> deck = new List<CardData>();

    // 現在の手札
    public List<CardData> hand = new List<CardData>();

    // 初期手札枚数
    public int handSize = 9;

    [Header("Effects")]
    // 攻撃用パーティクル
    public ParticleSystem attackEffectPrefab;

    // サポート用パーティクル
    public ParticleSystem supportEffectPrefab;

    void Awake()
    {
        // シングルトンの重複防止
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // ゲーム開始時にデッキをシャッフルし、初期手札を配る
        ShuffleDeck();
        DrawInitialHand();
    }

    // 山札をランダムに並び替える（簡易シャッフル）
    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(i, deck.Count);
            var tmp = deck[i];
            deck[i] = deck[r];
            deck[r] = tmp;
        }
    }

    // 初期手札を配る
    public void DrawInitialHand()
    {
        hand.Clear(); // 手札をリセット
        for (int i = 0; i < handSize; i++) DrawCard();
    }

    // 山札の一番上からカードを引く
    public CardData DrawCard()
    {
        if (deck.Count == 0) return null; // 山札が空なら引けない

        var top = deck[0];       // 一番上のカード
        deck.RemoveAt(0);        // 山札から削除
        hand.Add(top);           // 手札に追加
        return top;
    }

    // カードを使用する処理
    public bool PlayCard(CardData card)
    {
        if (card == null) return false;            // nullチェック
        if (!hand.Contains(card)) return false;    // 手札にないカードは使えない

        // コストが足りない場合は使用不可
        if (!PlayerManager.Instance.TryUseCost(card.cost)) return false;

        // カード効果を適用
        ApplyCardEffect(card);

        // 使用後は手札から削除
        hand.Remove(card);

        // カードの種類に応じてパーティクルを再生
        SpawnEffectForCard(card);

        return true;
    }

    // カード効果を実際に適用する処理
    void ApplyCardEffect(CardData card)
    {
        switch (card.effectType)
        {
            case CardEffectType.DamageSingle:
                // 画面内で一番近い敵を探してダメージ
                var enemy = FindClosestEnemyOnScreen();
                if (enemy != null)
                {
                    // min〜maxの間でランダムダメージ
                    int dmg = Random.Range(card.minDamage, card.maxDamage + 1);
                    enemy.TakeDamage(dmg);
                }
                break;

            case CardEffectType.CostRecover:
                // コスト回復
                PlayerManager.Instance.RecoverCost(card.minDamage);
                break;

            case CardEffectType.Draw:
                // 2枚ドロー
                DrawCard();
                DrawCard();
                break;

            case CardEffectType.Heal:
                // 体力回復
                PlayerManager.Instance.ChangeLife(card.minDamage);
                break;

            default:
                // 未対応の効果
                Debug.Log("Unhandled card effect: " + card.effectType);
                break;
        }
    }

    // 画面内に表示されている敵の中で一番近い敵を探す
    Enemy FindClosestEnemyOnScreen()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        Enemy closest = null;
        float best = float.MaxValue;

        Camera cam = Camera.main;
        if (cam == null) return null;

        foreach (var e in enemies)
        {
            // ワールド座標 → スクリーン座標へ変換
            Vector3 sp = cam.WorldToScreenPoint(e.transform.position);

            if (sp.z < 0) continue; // カメラの後ろは除外

            // 画面内かチェック
            if (sp.x < 0 || sp.x > Screen.width || sp.y < 0 || sp.y > Screen.height) continue;

            // カメラとの距離（平方距離で計算コスト軽減）
            float d = (cam.transform.position - e.transform.position).sqrMagnitude;

            if (d < best)
            {
                best = d;
                closest = e;
            }
        }

        return closest;
    }

    // カードの種類に応じてエフェクトを再生
    void SpawnEffectForCard(CardData card)
    {
        if (card == null) return;

        // 攻撃カードの場合
        if (card.cardType == CardType.Attack)
        {
            var enemy = FindClosestEnemyOnScreen();

            // 敵が存在する場合は敵の位置で再生
            if (enemy != null && attackEffectPrefab != null)
            {
                SpawnParticleAt(attackEffectPrefab, enemy.transform.position);
                return;
            }

            // フォールバック（カメラ前方）
            if (attackEffectPrefab != null)
            {
                var cam = Camera.main;
                Vector3 pos = cam != null ? cam.transform.position + cam.transform.forward * 5f : Vector3.zero;
                SpawnParticleAt(attackEffectPrefab, pos);
            }
        }

        // サポートカードの場合
        if (card.cardType == CardType.Support)
        {
            if (supportEffectPrefab != null)
            {
                Vector3 pos;

                // プレイヤー位置で再生
                if (PlayerManager.Instance != null)
                {
                    pos = PlayerManager.Instance.transform.position;
                }
                else
                {
                    // プレイヤーがいない場合はカメラ前方
                    var cam = Camera.main;
                    pos = cam != null ? cam.transform.position + cam.transform.forward * 3f : Vector3.zero;
                }

                SpawnParticleAt(supportEffectPrefab, pos);
            }
        }
    }

    // 指定位置にパーティクルを生成
    void SpawnParticleAt(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null) return;

        var instance = Instantiate(prefab, position, Quaternion.identity);

        // パーティクルの再生時間を取得
        var main = instance.main;
        float dur = main.duration;

        // ループしていない場合のみ自動削除
        if (!main.loop)
        {
            Destroy(instance.gameObject, dur + 0.5f);
        }
    }
}