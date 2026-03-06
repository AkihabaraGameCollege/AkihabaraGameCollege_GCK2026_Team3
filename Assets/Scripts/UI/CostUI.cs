using UnityEngine;
using UnityEngine.UI;
using ForestDraw.Player.Combat;

public class CostUI : MonoBehaviour
{
    [SerializeField] private PlayerCost playerCost;
    [SerializeField] private Transform costParent; // コストUIの親

    private Image[] costImages;

    private void Awake()
    {
        // 親の子オブジェクトからImageを取得
        costImages = costParent.GetComponentsInChildren<Image>();
    }

    private void Start()
    {
        playerCost.OnCostChanged += UpdateUI;
    }

    private void UpdateUI(int currentCost, float progress)
    {
        for (int i = 0; i < costImages.Length; i++)
        {
            if (i < currentCost)
            {
                costImages[i].fillAmount = 1f;
            }
            else if (i == currentCost)
            {
                costImages[i].fillAmount = progress;
            }
            else
            {
                costImages[i].fillAmount = 0f;
            }
        }
    }
}