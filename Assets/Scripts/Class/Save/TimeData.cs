using TMPro;
using UnityEngine;

public class TimeData : MonoBehaviour
{
    public static TimeData Instance { get; private set; }

    [SerializeField]
    TextMeshProUGUI[] timerTextStage;
    //float[] timeRecordStage = { 0.0f, 0.0f, 0.0f };

    void Start()
    {
        SaveManager saveManager = new SaveManager();
        if (saveManager != null)
        {
            ShowTimeRecord(saveManager);
        }
        else { Debug.LogError("Time Data Error"); }
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        ShowTimeRecord(new SaveManager());
    }

    void ShowTimeRecord(SaveManager saveManager)
    {
        for (int i = 0; i < 9; i++)
        {
            if (saveManager.LoadRecordTime(i) != 0.0f)
            {
                float timeRecordStage = saveManager.LoadRecordTime(i);
                int minutes = Mathf.FloorToInt(timeRecordStage / 60F); // •ª‚ðŒvŽZ
                int seconds = Mathf.FloorToInt(timeRecordStage % 60F); // •b‚ðŒvŽZ
                int milliseconds = Mathf.FloorToInt((timeRecordStage * 1000F) % 1000F); // ƒ~ƒŠ•b‚ðŒvŽZ
                string timeText = string.Format("{0:0}'{1:00},{2:000}", minutes, seconds, milliseconds); // ƒtƒH[ƒ}ƒbƒg‚³‚ê‚½•¶Žš—ñ‚ð•Ô‚·

                timerTextStage[i].text = timeText;
            }
            else
            {
                timerTextStage[i].text = "-'--,---";
            }
        }
    }

}
