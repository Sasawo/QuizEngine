using UnityEngine;

public class SetupUpdate : MonoBehaviour
{
	[SerializeField] GameObject playerShow;
    void Update()
    {
		int children = playerShow.transform.childCount;

		if (children < GameManager.Instance.players.Count)
		{
			GameManager.Instance.players[children].GetIcon(playerShow.GetComponent<RectTransform>());
			Debug.Log($"showing {GameManager.Instance.players[children]}");
		}
	}
}
