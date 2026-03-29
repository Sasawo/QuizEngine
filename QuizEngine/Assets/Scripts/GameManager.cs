using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	public GameData GameData { get; set; } = new("C:/Users/sawas/OneDrive/Desktop/Folders/Resources/Repositories/QuizEngine/Questions.json");
	[SerializeField] public List<GameObject> playerPrefabs;
	[NonSerialized] public List<Player> players = new();
	[NonSerialized] public List<string> readyPlayers = new();
	private string newScene = null;
	public int RoundNumber { get; set; } = 0;
	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
	private void Start()
	{
		playerPrefabs = playerPrefabs.Shuffle();
	}
	private void Update()
	{
		if (newScene != null)
		{
			string temp = newScene;
			newScene = null;
			SceneManager.LoadScene(temp);
		}
	}
	public void LoadScene(string scene)
	{
		newScene = scene;
	}
}



