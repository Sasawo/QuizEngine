using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ServerSingleton : MonoBehaviour
{
	private WebSocketServer wssv;
	private int port;
	private Process process;
	[SerializeField] TMP_Text ipShow;

	void Start()
	{
		port = GetFreePort();
		wssv = new WebSocketServer(IPAddress.Any, port);
		wssv.AddWebSocketService<UserActions>("/actions");
		wssv.Start();
		UnityEngine.Debug.Log($"Server started on ws://<IP>:{port}/actions");

		process = new Process();
		process.StartInfo.FileName = "python";
		process.StartInfo.Arguments = "-m http.server 8000";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.WorkingDirectory = @"C:\Users\sawas\OneDrive\Desktop\Folders\Resources\Repositories\QuizEngine\QuizEngine\Assets\Web";
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;

		process.Start();

		ipShow.text = $"http://{GetActiveIPv4()}:{port}";

		DontDestroyOnLoad(gameObject);
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
	protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
	{
		UnityEngine.Debug.Log("Received: " + e.Data);
	}
}
