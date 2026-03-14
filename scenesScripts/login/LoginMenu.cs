namespace FSDClient.sceneScripts.login;

using Godot;
using System;
using System.Text.Json;
using FSDClient.autoLoad;

class LoginResponse
{
	public string Token { get; set; }
	public long UserID { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }
	public DateTime ExpiresAt { get; set; }
}

public partial class LoginMenu : Control
{
	private NetworkManager networkManager;
	private string Username { get; set; } = "";
	private string Password { get; set; } = "";
	private string Email { get; set; } = "";
	private bool CreatingAccount { get; set; } = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		networkManager = NetworkManager.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	
	// public void _on_

	public void _OnInformationReady()
	{
		var UsernameEdit = (TextEdit)FindChild("Username", true);
		Username = UsernameEdit.Text;

		var PasswordEdit = (TextEdit)FindChild("Password", true);
		Password = PasswordEdit.Text;

		// NOTE DO NOT UNCOMMENT BELOW. I HAVENT SETTLED IT YET THIS will cause null pointer exception
		// var EmailEdit = (TextEdit)FindChild("Email", true);
		// Email = EmailEdit.Text;


		if (CreatingAccount)
		{
			Register();
		}
		else
		{
			Login();
		}
		// Perform said http request
	}

	private void Login()
	{
		GD.Print("Logging in");
		var jsonObj = new
		{
			Username = Username,
			Password = Password
		};
		var jsonString = JsonSerializer.Serialize(jsonObj);

		// Temporarily hardcoded DO NOT PUSH TS until I find out how to properly send the data
		networkManager.SendRequest(String.Format(NetworkManager.BASE_URL + "/auth/login"), Godot.HttpClient.Method.Post, jsonString, LoginResponse);
	}

	private void Register()
	{
		var jsonObj = new
		{
			Username = Username,
			Password = Password,
			Email = Email
		};
		var jsonString = JsonSerializer.Serialize(jsonObj);

		// Temporarily hardcoded DO NOT PUSH TS until I find out how to properly send the data
		networkManager.SendRequest(NetworkManager.BASE_URL + "/auth/register", Godot.HttpClient.Method.Post, jsonString, RegisterResponse);
	}

	private void RegisterResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		if (result != 201 || responseCode != 201)
        {
			// This one would be some sort of server fail or some sort of conluding error 
			return;
		}

		// If successful, swithc back to login mode
		CreatingAccount = false; 
	}

	private void LoginResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		if (result != (long)HttpRequest.Result.Success && responseCode != 200)
        {
			// TODO: Put some label that prevents them for conitnueing and ask them to try again
			return;
		}

		string json = System.Text.Encoding.UTF8.GetString(body);
		// Do other things with the data Ig
		var data = JsonSerializer.Deserialize<LoginResponse>(json);
		PlayerStateManager.Instance.Token = data.Token;

	}
}
