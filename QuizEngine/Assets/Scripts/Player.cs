using TMPro;
using UnityEngine;

public class Player
{
	public string Name { get; private set; }
	public int Points { get; set; } = 0;
	private GameObject playerIcon;

	public Player(string name, GameObject icon)
	{
		Name = name;
		playerIcon = icon;
	}

	public GameObject GetIcon(RectTransform parent)
	{
		GameObject player = Object.Instantiate(playerIcon, parent);
		player.transform.Find("Name").GetComponent<TMP_Text>().text = Name;

		return player;
	}
}