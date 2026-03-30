using ForestDraw.Combat;
using UnityEngine;

namespace ForestDraw.Enemy.Attack
{
    public class Explosion : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<IDamageable>()?.TakeDamage(int.MaxValue);
            Destroy(gameObject);
        }
    }
}
