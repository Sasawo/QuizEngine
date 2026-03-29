using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    public List<List<QuestionBallot>> QuestionList { get; set; } = new();
    public QuestionBallot CurrentQuestion = null;

    public int RowIndex = -1;
    public int ColIndex = -1;

	private void Awake()
	{
		Instance = this;
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("AltCamera")) o.GetComponent<Camera>().enabled = false;
	}

    void Update()
    {
        if (RowIndex >= 0 && ColIndex >= 0 && QuestionList[RowIndex][ColIndex].Valid)
        {
            CurrentQuestion = QuestionList[RowIndex][ColIndex];
			CurrentQuestion.Load();
            RowIndex = -1;
            ColIndex = -1;
		}
    }
    public QuestionBallot GetCurrentQuestion() => QuestionList[RowIndex][ColIndex];
}
