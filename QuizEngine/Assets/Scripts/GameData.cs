using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
public abstract class Question
{
	public enum QType { TEXT, IMAGE, AUDIO };
    public string Content { get; set; } = "No_QUESTION";
	public string Answer { get; set; } = "No_ANSWER";
	public QType Type { get; set; }
	public Question(string content, string answer)
	{
		Content = content;
		Answer = answer;
	}

	public static Question GetQuestion(string content, string answer, string type)
	{
		switch (type)
		{
			case "Text":
				return new TextQuestion(content, answer);
			case "Image":
				return new ImageQuestion(content, answer);
			case "Audio":
				return new AudioQuestion(content, answer);
			default:
				return new TextQuestion("bad", "bad");
		}
	}

	public abstract void Load();
}

public class TextQuestion : Question
{
	public TextQuestion(string content, string answer) : base(content, answer)
	{
		Type = QType.TEXT;
	}

	public override void Load()
	{
		GameObject.Find("MainCamera").tag = "AltCamera";
		GameObject.Find("TextCamera").tag = "MainCamera";

		GameObject.Find("TextCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;
	}
}

public class ImageQuestion : Question
{
	public ImageQuestion(string content, string answer) : base(content, answer)
	{
		Type = QType.IMAGE;
	}

	public override void Load()
	{
		GameObject.Find("MainCamera").tag = "AltCamera";
		GameObject.Find("ImageCamera").tag = "MainCamera";

		GameObject.Find("ImageCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;
	}
}

public class AudioQuestion : Question
{
	public AudioQuestion(string content, string answer) : base(content, answer)
	{
		Type = QType.AUDIO;
	}

	public override void Load()
	{
		GameObject.Find("MainCamera").tag = "AltCamera";
		GameObject.Find("AudioCamera").tag = "MainCamera";

		GameObject.Find("AudioCamera").GetComponent<Camera>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;
	}
}

public class Category
{
	public string Name { get; set; } = "No_NAME";
    public List<Question> Questions { get; set; } = new();
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

    public GameData(string path)
    {
		JObject root = JObject.Parse(File.ReadAllText(path));

		foreach (var round in root["rounds"])
		{
			Round rnd = new();

			foreach (var category in round["categories"])
			{
				Category cat = new((string)category["name"]);

				foreach (var question in category["questions"])
				{
					Question quest = Question.GetQuestion((string)question["content"], (string)question["answer"], (string)question["type"]);

					cat.Questions.Add(quest);
				}

				rnd.Categories.Add(cat);
			}

			Rounds.Add(rnd);
		}
	}
    
}
