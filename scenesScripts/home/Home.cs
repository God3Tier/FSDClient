namespace FSDClient.home;

using Godot;
using System;
using FSDClient.autoLoad;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FSDClient.card;

class MatchStatusResponse
{
	[JsonPropertyName("matched")]
	public bool Matched { get; set; }
	[JsonPropertyName("session_id")]
	public string SessionID { get; set; }
	[JsonPropertyName("opponent")]
	public string Opponent { get; set; }
	[JsonPropertyName("your_mmr")]
	public int YourMMR { get; set; }
	[JsonPropertyName("their_mmr")]
	public int TheirMMR { get; set; }

	public MatchStatusResponse(bool matched, string sessionId, string opponent, int yourMMR, int theirMMR)
	{
		Matched = matched;
		SessionID = sessionId;
		Opponent = opponent;
		YourMMR = yourMMR;
		TheirMMR = theirMMR;
	}
}

class AcceptMatchResponse
{
	[JsonPropertyName("message")]
	public string Message { get; set; }

	[JsonPropertyName("session_id")]
	public string SessionId { get; set; }

	[JsonPropertyName("status")]
	public string Status { get; set; }

	[JsonPropertyName("player1_ready")]
	public bool Player1Ready { get; set; }

	[JsonPropertyName("player2_ready")]
	public bool Player2Ready { get; set; }

	public AcceptMatchResponse(string message, string sessionId, string status, bool player1Ready, bool player2Ready)
	{
		Message = message;
		SessionId = sessionId;
		Status = status;
		Player1Ready = player1Ready;
		Player2Ready = player2Ready;
	}
}

public class ActiveDeckResponse
{
	[JsonPropertyName("active_deck_id")]
	public int ActiveDeckId { get; set; }

	[JsonPropertyName("deck")]
	public Deck Deck { get; set; }
}

public class Deck
{
	[JsonPropertyName("deck_id")]
	public int DeckId { get; set; }
	
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";
	
	[JsonPropertyName("card_ids")]
	public List<int> CardIds { get; set; } = new();
	
	[JsonPropertyName("cards")]
	public List<CardInfo> Cards { get; set; } = new();
}

public class CardInfo
{
	[JsonPropertyName("card_id")]
	public int CardId { get; set; }
	
	[JsonPropertyName("position")]
	public int Position { get; set; }
	
	[JsonPropertyName("level")]
	public int Level { get; set; }

}

public class GetCrystalResponse
{
	[JsonPropertyName("crystals")]
	public int Crystals { get; set; }

}

public class GetPacksResponse
{
	[JsonPropertyName("packs")]
	public List<PackData> Packs { get; set; }

}

public class PackData
{
	[JsonPropertyName("pack_id")]
	public int PackId { get; set; }

	[JsonPropertyName("pack_type")]
	public string PackType { get; set; }

	[JsonPropertyName("is_opened")]
	public bool IsOpened { get; set; }

	[JsonPropertyName("created_at")]
	public string CreatedAt { get; set; }

}

public class OpenPackResponse
{
	[JsonPropertyName("pack_id")]
	public int PackId { get; set; }

	[JsonPropertyName("pack_type")]
	public string PackType { get; set; }

	[JsonPropertyName("crystals_awarded")]
	public int CrystalsAwarded { get; set; }
	
	[JsonPropertyName("crystals_total")]
	public int CrystalsTotal { get; set; }

	[JsonPropertyName("cards")]
	public List<PackCardData> Cards { get; set; }
}

public class PackCardData
{
	[JsonPropertyName("card_id")]
	public int CardId { get; set; }

	[JsonPropertyName("card_name")]
	public string CardName { get; set; }

	[JsonPropertyName("rarity")]
	public string Rarity { get; set; }

	[JsonPropertyName("quantity")]
	public int Quantity { get; set; }

}

public partial class Home : Control
{
	private volatile bool _searching = false;
	private volatile bool _cancelled = false;
	private volatile bool _accepted = false;
	private PlayerStateManager CurrentPlayer { get; set; }
	private NetworkManager Network { get; set; }
	private string SessionID { get; set; }
	private readonly Dictionary<string, Texture2D> PackTextures = new Dictionary<string, Texture2D>
	{
		{"none", GD.Load<Texture2D>("res://assets/cards/pack.png")},
		{"common", GD.Load<Texture2D>("res://assets/cards/CommonPack.png")},
		{"rare", GD.Load<Texture2D>("res://assets/cards/RarePack.png")},
		{"epic", GD.Load<Texture2D>("res://assets/cards/EpicPack.png")},
		{"legendary", GD.Load<Texture2D>("res://assets/cards/LegendaryPack.png")}
	};

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

