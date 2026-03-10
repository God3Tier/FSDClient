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
    public static readonly string BASE_URL = "http://localhost:8000/";
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
                GD.Print("Request has been sent");
                httpRequest.QueueFree();
            };

            httpRequest.Request(
                request.Url,
                request.Headers,
                request.Method,
                request.Json
            );
            GD.Print("Request has been sent");
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
