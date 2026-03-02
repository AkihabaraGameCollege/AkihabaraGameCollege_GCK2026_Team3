using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int maxLife = 4000;
    public int life = 4000;

    public int maxCost = 8;
    public int cost = 0;
    public float costRegenPerSecond = 1f;

    public event Action<int> OnCostChanged;
    public event Action<int> OnLifeChanged;

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
        cost = maxCost; // start full
    }

    void Update()
    {
        RegenerateCost();
    }

    void RegenerateCost()
    {
        if (cost >= maxCost) return;
        float toAdd = costRegenPerSecond * Time.deltaTime;
        // accumulate fractional but only apply integer when it changes
        int newCost = Mathf.Min(maxCost, cost + Mathf.FloorToInt(toAdd + 0.0001f));
        if (newCost != cost)
        {
            cost = newCost;
            OnCostChanged?.Invoke(cost);
        }
    }

    public bool TryUseCost(int amount)
    {
        if (amount <= 0) return true;
        if (cost >= amount)
        {
            cost -= amount;
            OnCostChanged?.Invoke(cost);
            return true;
        }
        return false;
    }

    public void RecoverCost(int amount)
    {
        if (amount <= 0) return;
        cost = Mathf.Min(maxCost, cost + amount);
        OnCostChanged?.Invoke(cost);
    }

    public void ChangeLife(int delta)
    {
        life = Mathf.Clamp(life + delta, 0, maxLife);
        OnLifeChanged?.Invoke(life);
    }
}
