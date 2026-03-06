using UnityEngine;
using ForestDraw.Player.Combat;

using UnityEngine.UI;
public class TestAttack : MonoBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float width;
    [SerializeField]
    private float length;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private Vector3 origin;
    public void SingleAttack()
    {
        PlayerAttack.AttackNearest(target.transform.position,damage);
    }
    public void LineAttack()
    {
        PlayerAttack.AttackLine(target.transform.position,width,damage ,length);
    }
    public void CircleAttack()
    {
        PlayerAttack.AttackCircle(target.transform.position + origin, radius, damage);
    }
    public void AllAttack()
    {
        PlayerAttack.AttackAll(damage);
    }
}
