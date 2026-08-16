using UnityEngine;

namespace ForestDraw
{
    public class AttackExecutor : MonoBehaviour
    {
        public static AttackExecutor Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}