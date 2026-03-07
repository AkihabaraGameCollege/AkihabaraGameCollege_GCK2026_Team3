using UnityEngine;
using ForestDraw.Player.Combat;
using ForestDraw;

public class TestAttack : MonoBehaviour
{
    [SerializeField]
    private CardData data1;
    [SerializeField]
    private CardData data2;
    [SerializeField]
    private CardData data3;
    [SerializeField]
    private CardData data4;
    [SerializeField]
    private CardData data5;

    [SerializeField]
    private Transform player;
    
    public void UseCard1()
    {
        CardUseExecutor.Execute(data1, player);
    }
    public void UseCard2()
    {
        CardUseExecutor.Execute(data2, player);
    }
    public void UseCard3()
    {
        CardUseExecutor.Execute(data3, player);
    }
    public void UseCard4()
    {
        CardUseExecutor.Execute(data4, player);
    }
    public void UseCard5()
    {
        CardUseExecutor.Execute(data5, player);
    }
    
}
