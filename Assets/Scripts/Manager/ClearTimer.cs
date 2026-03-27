using UnityEngine;
using TMPro;
using ForestDraw;

public class ClearTimer : MonoBehaviour
{
    [Header("制限時間（秒）")]
    [SerializeField] private float clearTime = 60f;

    [Header("UI表示")]
    [SerializeField] private TMP_Text timeText;

    private float currentTime = 0f;
    private bool isCleared = false;

    void Start()
    {
        currentTime = clearTime;
        UpdateText();
    }

    void Update()
    {
        if (isCleared) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCleared = true;
            UpdateText(); 
            Debug.Log("ステージクリア！！！！！");
            StageScene.Instance.InClearScene();
            return;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}