using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public int maxLife = 4000;
    public int currentLife;

    public int maxCost = 8;
    public int currentCost;

    private float damageMultiplier = 1f;
    private float damageReduction = 0f;
    private int costRegenAmount = 1;

    void Start()
    {
        currentLife = maxLife;
        currentCost = maxCost;
        StartCoroutine(CostRecovery());
    }

    IEnumerator CostRecovery()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            AddCost(costRegenAmount);
        }
    }

    public bool UseCost(int value)
    {
        if (currentCost < value) return false;
        currentCost -= value;
        return true;
    }

    public void AddCost(int value)
    {
        currentCost += value;
        if (currentCost > maxCost)
            currentCost = maxCost;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.RoundToInt(damage * (1f - damageReduction));
        currentLife -= finalDamage;
        if (currentLife <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    public void Heal(int amount)
    {
        currentLife += amount;
        if (currentLife > maxLife)
            currentLife = maxLife;
    }

    public int CalculateDamage(int min, int max)
    {
        int baseDamage = Random.Range(min, max + 1);
        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        damageMultiplier = 1f; // çUåÇå„ÉäÉZÉbÉg
        return finalDamage;
    }

    public void ApplyDamageBoost(float value)
    {
        damageMultiplier = value;
    }

    public void ApplyDamageReduction(float value, float duration)
    {
        StartCoroutine(DamageReductionRoutine(value, duration));
    }

    IEnumerator DamageReductionRoutine(float value, float duration)
    {
        damageReduction = value;
        yield return new WaitForSeconds(duration);
        damageReduction = 0f;
    }

    public void ApplyCostRegenBoost(int value, float duration)
    {
        StartCoroutine(CostRegenRoutine(value, duration));
    }

    IEnumerator CostRegenRoutine(int value, float duration)
    {
        costRegenAmount = value;
        yield return new WaitForSeconds(duration);
        costRegenAmount = 1;
    }
}