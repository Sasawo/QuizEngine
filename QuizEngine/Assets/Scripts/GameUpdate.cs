using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GameUpdate : MonoBehaviour
{
    [SerializeField] RectTransform readyShow;
	[SerializeField] RectTransform playerShow;
	[SerializeField] GameObject namePrefab;
	private List<GameObject> players = new();
    int prevCount = 0;

	private void Start()
	{
		foreach (Player p in GameManager.Instance.players)
        {
			GameObject player = p.GetIcon(playerShow);
			player.transform.Find("Points").GetComponent<TMP_Text>().text = $"{p.Points}";
			player.GetComponent<RectTransform>().localScale *= 0.5f;
			players.Add(player);
		}
	}
	void Update()
    {
        if (GameManager.Instance.readyPlayers.Count == prevCount) return;

        for (int i = readyShow.childCount - 1; i >= 0; --i)
				Destroy(readyShow.GetChild(i).gameObject);

		for (int i = 0; i < GameManager.Instance.readyPlayers.Count; ++i)
        {
            GameObject text = Instantiate(namePrefab, readyShow);
            text.GetComponent<TMP_Text>().text = GameManager.Instance.readyPlayers[i];
		}

        prevCount = GameManager.Instance.readyPlayers.Count;


	}
}
