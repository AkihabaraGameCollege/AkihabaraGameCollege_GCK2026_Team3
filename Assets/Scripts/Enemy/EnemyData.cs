using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("ステータス")]
    public int maxHealth;
    public int takeDamageInterval;

    [Header("移動設定")]
    public float moveSpeed;
    public float waypointRadius;

    [Header("攻撃設定")]
    public int attackDamage;
    public float attackInterval;
}