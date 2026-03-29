using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	public GameData GameData { get; set; } = new("C:/Users/sawas/OneDrive/Desktop/Folders/Resources/Repositories/QuizEngine/QuickQuestions.json");
	[SerializeField] public List<GameObject> playerPrefabs;
	[NonSerialized] public List<Player> players = new();
	[NonSerialized] public List<string> readyPlayers = new();
	[NonSerialized] private string newScene = null;
	[NonSerialized] public string currentScene = null;
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
		if (currentScene != SceneManager.GetActiveScene().name) currentScene = SceneManager.GetActiveScene().name;

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
	public void LoadAudioClip(string path, AudioQuestion self)
	{
		StartCoroutine(LoadAudioClipCoroutine(path, self));
	}
	private IEnumerator LoadAudioClipCoroutine(string path, AudioQuestion self)
	{
		if (!File.Exists(path)) yield break;

		string url = "file:///" + path;

		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
		{
			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success) yield break;
			else
			{
				AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
				self.AudioClip = clip;
			}
		}
	}
	public void LoadImage(string path, ImageQuestion self)
	{
		if (!File.Exists(path)) return;

		self.Texture = new Texture2D(2, 2);
		self.Texture.LoadImage(File.ReadAllBytes(path));
	}
	public void ShowImage(GameObject mask, ImageQuestion self)
	{
		self.Coroutine = StartCoroutine(ShowImageCoroutine(mask.GetComponent<RectMask2D>(), self));
	}
	public void StopImage(Coroutine co)
	{
		if (co != null)
			StopCoroutine(co);
	}
	private IEnumerator ShowImageCoroutine(RectMask2D mask, ImageQuestion self)
	{
		float padding = mask.padding.z / 30;

		while (mask.padding.z > 0)
		{
			yield return new WaitWhile(() => self.CoroutinePaused);

			mask.padding = new(0, 0, mask.padding.z - padding, 0);

			yield return new WaitForSeconds(0.3f);
		}
	}
}



