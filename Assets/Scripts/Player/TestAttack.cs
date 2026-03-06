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
    [SerializeField]
    private int useCost = 2;
    [SerializeField]
    private int recoverCost = 5;
    [SerializeField]
    private int timeCost = 2;
    [SerializeField]
    private float recoverCostInterval = 10;
    [SerializeField]
    private PlayerCost playerCost;
    public void SingleAttack()
    {
       if (!playerCost.UseCost(useCost)) return;
        PlayerAttack.AttackNearest(target.transform.position,damage);
    }
    public void LineAttack()
    {
        if (!playerCost.UseCost(useCost)) return;
        PlayerAttack.AttackLine(target.transform.position,width,damage ,length);
    }
    public void CircleAttack()
    {
        if (!playerCost.UseCost(useCost)) return;
        PlayerAttack.AttackCircle(target.transform.position + origin, radius, damage);
    }
    public void AllAttack()
    {
        if (!playerCost.UseCost(useCost)) return;
        PlayerAttack.AttackAll(damage);
    }
    public void CostRecover()
    {
        playerCost.RecoverCost(recoverCost);
    }

    public void CostRecovertime() 
    {
        playerCost.ReduceRecoverInterval(timeCost,recoverCostInterval);
    }
}
