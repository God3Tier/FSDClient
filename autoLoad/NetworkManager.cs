namespace FSDClient.autoLoad;

using Godot;
using System.Collections.Generic;
using static Godot.HttpClient;
using HttpRequestCompletedHandler = Godot.HttpRequest.RequestCompletedEventHandler;

// I will leave this as static empty methods or will load fake data into here
// since the server is not up yet
// Also I do not want to learn network code yet so....
public partial class NetworkManager : Node
{
	// These are the constants used for all network stuff
	public static readonly string BASE_URL = "http://localhost";
	public static readonly string AUTH = ":8000";
	public static readonly string MATCHMAKING = ":8001";
	public static readonly string GAMEPLAY = ":8002";
	public static readonly string FRIENDLIST = ":8003";
	public static readonly string REPLAY = ":8004";    
	public static readonly string DECK = ":8005";    
	public static NetworkManager Instance { get; set; }
	private readonly Queue<HttpRequestData> httpRequestQueue = new();

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _Process(double delta)
	{
		while (httpRequestQueue.Count > 0)
		{
			var request = httpRequestQueue.Dequeue();
			HttpRequest httpRequest = new();
			AddChild(httpRequest);
			httpRequest.RequestCompleted += request.HttpRequestCompletedHandler;
			httpRequest.RequestCompleted += (
				long result, long responseCode, string[] headers, byte[] body
			) =>
			{
				// Process the actual data properly. However, this one is essentially overwritten
				// later
				if (result == (long)HttpRequest.Result.Timeout)
				{
					GD.PrintErr($"Request timed out: {request.Url}");
					// handle timeout — retry, notify user, etc.
				}
				else if (result != (long)HttpRequest.Result.Success)
				{
					GD.PrintErr($"Request failed with result: {(HttpRequest.Result)result}, URL: {request.Url}");
				}
				httpRequest.QueueFree();
			};
			GD.Print("Attempting to Request method");
			httpRequest.Request(
				request.Url,
				request.Headers,
				request.Method,
				request.Json
			);
			GD.Print("Successfully to Received method");

		}
	}

	public void SendRequest(
		string url,
		Method method,
		string json,
		HttpRequestCompletedHandler httpRequestCompletedHandler,
		string[] headers = null
	)
	{
		if (headers == null)
		{
			headers = new string[1] { "Content-Type: application/json" };
		} else {
			headers[0] = "Content-Type: application/json";
		}
		
		httpRequestQueue.Enqueue(new HttpRequestData(
			url,
			headers,
			method,
			json,
			httpRequestCompletedHandler
		));
	}

		public void SendRequestWithToken(
		string url,
		Method method,
		string json,
		HttpRequestCompletedHandler httpRequestCompletedHandler,
		string[] headers = null
	)
	{
		if (headers == null)
		{
			headers = new string[2] { "Content-Type: application/json", "Authorization: Bearer " + PlayerStateManager.Instance.Token };
		} else {
			headers[0] = "Content-Type: application/json";
			headers[1] = "Authorization: Bearer " + PlayerStateManager.Instance.Token;
		}
		
		httpRequestQueue.Enqueue(new HttpRequestData(
			url,
			headers,
			method,
			json,
			httpRequestCompletedHandler
		));
	}

	// private void RequestFriendList()
	// {

	// }

	private struct HttpRequestData
	{
		public string Url;
		public string[] Headers;
		public Method Method;
		public string Json;
		public HttpRequestCompletedHandler HttpRequestCompletedHandler;

		public HttpRequestData(
			string url,
			string[] headers,
			Method method,
			string json,
			HttpRequestCompletedHandler httpRequestCompletedHandler
		)
		{
			Url = url;
			Headers = headers;
			Method = method;
			Json = json;
			HttpRequestCompletedHandler = httpRequestCompletedHandler;
		}
	}
}
