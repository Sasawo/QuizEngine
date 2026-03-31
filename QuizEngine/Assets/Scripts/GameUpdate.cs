using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameUpdate : MonoBehaviour
{
    [SerializeField] List<RectTransform> readyShow;
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
		if (GameplayManager.Instance.UpdatePoints)
		{
			foreach (GameObject p in players)
				p.transform.Find("Points").GetComponent<TMP_Text>().text = $"{GameManager.Instance.players.Where(x => x.Name == p.transform.Find("Name").GetComponent<TMP_Text>().text).First().Points}";

			GameplayManager.Instance.UpdatePoints = false;
		}

        if (GameManager.Instance.readyPlayers.Count == prevCount) return;

		for (int i = 0; i < readyShow.Count; ++i)
			for (int j = readyShow[i].childCount - 1; j >= 0; --j)
				Destroy(readyShow[i].GetChild(j).gameObject);

		for (int i = 0; i < readyShow.Count; ++i)
			for (int j = 0; j < GameManager.Instance.readyPlayers.Count; ++j)
			{
				GameObject text = Instantiate(namePrefab, readyShow[i]);
				text.GetComponent<TMP_Text>().text = GameManager.Instance.readyPlayers[j];
			}

        prevCount = GameManager.Instance.readyPlayers.Count;


	}
}
