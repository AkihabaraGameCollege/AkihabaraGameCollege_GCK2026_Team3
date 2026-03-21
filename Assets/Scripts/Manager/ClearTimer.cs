using UnityEngine;
using TMPro;

public class ClearTimer : MonoBehaviour
{
    [Header("制限時間（秒）")]
    [SerializeField] private float clearTime = 60f;

    [Header("UI表示")]
    [SerializeField] private TMP_Text timeText;

    private float currentTime = 0f;
    private bool isCleared = false;


    void Update()
    {
        if (isCleared) return;

        currentTime += Time.deltaTime;

        if (currentTime >= clearTime)
        {
            currentTime = clearTime;
            isCleared = true;
            UpdateText(); 
            Debug.Log("ステージクリア！！！！！");
            // StageScene.Instance.StageClear();
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