using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 手札表示を管理する簡易UIマネージャ
// プレハブには `CardView` (Button + TMP fields) を想定
public class HandUIManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject cardViewPrefab; // プレハブ
    [SerializeField] Transform container; // UIコンテナ（HorizontalLayout等）

    List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        Refresh();
    }

    void OnEnable()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.OnHandChanged += Refresh;
        }
    }

    void OnDisable()
    {
        if (player != null)
        {
            player.OnHandChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        ClearAll();
        if (player == null || cardViewPrefab == null || container == null) return;

        var hand = player.GetHand();
        for (int i = 0; i < hand.Count; i++)
        {
            var c = hand[i];
            var go = Instantiate(cardViewPrefab, container);
            var view = go.GetComponent<CardView>();
            if (view != null) view.Bind(player, c);
            spawned.Add(go);
        }
    }

    void ClearAll()
    {
        foreach (var go in spawned)
        {
            if (go != null) Destroy(go);
        }
        spawned.Clear();
    }
}