		try
		{
			// update player information
			InitialisePlayerInformation();

			// Update packs on pressed function
			UpdatePackSlotPressedFunction();
			// Function to get pack
			FetchPacks();
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

		// update crystals
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/players/me/crystals", Godot.HttpClient.Method.Get, "", GetCrystalResponse);		

		// Set Header -> Row1 -> Gold ->  Label
		var Gld = (Label)((ColorRect)Row1.FindChild("Gold", true)).FindChild("Label");
		Gld.Text = CurrentPlayer.Gold.ToString();

		// Set Header -> Row2 -> Name ->  Label
		var Username = (Label)((ColorRect)Row1.FindChild("Name", true)).FindChild("Label");
		Username.Text = CurrentPlayer.PlayerData.Username;

	}
	
	private void GetCrystalResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print(responseCode);
		string json = System.Text.Encoding.UTF8.GetString(body);
		if (result != (long)HttpRequest.Result.Success || responseCode != 200)
		{
			GD.PrintErr("Failed in fetching crystals");
			SetTextPopup("Failed in fetching crystals");
			return;
		}

		// Do other things with the data
		var data = JsonSerializer.Deserialize<GetCrystalResponse>(json);
		UpdateCrystals(data.Crystals);
	}

	// Function to update crystal count in header and in player data
	private void UpdateCrystals(int CrystalsTotal)
	{
		var Header = (BoxContainer)FindChild("Header");
		var Row1 = (BoxContainer)Header.FindChild("Row 1");
		
		CurrentPlayer.Crystal = CrystalsTotal;

		// Set Header -> Row1 -> Crystal ->  Label
		var Cry = (Label)((ColorRect)Row1.FindChild("Crystal", true)).FindChild("Label");
		Cry.Text = CurrentPlayer.Crystal.ToString();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//	Press Battle button
	public async void _OnBattleButtonPressed()
	{
		// to check if theres 12 cards in active deck
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/decks/active", Godot.HttpClient.Method.Get, "", ActiveDeckResponse);

	}

	// Press card button
	public void _OnCardButtonPressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.CARDSCREEN);
	}

	// Press cancel button when finding match
	public void _OnCancelButtonPressed()
	{
		// Send request to cancel the game connection
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/queue", Godot.HttpClient.Method.Delete, "", CancelMatchResponse);
		_cancelled = true;

		_searching = false;
		Control loadingNode = GetNode<ColorRect>("Loading");

		// stop animation
		AnimatedSprite2D LoadingSprite = GetNode<AnimatedSprite2D>("Loading/LoadingAnimation");
		LoadingSprite.Stop();  // Replace "default" with your animation name

		// Turn it OFF (make visible false)
		loadingNode.Visible = false;
	}

	// Press friend button (to show friend list)
	public void _OnFriendButtonPressed()
	{
		Control FriendPopupContainerNode = GetNode<Control>("FriendPopupContainer");

		// Turn it ON (make visible)
		FriendPopupContainerNode.Visible = true;
	}

	// Button press function when clicking background to close friends popup
	public void _OnFriendPopupBackgroundPressed()
	{
		CloseFriendPopup();
	}

	// Button press function when clicking X to close friends popup
	public void _OnCloseFriendButtonPressed()
	{
		CloseFriendPopup();
	}

	// Function to close friend popup
	public void CloseFriendPopup()
	{
		Control FriendPopupContainerNode = GetNode<Control>("FriendPopupContainer");

		// Turn it ON (make visible)
		FriendPopupContainerNode.Visible = false;
	}


	/*
	*  Network helper functions and managers
	*/
	private void JoinResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("MATCH CHECK JOIN LOGGING");
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response about state of matchmaking");
			SetTextPopup("Failed to get a successful response about state of matchmaking");
			return;
		}

		_searching = true;

	}

	private void MatchCheckResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("MATCH CHECK RESPONSE LOGGING");
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response about state of matchmaking");
			SetTextPopup("Failed to get a successful response about state of matchmaking");
			return;
		}


		var data = JsonSerializer.Deserialize<MatchStatusResponse>(json);
		if (data.Matched == false)
		{
			GD.Print("Not matched");
			return;
		}

		var PlayerStateManager = GetNode<PlayerStateManager>("/root/PlayerStateManager");
		PlayerStateManager.SessionId = data.SessionID;
		SessionID = data.SessionID;
		_searching = false;
	}

	private void CancelMatchResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("CANCEL MATCH RESPONSE LOGGING");
		GD.Print(responseCode);
		GD.Print(System.Text.Encoding.UTF8.GetString(body));
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to leave current queue");
			SetTextPopup("Failed to leave current queue");
			return;
		}
	}

	private void AcceptMatchResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("ACCEPTING MATCH RESPONSE LOGGING");
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		GD.Print(json);

		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Unable to accept match");
			SetTextPopup("Unable to accept match");
			
			return;
		}
		try
		{
			var Response = JsonSerializer.Deserialize<AcceptMatchResponse>(json);
			PlayerStateManager.Instance.SessionId = Response.SessionId;

		}
		catch (Exception e)
		{
			GD.Print(e);
		}

		GD.Print("Successfully able to end thus properly");
		_searching = false;
		_accepted = true;
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.INGAMEMODE);
	}

	private async void ActiveDeckResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);

		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Unable to get active deck");
			SetTextPopup("Unable to get active deck");
			return;
		}

		ActiveDeckResponse data = JsonSerializer.Deserialize<ActiveDeckResponse>(json);
		ActiveDeckResponse ActiveDeck = data;
		
		PlayerStateManager.Instance.ActiveDeck = ActiveDeck;
		
		// check that there is 12 cards (aka deck is valid)
		if (ActiveDeck.Deck.CardIds.Count != 12){
			SetTextPopup("Cannot queue: active deck must have exactly 12 cards, currently has " + ActiveDeck.Deck.CardIds.Count);
			return;
		}

		ColorRect loadingNode = GetNode<ColorRect>("Loading");
		// 1. Add player to queue
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/queue", Godot.HttpClient.Method.Post, "", JoinResponse);

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
			await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

			// 2. Check if the player has been matched
			Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/match", Godot.HttpClient.Method.Get, "", MatchCheckResponse);
		}


		if (_cancelled)
		{
			return;
		}
		// 3. Accept the match and proceed
		var obj = new
		{
			session_id = SessionID
		};

		var jsonString = JsonSerializer.Serialize(obj);
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.MATCHMAKING + "/matchmaking/match/accept", Godot.HttpClient.Method.Post, jsonString, AcceptMatchResponse);
	}


	// Update packs on pressed function
	private void UpdatePackSlotPressedFunction()
	{
		// Make each pack clickable
		Button PackSlot0 = GetNode<Button>("Packs/PackSlot0");
		Button PackSlot1 = GetNode<Button>("Packs/PackSlot1");
		Button PackSlot2 = GetNode<Button>("Packs/PackSlot2");
		Button PackSlot3 = GetNode<Button>("Packs/PackSlot3");

		// All call the same function, but with different index
		PackSlot0.Pressed += () => OnPackSlotPressed(0);
		PackSlot1.Pressed += () => OnPackSlotPressed(1);
		PackSlot2.Pressed += () => OnPackSlotPressed(2);
		PackSlot3.Pressed += () => OnPackSlotPressed(3);
	}

	// Function to fetch backend for packs
	private void FetchPacks()
	{
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/packs", Godot.HttpClient.Method.Get, "", GetPacksResponse);
	}


	private void GetPacksResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print(responseCode);
		string json = System.Text.Encoding.UTF8.GetString(body);
		if (result != (long)HttpRequest.Result.Success || responseCode != 200)
		{
			GD.PrintErr("Failed when fetching packs");
			SetTextPopup("Failed when fetching packs");
			return;
		}

		// Do other things with the data
		var data = JsonSerializer.Deserialize<GetPacksResponse>(json);
		GD.Print("Successful Message received");
		UpdatePacksVisually(data);
	}

	// Function to set packs visually
	private void UpdatePacksVisually(GetPacksResponse PacksData)
	{
		var PacksContainer = GetNode<HFlowContainer>("Packs");
		var PackSlotCounter = 0;

		for (int i = 0; i < PacksData.Packs.Count; i++)
		{
			var IndividualPack = PacksData.Packs[i];
			// false means show the pack
			if (IndividualPack.IsOpened == false)
			{
				var PackSlot = PacksContainer.GetChild(PackSlotCounter) as Button;
				var PackImage = PackSlot.GetNode<TextureRect>("PackImage");
				PackImage.Texture = PackTextures[IndividualPack.PackType];
				PackImage.SetMeta("PackId", IndividualPack.PackId);
				PackSlotCounter++;
			}
		}

		// means no more packs to show
		for (int i = PackSlotCounter; i < 4; i++)
		{
			var PackSlot = PacksContainer.GetChild(i) as Button;
			var PackImage = PackSlot.GetNode<TextureRect>("PackImage");
			PackImage.Texture = PackTextures["none"];
			PackImage.SetMeta("PackId", default(Variant));
		}
	}

	// When the pack slot is being pressed
	private void OnPackSlotPressed(int index)
	{
		Button PackSlot = GetNode<Button>("Packs/PackSlot" + index);

		TextureRect Pack = PackSlot.GetNode<TextureRect>("PackImage");

		// if got pack then call
		if (Pack.HasMeta("PackId"))
		{
			// do the async call here to open pack
			OpenPack((string)Pack.GetMeta("PackId"));
		}
	}

	// Function to fetch backend to open packs
	private void OpenPack(string index)
	{
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/packs/open?pack_id=" + index, Godot.HttpClient.Method.Post, "", OpenPackResponse);
	}

	// response for opening packs
	private void OpenPackResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print(responseCode);
		string json = System.Text.Encoding.UTF8.GetString(body);
		if (result != (long)HttpRequest.Result.Success || responseCode != 200)
		{
			GD.PrintErr("Failed when opening packs");
			SetTextPopup("Failed when opening packs");
			return;
		}

		// Do other things with the data
		var data = JsonSerializer.Deserialize<OpenPackResponse>(json);
		GD.Print("Successful Message received");
		OpenPackVisuals(data);
	}

	// Function to show the pack opening animations and visuals
	private void OpenPackVisuals(OpenPackResponse data)
	{
		//open pack visuals
		ColorRect PacksPopupContainer = GetNode<ColorRect>("PacksPopupContainer");
		Control IndivCardContainer = PacksPopupContainer.GetNode<Control>("IndivCardContainer");
		HFlowContainer FinalCardsResult = PacksPopupContainer.GetNode<HFlowContainer>("PacksPopupBackground/FinalCardsResultScroll/FinalCardsResult");

		// create a screen showing all cards (Visibility off)
		FinalCardsResult.Visible = false;

		// create crystal card (need duplicate since 1st and 2nd need to be using diff pointers if not it will affect each other)
		VBoxContainer CrystalContainer = CreateCrystalCard(data.CrystalsAwarded);
		IndivCardContainer.AddChild(CrystalContainer);
		VBoxContainer CrystalContainer2 = CreateCrystalCard(data.CrystalsAwarded);
		FinalCardsResult.AddChild(CrystalContainer2);

		List<PackCardData> Cards = data.Cards;
		for (int i = 0; i < Cards.Count; i++)
		{
			// set generate cards one on top of another
			VBoxContainer CardContainer = CreateCard(Cards[i].Rarity, Cards[i].CardId, Cards[i].Quantity);

			// Set visibility to false (for all but 1st)
			CardContainer.Visible = false;

			IndivCardContainer.AddChild(CardContainer);

			VBoxContainer FinalCardContainer = CreateCard(Cards[i].Rarity, Cards[i].CardId, Cards[i].Quantity);
			FinalCardsResult.AddChild(FinalCardContainer);
		}

		// Turn it ON (make visible)
		PacksPopupContainer.Visible = true;

		// Update total crystal counter
		UpdateCrystals(data.CrystalsTotal);

		// Update packs visually
		FetchPacks();
	}

	// Function to create a card for display
	private static VBoxContainer CreateCard(string rarity, int CardId, int count)
	{
		var CardContainer = new VBoxContainer();
		CardContainer.CustomMinimumSize = new Vector2(226, 346);

		// add Rarity label into CardContainer
		var RarityLabel = new Label();
		RarityLabel.Text = rarity;
		RarityLabel.AddThemeFontSizeOverride("font_size", 39);
		RarityLabel.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		RarityLabel.HorizontalAlignment = HorizontalAlignment.Center;
		CardContainer.AddChild(RarityLabel);

		// Create CardControl with card inside
		var CardControl = new Control();
		CardControl.CustomMinimumSize = new Vector2(200, 230);
		CardControl.Position = new Vector2(13, 58);
		CardControl.SizeFlagsHorizontal = (Control.SizeFlags)SizeFlags.ShrinkCenter;

		// Create the card itself
		var Card = CardBuilder.GenerateCard(CardId);
		Card.Position = new Vector2(100, 115);

		CardControl.AddChild(Card);
		CardContainer.AddChild(CardControl);

		// Add CardCountLabel and add into CardContainer
		var CardCountLabel = new Label();
		CardCountLabel.Text = "X" + count;
		CardCountLabel.AddThemeFontSizeOverride("font_size", 39);
		CardCountLabel.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		CardCountLabel.HorizontalAlignment = HorizontalAlignment.Center;
		CardContainer.AddChild(CardCountLabel);

		return CardContainer;
	}

	// Function to create Crystal value card for display
	private static VBoxContainer CreateCrystalCard(int quantity)
	{
		var CardContainer = new VBoxContainer();
		CardContainer.CustomMinimumSize = new Vector2(226, 346);
		CardContainer.Position = new Vector2(0, 58);

		// Create Crystal
		var CrystalTextureRect = new TextureRect();
		CrystalTextureRect.Texture = GD.Load<Texture2D>("res://assets/header/crystal_colored.png");
		CrystalTextureRect.CustomMinimumSize = new Vector2(200, 230);
		CrystalTextureRect.Position = new Vector2(13, 58);
		CrystalTextureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
		CrystalTextureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

		CardContainer.AddChild(CrystalTextureRect);

		// Add CardCountLabel and add into CardContainer
		var CardCountLabel = new Label();
		CardCountLabel.Text = "X" + quantity;
		CardCountLabel.AddThemeFontSizeOverride("font_size", 39);
		CardCountLabel.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		CardCountLabel.HorizontalAlignment = HorizontalAlignment.Center;
		CardContainer.AddChild(CardCountLabel);

		return CardContainer;
	}

	// When clicking the background
	public void _OnPacksPopupBackgroundPressed()
	{

		ColorRect PacksPopupContainer = GetNode<ColorRect>("PacksPopupContainer");
		Control IndivCardContainer = PacksPopupContainer.GetNode<Control>("IndivCardContainer");
		HFlowContainer FinalCardsResult = PacksPopupContainer.GetNode<HFlowContainer>("PacksPopupBackground/FinalCardsResultScroll/FinalCardsResult");

		// if still have card to show
		if (IndivCardContainer.GetChildCount() > 1)
		{
			// make bottom card visible
			VBoxContainer NewCardContainer = IndivCardContainer.GetChild(1) as VBoxContainer;
			NewCardContainer.Visible = true;

			// move card animation
			VBoxContainer CardContainer = IndivCardContainer.GetChild(0) as VBoxContainer;
			SlideOutAndDelete(CardContainer);
		}
		else if (IndivCardContainer.GetChildCount() == 1)
		{
			// no more card to show afterwards
			// move card animation
			VBoxContainer CardContainer = IndivCardContainer.GetChild(0) as VBoxContainer;
			SlideOutAndDelete(CardContainer);

			// display all cards (set to visible)
			FinalCardsResult.Visible = true;
		}
		else
		{
			// Means showing all cards already
			// Do clean up and remove all info
			FinalCardsResult.Visible = false;
			// Need delete all items to prepare if user open another pack
			for (int i = FinalCardsResult.GetChildCount() - 1; i >= 0; i--)
			{
				FinalCardsResult.GetChild(i).QueueFree();
			}

			PacksPopupContainer.Visible = false;
		}

	}

	// Animation to slide out and delete
	private static void SlideOutAndDelete(Node node)
	{
		var tween = node.CreateTween();
		tween.TweenProperty(node, "position:x", 200f, 0.3f)
			 .SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(() => node.QueueFree()));
	}
	
	// function to make text popup visible and set the text
	private void SetTextPopup(string Text)
	{
		Control TextPopupContainer = GetNode<Control>("TextPopupContainer");
		Label TextLabel = TextPopupContainer.GetNode<Label>("TextBackgroundContainer/MarginTextContainer/TextLabel");
		TextPopupContainer.Visible = true;
		TextLabel.Text = Text;
	}
	
	// Close text popup container
	private void _OnTextPopupBackgroundPressed()
	{
		Control TextPopupContainer = GetNode<Control>("TextPopupContainer");
		TextPopupContainer.Visible = false;
	}
}
