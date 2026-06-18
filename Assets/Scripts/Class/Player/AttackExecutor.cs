using UnityEngine;

public class AttackExecutor : MonoBehaviour
{
    public static AttackExecutor Instance;

    private void Awake()
    {
        Instance = this;
    }
}