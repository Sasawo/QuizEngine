using TMPro;
using UnityEngine;

public class SetupUI : MonoBehaviour
{
	public static SetupUI Instance;
	[SerializeField] RectTransform Column;
	[SerializeField] GameObject RowPrefab;
	[SerializeField] GameObject TitleBallotPrefab;
	[SerializeField] GameObject QuestionBallotPrefab;
	void Awake()
	{
		Instance = this;
	}
	void Start()
    {
		Setup();
    }
    public void Setup()
    {
		foreach (Transform child in Column.transform)
		{
			Destroy(child.gameObject);
		}
		GameplayManager.Instance.QuestionList.Clear();

		GameData data = GameManager.Instance.GameData;

		for (int i = 0; i < data.Rounds[GameManager.Instance.RoundNumber].Categories.Count; ++i)
		{
			GameObject row = Instantiate(RowPrefab, Column);
			GameplayManager.Instance.QuestionList.Add(new());

			GameObject title = Instantiate(TitleBallotPrefab, row.GetComponent<RectTransform>());
			title.transform.Find("Text").GetComponent<TMP_Text>().text = data.Rounds[GameManager.Instance.RoundNumber].Categories[i].Name;
			title.GetComponent<QuestionBallot>().Question = GameManager.Instance.GameData.Rounds[GameManager.Instance.RoundNumber].Categories[i].Bonus;

			for (int j = 0; j < data.Rounds[GameManager.Instance.RoundNumber].Categories[i].Questions.Count; ++j)
			{
				GameObject question = Instantiate(QuestionBallotPrefab, row.GetComponent<RectTransform>());
				question.transform.Find("Text").GetComponent<TMP_Text>().text = GameManager.Instance.GameData.Rounds[GameManager.Instance.RoundNumber].Categories[i].Questions[j].Points.ToString();
				question.GetComponent<QuestionBallot>().Question = GameManager.Instance.GameData.Rounds[GameManager.Instance.RoundNumber].Categories[i].Questions[j];

				GameplayManager.Instance.QuestionList[i].Add(question.GetComponent<QuestionBallot>());
			}

			GameplayManager.Instance.QuestionList[i].Add(title.GetComponent<QuestionBallot>());
		}
	}
}
