using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [Header("Deck")]
    public List<CardData> deck = new List<CardData>();
    public List<CardData> hand = new List<CardData>();
    public int handSize = 9;

    [Header("Effects")]
    public ParticleSystem attackEffectPrefab;
    public ParticleSystem supportEffectPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ShuffleDeck();
        DrawInitialHand();
    }

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

    public void DrawInitialHand()
    {
        hand.Clear();
        for (int i = 0; i < handSize; i++) DrawCard();
    }

    public CardData DrawCard()
    {
        if (deck.Count == 0) return null;
        var top = deck[0];
        deck.RemoveAt(0);
        hand.Add(top);
        return top;
    }

    public bool PlayCard(CardData card)
    {
        if (card == null) return false;
        if (!hand.Contains(card)) return false;

        if (!PlayerManager.Instance.TryUseCost(card.cost)) return false;

        // apply effect (simple demo)
        ApplyCardEffect(card);

        hand.Remove(card);

        // カードの種類に応じてパーティクルを再生
        SpawnEffectForCard(card);

        return true;
    }

    void ApplyCardEffect(CardData card)
    {
        switch (card.effectType)
        {
            case CardEffectType.DamageSingle:
                // find closest enemy and deal random damage
                var enemy = FindClosestEnemyOnScreen();
                if (enemy != null)
                {
                    int dmg = Random.Range(card.minDamage, card.maxDamage + 1);
                    enemy.TakeDamage(dmg);
                }
                break;
            case CardEffectType.CostRecover:
                PlayerManager.Instance.RecoverCost(card.minDamage);
                break;
            case CardEffectType.Draw:
                DrawCard();
                DrawCard();
                break;
            case CardEffectType.Heal:
                PlayerManager.Instance.ChangeLife(card.minDamage);
                break;
            default:
                Debug.Log("Unhandled card effect: " + card.effectType);
                break;
        }
    }

    Enemy FindClosestEnemyOnScreen()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float best = float.MaxValue;
        Camera cam = Camera.main;
        if (cam == null) return null;
        foreach (var e in enemies)
        {
            Vector3 sp = cam.WorldToScreenPoint(e.transform.position);
            if (sp.z < 0) continue; // behind camera
            // check if on screen
            if (sp.x < 0 || sp.x > Screen.width || sp.y < 0 || sp.y > Screen.height) continue;
            float d = (cam.transform.position - e.transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                closest = e;
            }
        }
        return closest;
    }

    void SpawnEffectForCard(CardData card)
    {
        if (card == null) return;

        // 攻撃カードはターゲット（一番近い敵）位置で再生
        if (card.cardType == CardType.Attack)
        {
            var enemy = FindClosestEnemyOnScreen();
            if (enemy != null && attackEffectPrefab != null)
            {
                SpawnParticleAt(attackEffectPrefab, enemy.transform.position);
                return;
            }

            // 敵がいない or プレハブ未設定なら画面中心で再生（フォールバック）
            if (attackEffectPrefab != null)
            {
                var cam = Camera.main;
                Vector3 pos = cam != null ? cam.transform.position + cam.transform.forward * 5f : Vector3.zero;
                SpawnParticleAt(attackEffectPrefab, pos);
            }
        }

        // サポートカードはプレイヤー位置で再生（未設定時はカメラ前方で再生）
        if (card.cardType == CardType.Support)
        {
            if (supportEffectPrefab != null)
            {
                Vector3 pos;
                if (PlayerManager.Instance != null)
                {
                    pos = PlayerManager.Instance.transform.position;
                }
                else
                {
                    var cam = Camera.main;
                    pos = cam != null ? cam.transform.position + cam.transform.forward * 3f : Vector3.zero;
                }
                SpawnParticleAt(supportEffectPrefab, pos);
            }
        }
    }

    void SpawnParticleAt(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null) return;
        var instance = Instantiate(prefab, position, Quaternion.identity);
        // duration を取得して自動破棄（ループ設定のあるエフェクトは手動管理推奨）
        var main = instance.main;
        float dur = main.duration;
        // safety: もしループしている場合は自動破棄しない
        if (!main.loop)
        {
            Destroy(instance.gameObject, dur + 0.5f);
        }
    }
}