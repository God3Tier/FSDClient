namespace FSDClient.sceneScripts.login;

using Godot;
using System;
using System.Text.Json;
using FSDClient.autoLoad;
using System.Text.Json.Serialization;

public class LoginResponse
{
	[JsonPropertyName("token")]
	public string Token { get; set; }
	[JsonPropertyName("user_id")]
	public long UserID { get; set; }
	[JsonPropertyName("username")]
	public string Username { get; set; }
	public string IconName { get; set; }
	public string BorderColour { get; set; }
	public int Gold { get; set; }
	public int Crystal { get; set; }
	// This one I will set later
	// public List<CardData> CurrentDeck { get; set; }
	public int Level { get; set; }
	// TODO: Settle how the levels will be do1ne
	// TODO: Also settle whatever to do with the bottom one
	[JsonPropertyName("expires_at")]
	public DateTime ExpiresAt { get; set; }


	// Note: This is just to allow me to mock data since I dont want to test it with full backend yet
	public LoginResponse(string token, long userid, string username, string iconName, string borderColour, int gold, int crystal, int level)
	{
		Token = token;
		UserID = userid;
		Username = username;
		IconName = iconName;
		BorderColour = borderColour;
		Gold = gold;
		Crystal = crystal;
		Level = level;
	}
}

public partial class LoginMenu : Control
{
	private NetworkManager networkManager;
	private string Username { get; set; } = "";
	private string Password { get; set; } = "";
	private string Email { get; set; } = "";

	// This may or may not cause a problem later
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
		var RightPanel = (Control)FindChild("RightPanel", true);
		var UsernameEdit = (LineEdit)RightPanel.FindChild("UsernameInput", true);
		Username = UsernameEdit.Text;

		var PasswordEdit = (LineEdit)RightPanel.FindChild("PasswordInput", true);
		Password = PasswordEdit.Text;

		// NOTE DO NOT UNCOMMENT BELOW. I HAVENT SETTLED IT YET THIS will cause null pointer exception
		// var EmailEdit = (LineEdit)FindChild("Email", true);
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

	public void _onStateChangerPressed()
	{
		if (CreatingAccount)
		{
			GD.Print("State changer button called");
			var SendInformationButton = (Button)FindChild("SendInformation");
			SendInformationButton.Text = "Login";

			var StateChangerButton = (Button)FindChild("StateChanger");
			StateChangerButton.Text = "No account? Click here to create one";
			CreatingAccount = false;
		}
		else
		{
			GD.Print("State changer button called");
			var SendInformationButton = (Button)FindChild("SendInformation");
			SendInformationButton.Text = "Create Account";

			var StateChangerButton = (Button)FindChild("StateChanger");
			StateChangerButton.Text = "Have an account already? Click here to Login";

			CreatingAccount = true;
		}
	}

	private void Login()
	{
		GD.Print("Logging in");
		var jsonObj = new
		{
			username = Username,
			password = Password
		};
		var jsonString = JsonSerializer.Serialize(jsonObj);

		// Temporarily hardcoded DO NOT PUSH TS until I find out how to properly send the data
		networkManager.SendRequest(String.Format(NetworkManager.BASE_URL + NetworkManager.AUTH + "/auth/login"), Godot.HttpClient.Method.Post, jsonString, LoginResponse);
	}

	// private LoginResponse MockLogin()
	// {
	// 	return new("token", 1, "JohnDoe", "purple", "grey", 10, 10, 1);
	// }

	private void Register()
	{
		var jsonObj = new
		{
			username = Username,
			password = Password,
		};
		var jsonString = JsonSerializer.Serialize(jsonObj);
		GD.Print(jsonObj);
		// Temporarily hardcoded DO NOT PUSH TS until I find out how to properly send the data
		GD.Print("Sending the register method");
		networkManager.SendRequest(NetworkManager.BASE_URL + NetworkManager.AUTH + "/auth/register", Godot.HttpClient.Method.Post, jsonString, RegisterResponse);
	}

	private void RegisterResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print(responseCode);
		GD.Print(System.Text.Encoding.UTF8.GetString(body));
		if (result != 201 || responseCode != 201)
		{
			// This one would be some sort of server fail or some sort of conluding error 
			return;
		}
		// If successful, swithc back to login mode
		CreatingAccount = false;
		GD.Print("Login Request sent");
	}

	private void LoginResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print(responseCode); ;
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(json);
		if (result != (long)HttpRequest.Result.Success || responseCode != 200)
		{
			// GD.Print("Attempting mock login");
			// PlayerStateManager.Instance.SetPlayerData(MockLogin());
			// // TODO: Put some label that prevents them for conitnueing and ask them to try again
			// GD.Print("Going Home");
			return;
		}

		// Do other things with the data Ig
		var data = JsonSerializer.Deserialize<LoginResponse>(json);
		PlayerStateManager.Instance.SetPlayerData(data);
		GD.Print("Successful Message received");
		try
		{
			var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
			GameStateManager.ChangeGameState(GameState.HOMESCREEN);

		}
		catch (Exception e)
		{
			GD.PrintErr("Failled Home", e);

		}

	}
}
