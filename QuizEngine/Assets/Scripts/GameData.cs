using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
public class Question
{
    public string Text { get; set; } = "No_QUESTION";
	public string Answer { get; set; } = "No_ANSWER";
}
public class Category
{
	public string Name { get; set; } = "No_NAME";
    public List<Question> Questions { get; set; } = new();
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
				Category cat = new();
				cat.Name = (string)category["name"];

				foreach (var question in category["questions"])
				{
					Question quest = new();
					quest.Text = (string)question["text"];
					quest.Answer = (string)question["answer"];

					cat.Questions.Add(quest);
				}

				rnd.Categories.Add(cat);
			}

			Rounds.Add(rnd);
		}
	}
    
}
