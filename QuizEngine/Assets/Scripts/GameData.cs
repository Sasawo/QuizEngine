using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public abstract class Question
{
	public enum QType { TEXT, IMAGE, AUDIO };
    public string Content { get; set; } = "No_QUESTION";
	public string Answer { get; set; } = "No_ANSWER";
	public int Points { get; set; } = 0;
	public QType Type { get; set; }
	public Question(string content, string answer, int pts)
	{
		Content = content;
		Answer = answer;
		Points = pts;
	}

	public static Question GetQuestion(string content, string answer, string type, int pts)
	{
		switch (type)
		{
			case "Text":
				return new TextQuestion(content, answer, pts);
			case "Image":
				return new ImageQuestion(content, answer, pts);
			case "Audio":
				return new AudioQuestion(content, answer, pts);
			default:
				return new TextQuestion("bad", "bad", 0);
		}
	}

	public abstract void Load();
	public abstract void Show();
	public abstract void Stop();
	public abstract void ShowAnswer();
	public abstract void Continue();
}

public class TextQuestion : Question
{
	public TextQuestion(string content, string answer, int pts) : base(content, answer, pts)
	{
		Type = QType.TEXT;
	}

	public override void Load()
	{
		GameObject.Find("MainCamera").tag = "AltCamera";
		GameObject.Find("TextCamera").tag = "MainCamera";

		GameObject.Find("TextCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;

		GameObject canvas = GameObject.Find("TextQuestion");
		canvas.transform.Find("QuestionText").GetComponent<TMP_Text>().text = "";
		canvas.transform.Find("AnswerText").GetComponent<TMP_Text>().text = "";
	}
	public override void Show()
	{
		GameObject.Find("TextQuestion").transform.Find("QuestionText").GetComponent<TMP_Text>().text = Content;
	}
	public override void Stop() { }
	public override void ShowAnswer()
	{
		GameObject.Find("TextQuestion").transform.Find("AnswerText").GetComponent<TMP_Text>().text = Answer;
	}
	public override void Continue()
	{
		GameObject.Find("MainCamera").tag = "MainCamera";
		GameObject.Find("TextCamera").tag = "AltCamera";

		GameObject.Find("TextCamera").GetComponent<Camera>().enabled = false;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = true;
	}
}

public class ImageQuestion : Question
{
	public Texture2D Texture;
	public Coroutine Coroutine;
	public bool CoroutinePaused = false;
	public ImageQuestion(string content, string answer, int pts) : base(content, answer, pts)
	{
		Type = QType.IMAGE;
	}

	public override void Load()
	{
		GameObject.Find("MainCamera").tag = "AltCamera";
		GameObject.Find("ImageCamera").tag = "MainCamera";

		GameObject.Find("ImageCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;

		GameObject canvas = GameObject.Find("ImageQuestion");
		canvas.transform.Find("AnswerText").GetComponent<TMP_Text>().text = "";
		canvas.transform.Find("Mask").GetComponent<RectMask2D>().padding = new(0, 0, 1262.52f, 0);

		GameManager.Instance.LoadImage(Content, this);
	}
	public override void Show()
	{
		if (CoroutinePaused)
		{
			CoroutinePaused = false;
			return;
		}

		GameObject image = GameObject.Find("ImageQuestion").transform.Find("Mask").gameObject;
		image.transform.Find("QuestionImage").GetComponent<RawImage>().texture = Texture;
		image.transform.Find("QuestionImage").GetComponent <AspectRatioFitter>().aspectRatio = (float)Texture.width / Texture.height;
		GameManager.Instance.ShowImage(image, this);
	}
	public override void Stop()
	{
		CoroutinePaused = true;
	}
	public override void ShowAnswer()
	{
		GameObject.Find("ImageQuestion").transform.Find("AnswerText").GetComponent<TMP_Text>().text = Answer;
	}
	public override void Continue()
	{
		GameObject.Find("MainCamera").tag = "MainCamera";
		GameObject.Find("ImageCamera").tag = "AltCamera";

		GameObject.Find("ImageCamera").GetComponent<Camera>().enabled = false;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = true;
	}
}

public class AudioQuestion : Question
{
	public AudioClip AudioClip;
	private bool isPaused = false;
	public AudioQuestion(string content, string answer, int pts) : base(content, answer, pts)
	{
		Type = QType.AUDIO;
	}

	public override void Load()
	{
		GameObject.Find("MainCamera").tag = "AltCamera";
		GameObject.Find("AudioCamera").tag = "MainCamera";

		GameObject.Find("AudioCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;

		GameObject canvas = GameObject.Find("AudioQuestion");
		canvas.transform.Find("AnswerText").GetComponent<TMP_Text>().text = "";

		GameManager.Instance.LoadAudioClip(Content, this);
	}
	public override void Show()
	{
		AudioSource audioSource = GameObject.Find("AudioQuestion").GetComponent<AudioSource>();
		if (isPaused)
		{
			audioSource.UnPause();
			isPaused = false;
		}
		else
		{
			audioSource.Stop();
			audioSource.clip = AudioClip;
			audioSource.Play();
		}
	}
	public override void Stop()
	{
		isPaused = true;
		GameObject.Find("AudioQuestion").GetComponent<AudioSource>().Pause();
	}
	public override void ShowAnswer()
	{
		GameObject.Find("AudioQuestion").transform.Find("AnswerText").GetComponent<TMP_Text>().text = Answer;
	}
	public override void Continue()
	{
		GameObject.Find("MainCamera").tag = "MainCamera";
		GameObject.Find("AudioCamera").tag = "AltCamera";

		GameObject.Find("AudioCamera").GetComponent<Camera>().enabled = false;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = true;

		GameObject.Find("AudioQuestion").GetComponent<AudioSource>().Stop();
	}
}

public class Category
{
	public string Name { get; set; } = "No_NAME";
    public List<Question> Questions { get; set; } = new();
	public Question Bonus { get; set; }
	public Category(string name)
	{
		Name = name;
	}
}
public class Round
{
	public List<Category> Categories { get; set; } = new();
}
public class GameData
{
    public List<Round> Rounds { get; set; } = new();
	public int CommonDenominator = 0;

    public GameData(string path)
    {
		List<int> pts = new();

		JObject root = JObject.Parse(File.ReadAllText(path));

		foreach (var round in root["rounds"])
		{
			Round rnd = new();

			foreach (var category in round["categories"])
			{
				Category cat = new((string)category["name"]);
				cat.Bonus = Question.GetQuestion((string)category["content"], (string)category["answer"], (string)category["type"], (int)category["points"]);

				foreach (var question in category["questions"])
				{
					Question quest = Question.GetQuestion((string)question["content"], (string)question["answer"], (string)question["type"], (int)question["points"]);
					pts.Add((int)question["points"]);

					cat.Questions.Add(quest);
				}

				rnd.Categories.Add(cat);
			}

			Rounds.Add(rnd);
		}

		pts.Sort();

		int denom = pts[pts.Count - 1];

		for (int i = 1; i < pts.Count; ++i)
		{
			int result = pts[i] - pts[i - 1];

			if (result < denom && result > 0) denom = result;
		}

		CommonDenominator = denom;
	}
    
}
