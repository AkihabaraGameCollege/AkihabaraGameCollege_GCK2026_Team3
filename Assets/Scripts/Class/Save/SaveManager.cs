using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveManager
{
    //public void SaveRecordTime(int stage, float recordTime)
    //{
    //    float saveTime = PlayerPrefs.GetFloat(string.Format("{0}", stage), 0.0f);
    //    if (saveTime > recordTime || saveTime == 0.0f)
    //    {
    //        PlayerPrefs.SetFloat(string.Format("{0}", stage), recordTime);
    //        PlayerPrefs.Save();
    //    }
    //}
    public void SaveRecordTime(int stage, float recordTime)
    {
        float[] time = new float[3];

        for (int i = 0; i < 3; i++)
        {
            time[i] = PlayerPrefs.GetFloat(string.Format("{0},{1}", stage, i), 0.0f); ;
        }

        for (int rank = 0; rank < 3 ; rank++)
        {
            if (time[rank] > recordTime || time[rank] == 0.0f)
            {
                PlayerPrefs.SetFloat(string.Format("{0},{1}", stage, rank), recordTime); 
                for (int newRank = rank + 1; newRank < 3 ; newRank++)
                {
                    PlayerPrefs.SetFloat(string.Format("{0},{1}", stage, newRank), time[newRank - 1]);
                }
                PlayerPrefs.Save();
                break;
            }
        }
    }

    //public float LoadRecordTime(int stage)
    //{
    //    return PlayerPrefs.GetFloat(string.Format("{0}", stage), 0.0f);
    //}
    public float LoadRecordTime(int recordText)
    {
        int stage = recordText/3;
        int rank = recordText%3;
        return PlayerPrefs.GetFloat(string.Format("{0},{1}", stage, rank), 0.0f);
    }

}
