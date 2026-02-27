using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private int damage;
    private GameObject target;

    public void Initialize(GameObject t, int attackdamage)
    {
        target = t;
        damage = attackdamage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            //Destroy(gameObject);
        }
    }
}