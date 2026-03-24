namespace FSDClient.home;

using Godot;
using System;
using FSDClient.autoLoad;
using System.Text.Json;


class MatchStatusResponse
{
	public bool Matched { get; set; }
	public string SessionId { get; set; }
	public string Opponent { get; set; }
	public int YourMMR { get; set; }
	public int TheirMMR { get; set; }

	public MatchStatusResponse(bool matched, string sessionId, string opponent, int yourMMR, int theirMMR)
	{
		Matched = matched;
		SessionId = sessionId;
		Opponent = opponent;
		YourMMR = yourMMR;
		TheirMMR = theirMMR;
	}
}

public partial class Home : Control
{

	private volatile bool _searching = false;
	private PlayerStateManager CurrentPlayer { get; set; }
	private NetworkManager Network { get; set; }
	private string[] Header { get; } = new string[2];
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Network = NetworkManager.Instance;
		try
		{
			CurrentPlayer = PlayerStateManager.Instance;
			GD.Print(CurrentPlayer.ToString());
			if (CurrentPlayer.PlayerData == null)
			{
				GD.Print("The PlayerData is empty");
				var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
				GameStateManager.ChangeGameState(GameState.LOGIN);
				return;
			}

		}
		catch (Exception e)
		{
			GD.PrintErr("Whoops ", e);
		}

		var token = CurrentPlayer.Token;
		Header[1] = $"Authorization: Bearer {token}";

		try
		{
			InitialisePlayerInformation();
		}
		catch (Exception e)
		{
			GD.PrintErr("Whoops", e);
		}
	}

	private void InitialisePlayerInformation()
	{
		// Set player Icon TODO: I need Windy's code to be merged before I can do this

		var Header = (BoxContainer)FindChild("Header");
		var Row1 = (BoxContainer)Header.FindChild("Row 1");
		var Row2 = (BoxContainer)Header.FindChild("Row 2");

		// GD.Print(Row1, Row2);

		// Set Header -> Row1 -> XP -> Banner -> Level
		var Lv = (Label)((TextureRect)((ColorRect)Row1.FindChild("XP")).FindChild("Banner")).FindChild("Level");
		Lv.Text = CurrentPlayer.Level.ToString();
		// Set Header -> Row1 -> Crystal ->  Label
		var Cry = (Label)((ColorRect)Row1.FindChild("Crystal", true)).FindChild("Label");
		Cry.Text = CurrentPlayer.Crystal.ToString();

		// Set Header -> Row1 -> Gold ->  Label
		var Gld = (Label)((ColorRect)Row1.FindChild("Gold", true)).FindChild("Label");
		Gld.Text = CurrentPlayer.Gold.ToString();

		// Set Header -> Row2 -> Name ->  Label
		var Username = (Label)((ColorRect)Row2.FindChild("Name", true)).FindChild("Label");
		Username.Text = CurrentPlayer.PlayerData.Username;


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//	Press Battle button
	public async void _on_battle_button_pressed()
	{

		Control loadingNode = GetNode<ColorRect>("Loading");
		// 1. Add player to queue
		Network.SendRequest(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/join", Godot.HttpClient.Method.Post, "", MatchCheckResponse, Header);

		// start animation
		AnimatedSprite2D LoadingSprite = GetNode<AnimatedSprite2D>("Loading/LoadingAnimation");
		LoadingSprite.Play("loading");  // Replace "default" with your animation name

		// Turn it ON (make visible)
		loadingNode.Visible = true;
		_searching = true;

		// find game stuff
		while (_searching)
		{

			// 1 SECOND TIMEOUT - Godot way
			await ToSignal(GetTree().CreateTimer(10.0f), "timeout");

			// 2. Check if the player has been matched
			Network.SendRequest(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/match", Godot.HttpClient.Method.Get, "", MatchCheckResponse, Header);
		}


		// 3. Accept the match and proceed
	}

	// Press card button
	public void _on_card_button_pressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.CARDSCREEN);
	}

	// Press cancel button when finding match
	public void _on_cancel_button_pressed()
	{
		// Send request to cancel the game connection
		Network.SendRequest(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/leave", Godot.HttpClient.Method.Post, "", CancelMatchResponse, Header);
		_searching = false;
		Control loadingNode = GetNode<ColorRect>("Loading");

		// stop animation
		AnimatedSprite2D LoadingSprite = GetNode<AnimatedSprite2D>("Loading/LoadingAnimation");
		LoadingSprite.Stop();  // Replace "default" with your animation name

		// Turn it OFF (make visible false)
		loadingNode.Visible = false;
	}

	// Press friend button (to show friend list)
	public void _on_friend_button_pressed()
	{
		Control FriendPopupContainerNode = GetNode<Control>("FriendPopupContainer");

		// Turn it ON (make visible)
		FriendPopupContainerNode.Visible = true;
	}

	public void _on_friend_popup_background_pressed()
	{
		close_friend_popup();
	}

	public void _on_close_friend_button_pressed()
	{
		close_friend_popup();
	}

	public void close_friend_popup()
	{
		Control FriendPopupContainerNode = GetNode<Control>("FriendPopupContainer");

		// Turn it ON (make visible)
		FriendPopupContainerNode.Visible = false;
	}


	// Network helper functions and managers
	private void MatchCheckResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 &&  responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response about state of matchmaking");
			GD.PrintErr(json);
			return;
		}


		var data = JsonSerializer.Deserialize<MatchStatusResponse>(json);
		if (data.Matched == false)
		{
			return;
		}
		_searching = false;
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.INGAMEMODE);
	}

	private void CancelMatchResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to leave current queue");
			GD.PrintErr(System.Text.Encoding.UTF8.GetString(body));
			return;
		}


	}
}
