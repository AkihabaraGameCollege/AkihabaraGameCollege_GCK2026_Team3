using UnityEngine;
using ForestDraw.Player.Combat;

public class CostUI : MonoBehaviour
{
    [SerializeField] private PlayerCost playerCost;
    [SerializeField] private Transform costParent;

    private GameObject[] costObjects;

    private void Awake()
    {
        int count = costParent.childCount;
        costObjects = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            costObjects[i] = costParent.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        playerCost.OnCostChanged += UpdateUI;
    }

    private void UpdateUI(int currentCost, float progress)
    {
        for (int i = 0; i < costObjects.Length; i++)
        {
            // ƒRƒXƒg‚ª—­‚Ü‚Á‚Ä‚¢‚é•ª‚¾‚¯ON
            costObjects[i].SetActive(i < currentCost);
        }
    }
}