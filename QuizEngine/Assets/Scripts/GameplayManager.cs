using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    public List<List<QuestionBallot>> QuestionList { get; set; } = new();
    public QuestionBallot CurrentQuestion = null;
	[NonSerialized] public bool UpdatePoints = false;
	[NonSerialized] public bool ShowContent = false;
	[NonSerialized] public bool StopContent = false;
	[NonSerialized] public bool ShowAnswer = false;
	[NonSerialized] public bool Continue = false;

	private void Awake()
	{
		Instance = this;
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("AltCamera")) o.GetComponent<Camera>().enabled = false;
	}

    void Update()
    {
        if (CurrentQuestion && CurrentQuestion.Valid)
		{
			CurrentQuestion.Question.Load();
			CurrentQuestion.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			CurrentQuestion.gameObject.transform.Find("Text").GetComponent<TMP_Text>().text = "";
			CurrentQuestion.Valid = false;
		}

		if (ShowContent)
		{
			CurrentQuestion.Question.Show();
			ShowContent = false;
		}

		if (StopContent)
		{
			CurrentQuestion.Question.Stop();
			StopContent = false;
		}

		if (ShowAnswer)
		{
			CurrentQuestion.Question.ShowAnswer();
			ShowAnswer = false;
		}

		if (!Continue) return;

		if (CurrentQuestion is not null)
		{
			CurrentQuestion.Question.Continue();
			CurrentQuestion = null;
			Continue = false;
		}
		else
		{
			Continue = false;
		}
	}
}
