using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public List<Enemy> enemies = new List<Enemy>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public List<Enemy> GetEnemiesInScreen(Camera cam)
    {
        List<Enemy> result = new List<Enemy>();

        foreach (var enemy in enemies)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(enemy.transform.position);
            if (viewPos.x > 0 && viewPos.x < 1 &&
                viewPos.y > 0 && viewPos.y < 1 &&
                viewPos.z > 0)
            {
                result.Add(enemy);
            }
        }
        return result;
    }
}