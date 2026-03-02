using System;
using UnityEngine;

// プレイヤーのHP・コスト管理を行うクラス
// シングルトンとして実装し、どこからでもアクセス可能にしている
public class PlayerManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static PlayerManager Instance { get; private set; }

    // 最大HP
    public int maxLife = 4000;

    // 現在HP
    public int life = 4000;

    // 最大コスト（エネルギー）
    public int maxCost = 8;

    // 現在コスト
    public int cost = 0;

    // 1秒あたりのコスト回復量
    public float costRegenPerSecond = 1f;

    // コストが変化したときに通知するイベント
    public event Action<int> OnCostChanged;

    // HPが変化したときに通知するイベント
    public event Action<int> OnLifeChanged;

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
        // ゲーム開始時はコスト満タン
        cost = maxCost;
    }

    void Update()
    {
        // 毎フレームコスト回復処理を行う
        RegenerateCost();
    }

    // コストを自動回復させる処理
    void RegenerateCost()
    {
        // すでに最大なら回復しない
        if (cost >= maxCost) return;

        // フレームごとの回復量を計算（Time.deltaTimeで時間依存）
        float toAdd = costRegenPerSecond * Time.deltaTime;

        // 小数を扱うためFloorToIntで整数化
        // 0.0001fは誤差対策
        int newCost = Mathf.Min(maxCost, cost + Mathf.FloorToInt(toAdd + 0.0001f));

        // コストが変化した場合のみ更新＆通知
        if (newCost != cost)
        {
            cost = newCost;
            OnCostChanged?.Invoke(cost);
        }
    }

    // コストを消費する（足りるかチェック）
    public bool TryUseCost(int amount)
    {
        // 0以下なら消費なし扱い
        if (amount <= 0) return true;

        // コストが足りる場合
        if (cost >= amount)
        {
            cost -= amount;

            // UIなどに通知
            OnCostChanged?.Invoke(cost);
            return true;
        }

        // 足りない場合は失敗
        return false;
    }

    // コストを回復させる処理
    public void RecoverCost(int amount)
    {
        if (amount <= 0) return;

        // 最大値を超えないように制限
        cost = Mathf.Min(maxCost, cost + amount);

        // UIなどに通知
        OnCostChanged?.Invoke(cost);
    }

    // HPを増減させる処理（回復・ダメージ両対応）
    public void ChangeLife(int delta)
    {
        // 0～最大HPの範囲に制限
        life = Mathf.Clamp(life + delta, 0, maxLife);

        // UIなどに通知
        OnLifeChanged?.Invoke(life);
    }
}