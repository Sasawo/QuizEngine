using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ServerPersistor : MonoBehaviour
{
	public static ServerPersistor Instance;
	private WebSocketServer wssv;
	private Process process;
	[SerializeField] TMP_Text ipShow = null;

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
	void Start()
	{
		int port = GetFreePort();
		wssv = new WebSocketServer(IPAddress.Any, port);
		wssv.AddWebSocketService<UserActions>("/actions");
		wssv.Start();
		UnityEngine.Debug.Log($"Server started on ws://<IP>:{port}/actions");

		string path = @"C:\Users\sawas\OneDrive\Desktop\Folders\Resources\Repositories\QuizEngine\QuizEngine\Assets\Web";

		InitTemplate("index", path, port);
		InitTemplate("user", path, port);
		InitTemplate("admin", path, port);
		InitGameContent("adminGame", path, port);

		process = new Process();
		process.StartInfo.FileName = "python";
		process.StartInfo.Arguments = "-m http.server 8000";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.WorkingDirectory = path;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;

		process.Start();

		if (ipShow != null) ipShow.text = $"http://{GetActiveIPv4()}:8000";

		DontDestroyOnLoad(gameObject);
	}
	private void InitTemplate(string name, string path, int port)
	{
		string content = System.IO.File.ReadAllText($@"{path}\templates\template-{name}.html");
		content = content.Replace("<IP_REPLACE>", $"{GetActiveIPv4()}:{port}");

		System.IO.File.WriteAllText($@"{path}\{name}.html", content);
	}
	private void InitGameContent(string name, string path, int port)
	{
		string replacing = "";
		for (int i = 0; i < GameManager.Instance.GameData.Rounds[0].Categories.Count; ++i)
		{
			replacing += $"<div id =\"{i}\" style =\"display:flex;flex-direction:row;gap:2vw;\">\n";

			replacing += $"<input id=\"category\" type =\"button\" value =\"{GameManager.Instance.GameData.Rounds[0].Categories[i].Name}\" class=\"button altButton\">\n";
			for (int j = 0; j < GameManager.Instance.GameData.Rounds[0].Categories[i].Questions.Count; ++j)
				replacing += $"<input id=\"{j}\" type =\"button\" value =\"{(j + 1) * 1000}\" class=\"button question altButton\">\n";

			replacing += $"<input id=\"{GameManager.Instance.GameData.Rounds[0].Categories[i].Questions.Count}\" type =\"button\" value =\"Bonus\" class=\"button question altButton\">\n";
			replacing += "</div>\n";
		}

		string content = System.IO.File.ReadAllText($@"{path}\templates\template-{name}.html");
		content = content.Replace("<IP_REPLACE>", $"{GetActiveIPv4()}:{port}");
		content = content.Replace("<CONTENT_REPLACE>", replacing);

		System.IO.File.WriteAllText($@"{path}\{name}.html", content);
	}
	void OnApplicationQuit()
	{
		wssv.Stop();
		process.Kill();
		print("stopped");
	}
	private int GetFreePort()
	{
		TcpListener l = new TcpListener(IPAddress.Loopback, 0);
		l.Start();
		int port = ((IPEndPoint)l.LocalEndpoint).Port;
		l.Stop();
		return port;
	}

	private static string GetActiveIPv4()
	{
		var network = NetworkInterface.GetAllNetworkInterfaces().ToList()
			.Where(n =>
				n.OperationalStatus == OperationalStatus.Up &&
				n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
				n.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily == AddressFamily.InterNetwork)
			)
			.FirstOrDefault();

		if (network == null)
			return null;

		var ip = network.GetIPProperties().UnicastAddresses
			.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

		return ip?.Address.ToString();
	}
}

public class UserActions : WebSocketBehavior
{
	protected override void OnMessage(MessageEventArgs e)
	{
		UnityEngine.Debug.Log(e.Data);
		string name = (string)JObject.Parse(e.Data)["user"];

		switch ((string)JObject.Parse(e.Data)["action"])
		{
			case "Init":
				UserInit(name);
				break;
			case "Ready":
				UserReady(name);
				break;
			case "Start":
				AdminStartGame();
				break;
			case "Open":
				AdminOpenQuestion(e);
				break;
			case "GetPlayers":
				AdminGetPlayers();
				break;
			case "Increment":
				AdminIncrement(e);
				break;
			case "Clear":
				AdminClear();
				break;
			case "Show":
				AdminShowQuestion();
				break;
			case "Stop":
				AdminStopQuestion();
				break;
			case "Answer":
				AdminShowAnswer();
				break;
			case "Continue":
				AdminContinue();
				break;
			default:
				break;
		}
	}
	private void UserInit(string name)
	{
		if (GameManager.Instance.currentScene != "UserSetup")
		{
			Send("Registration is closed");
			return;
		}

		if (!GameManager.Instance.players.Where(x => x.Name == name).Any()) Send("Valid");
		else
		{
			Send("Selected name is invalid");
			return;
		}

		Player player = new Player(name, GameManager.Instance.playerPrefabs[GameManager.Instance.players.Count % GameManager.Instance.playerPrefabs.Count]);
		GameManager.Instance.players.Add(player);
	}
	private void UserReady(string name)
	{
		if (!GameManager.Instance.readyPlayers.Where(x => x == name).Any()) GameManager.Instance.readyPlayers.Add(name);
	}
	private void AdminClear()
	{
		GameManager.Instance.readyPlayers.Clear();
	}
	private void AdminStartGame()
	{
		GameManager.Instance.LoadScene("GameScene");
	}
	private void AdminGetPlayers()
	{
		foreach (Player p in GameManager.Instance.players)
		{
			var o = new
			{
				user = p.Name,
				action = "PlayerAdd",
				content = GameManager.Instance.GameData.CommonDenominator,
				answer = "lol, why you reading this",
				type = "also useless"
			};

			Send(JsonConvert.SerializeObject(o));
		}
	}
	private void AdminOpenQuestion(MessageEventArgs e)
	{
		var parsed = JObject.Parse(e.Data);
		GameplayManager.Instance.CurrentQuestion = GameplayManager.Instance.QuestionList[(int)parsed["category"]][(int)parsed["question"]];
		
		var o = new { 
			user = (string)JObject.Parse(e.Data)["user"],
			action = "Question",
			content = GameplayManager.Instance.CurrentQuestion.Question.Content,
			answer = GameplayManager.Instance.CurrentQuestion.Question.Answer,
			type = GameplayManager.Instance.CurrentQuestion.Question.Type
		};

		Send(JsonConvert.SerializeObject(o));
	}
	private void AdminIncrement(MessageEventArgs e)
	{
		var parsed = JObject.Parse(e.Data);

		GameManager.Instance.players.Where(x => x.Name == (string)parsed["category"]).First().Points += (int)parsed["value"];
		GameplayManager.Instance.UpdatePoints = true;
	}
	private void AdminShowQuestion()
	{
		GameplayManager.Instance.ShowContent = true;
	}
	private void AdminStopQuestion()
	{
		GameplayManager.Instance.StopContent = true;
	}
	private void AdminShowAnswer()
	{
		GameplayManager.Instance.ShowAnswer = true;
	}
	private void AdminContinue()
	{
		GameplayManager.Instance.Continue = true;
	}
}
