using TMPro;
using UnityEngine;

public class SetupUI : MonoBehaviour
{
	[SerializeField] RectTransform Column;
	[SerializeField] GameObject RowPrefab;
	[SerializeField] GameObject TitleBallotPrefab;
	[SerializeField] GameObject QuestionBallotPrefab;
    void Start()
    {
        GameData data = GameManager.Instance.GameData;

        for (int i = 0; i < data.Rounds[GameManager.Instance.RoundNumber].Categories.Count; ++i)
        {
            GameObject row = Instantiate(RowPrefab, Column);

            GameObject title = Instantiate(TitleBallotPrefab, row.GetComponent<RectTransform>());
            title.transform.Find("Text").GetComponent<TMP_Text>().text = data.Rounds[GameManager.Instance.RoundNumber].Categories[i].Name;

			for (int j = 0; j < data.Rounds[GameManager.Instance.RoundNumber].Categories[i].Questions.Count; ++j)
            {
				GameObject question = Instantiate(QuestionBallotPrefab, row.GetComponent<RectTransform>());
				question.transform.Find("Text").GetComponent<TMP_Text>().text = ((j + 1) * 1000).ToString();
			}
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
