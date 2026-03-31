using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WinSetup : MonoBehaviour
{
    [SerializeField] List<GameObject> medals;
    void Start()
    {
        List<Player> podium = GameManager.Instance.players.OrderByDescending(x => x.Points).ToList();

        if (podium.Count == 1)
        {
            medals[1].SetActive(false);
			medals[2].SetActive(false);
			medals[3].SetActive(false);
			medals[4].SetActive(false);
		} else if (podium.Count == 2)
        {
			medals[0].SetActive(false);
			medals[1].SetActive(false);
			medals[2].SetActive(false);
		} else
        {
			medals[3].SetActive(false);
			medals[4].SetActive(false);
			Player temp = podium[0];
			podium[0] = podium[1];
			podium[1] = temp;
		}

		for (int i = 0; i < podium.Count && i < 3; ++i)
			podium[i].GetIcon(gameObject.GetComponent<RectTransform>());
    }
}
