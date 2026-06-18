using ForestDraw.Enemy.Data;
using UnityEngine;

public interface IEnemyComponent
{
    void Initialize(EnemyInitContext context);
}
public class EnemyInitContext
{
    public Transform[] path;
    public Transform target;
    public EnemyData data;
}