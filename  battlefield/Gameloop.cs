namespace FSDClient.battlefield;

using FSDClient.builder;
using Godot;
using FSDClient.player.display;
using FSDClient.card.display;
using FSDClient.card;
using FSDClient.battlefield.handManagement;
using FSDClient.autoLoad;
using FSDClient.battlefield.responseType;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Text.Json;
using FSDClient.battlefield.response;


public partial class Gameloop : Node2D
{
	// Store latest player and enemy health
	private int LatestPlayerHealth = -1;
	private int LatestEnemyHealth = -1;

	public static readonly string BASE_WEBSOCKET_URL = "ws://localhost:8002/ws?session_id=SESSIONID";

	public static readonly double MAX_ELIXER = 8;
	public static readonly double ROUND_TIMER = 10.0;
	public static readonly double PAUSE_TIMER = 10.0;
	public static readonly double SECONDS_PER_ELIXIR = 3f;
	public static readonly int BASE_ELIXIR = 4;

	// Unsure to keep this as the state manager or make it it's own individual player data. TBD on a later date
	public PlayerStateManager MainPlayer { get; set; }
	// References to player and enemy icons for health updates
	private PlayerView _playerIcon;
	private PlayerView _enemyIcon;

	// Deal with card placement and card movement
	private Slot[] Board { get; set; } = new BattleSlot[6];
	private Slot[] OpponentBoard { get; set; } = new BattleSlot[6];
	private CardManager CardManager;
	private HandArea HandArea;

	// Deal with active gameplay
	public PlayerState PlayerState { get; set; }

	// Parameters for game state to be managed
	private int Elixir { get; set; }
	private double GameTimer { get; set; } = 0;
	private double RegenInterval { get; set; } = 1;
	private bool TurnPause { get; set; } = true;
	private double PauseTimer { get; set; } = 0;
	private int TurnRound = 1;
	private bool GameEnd { get; set; } = false;

	// Websocket Connection and managing concurrency
	private WebSocketPeer Socket = new WebSocketPeer();
	private readonly ConcurrentQueue<string> readQueue = new();
	private double _reconnectTimer { get; set; }
	private readonly double _reconnectDelay = 3.0;
	private string _currentPhase = string.Empty;
	private bool _phaseDrivenByServer = false;

	// I am of the assumption that this is what is being called by the
	// We put this in gameloop later
	public override void _Ready()
	{
		MainPlayer = PlayerStateManager.Instance;
		try
		{
			var token = MainPlayer.Token;
			GD.Print("Token ", token);
			Socket.HandshakeHeaders = new string[]
			{
				"Authorization: Bearer " + token
			};
			Socket.ConnectToUrl(ConstructWebsocketUrl());

		}
		catch (Exception e)
		{
			GD.Print("Unable to connect to websocket becayse of ", e);
		}
		HandArea = GetNode<HandArea>("HandArea");

		// Load the battle slots for the player
		var BoardNode = (Control)FindChild("Board");
		foreach (Node child in BoardNode.GetChildren())
		{
			string childName = child.Name.ToString();
			if (childName.Contains("BattleSlot"))
			{
				var BattleSlot = (BattleSlot)child;
				int lastInt = (int)(childName[^1] - '1');
				Board[lastInt] = BattleSlot;
				// Map slot index to (row, col):
				// 0: (0,0), 1: (0,1), 2: (0,2), 3: (1,0), 4: (1,1), 5: (1,2)
				int[,] slotCoords = new int[,] { { 0, 0 }, { 0, 1 }, { 0, 2 }, { 1, 0 }, { 1, 1 }, { 1, 2 } };
				if (lastInt >= 0 && lastInt < 6)
				{
					BattleSlot.y = slotCoords[lastInt, 0]; // row
					BattleSlot.x = slotCoords[lastInt, 1]; // col
				}
			}
		}

		// Load the battle slots for the opponent
		var OpponentBoardNode = (Control)FindChild("OpponentBoard");
		foreach (Node child in OpponentBoardNode.GetChildren())
		{
			string childName = child.Name.ToString();
			if (childName.Contains("BattleSlot"))
			{
				var BattleSlot = (BattleSlot)child;
				int lastInt = (int)(childName[^1] - '1');
				OpponentBoard[lastInt] = BattleSlot;
			}
		}

		CardManager = (CardManager)FindChild("CardManager", true);
		CardManager._playerHand = GetNode<PlayerHand>("HandArea/PlayerHand");
		CardManager._deckSpace = GetNode<DeckSpace>("HandArea/DeckSpace");
		CardManager.CardDropped += OnCardDropped;
		CardManager._playerHand.AddCardMessage += OnCardAdd;
		CardManager._deckSpace.RemoveCardMessage += OnCardReturn;

		// Testing the HandArea
		HandArea = GetNode<HandArea>("HandArea");
		HandArea._playerHand = CardManager._playerHand;
		HandArea._deckSpace = CardManager._deckSpace;
		GD.Print(HandArea);
		GD.Print(CardManager._playerHand.Name);

		// Add player icon
		try
		{
			var PlayerTextureView = Builder.BuildPlayer(MainPlayer.PlayerData);
			// var PlayerIcon = (PlayerView)FindChild("PlayerIcon");
			var PlayerIcon = (PlayerView)FindChild("PlayerIcon", true);
			PlayerIcon.LoadDataTexture(PlayerTextureView);
			PlayerIcon.Scale = new Vector2(0.35f, 0.35f);


			_playerIcon = PlayerIcon;
			_enemyIcon = (PlayerView)FindChild("PlayerIcon2", true);
			_enemyIcon.Scale = new Vector2(0.35f, 0.35f);	
			_enemyIcon.LoadDataTexture(PlayerTextureView);
		}
		catch (Exception e)
		{
			GD.PrintErr("Exception: ", e);
		}
	}

	private void OnCardAdd(int cardID)
	{
		var obj = new
		{
			card_id = cardID
		};
		WriteToServer(RequestAction.SELECT_CARD, JsonSerializer.Serialize(obj));
	}

	private void OnCardReturn(int cardID)
	{
		var obj = new
		{
			card_id = cardID
		};
		WriteToServer(RequestAction.DESELECT_CARD, JsonSerializer.Serialize(obj));
	}

	private string ConstructWebsocketUrl()
	{
		var session = PlayerStateManager.Instance.SessionId;
		string BaseUrl = BASE_WEBSOCKET_URL.Replace("SESSIONID", session);
		GD.Print(BaseUrl);
		return BaseUrl;
	}

	// This function is a proof of concept
	private void OnCardDropped(BattleSlot battleslot)
	{
		// TODO: Write to Server should be implemented once backend decides how to transfer information
		var obj = new
		{
			card_id = battleslot.Card.CardID,
			row = battleslot.y,
			col = battleslot.x,
		};

		WriteToServer(RequestAction.CARD_PLACED, JsonSerializer.Serialize(obj));
		// Simulate delay of card
		// Thread.Sleep(1);
		battleslot.Card.ActiveY = battleslot.y;
		battleslot.Card.ActiveX = battleslot.x;

	}

	// This is how to generate Elixir. Since client only ever knows about 1 player's resource
	// i can do it within the gameplay loop itself
	public override void _Process(double delta)
	{
		HandleWebSocket();
		HandleGameTimer(delta);
		HandleInputFromServer();

	}

	private void HandleWebSocket()
	{
		Socket.Poll();
		var state = Socket.GetReadyState();

		if (state == WebSocketPeer.State.Connecting)
		{
			return;
		}

		if (state != WebSocketPeer.State.Open)
		{
			// Do Some connection error and try to rehandle
			_reconnectTimer += GetProcessDeltaTime();
			if (_reconnectTimer < _reconnectDelay) return;
			_reconnectTimer = 0;
			try
			{
				Socket.ConnectToUrl(ConstructWebsocketUrl());

			}
			catch (Exception e)
			{
				GD.Print("Unable to connect to websocket because of ", e);
			}
			WriteToServer(RequestAction.RECONNECT);

			return;
		}

		while (Socket.GetAvailablePacketCount() > 0)
		{
			readQueue.Enqueue(Socket.GetPacket().GetStringFromUtf8());
		}

	}

	private void HandleInputFromServer()
	{
		while (readQueue.TryDequeue(out string msg))
		{
			GD.Print(msg);
			try
			{
				var data = JsonSerializer.Deserialize<ResponseManager>(msg);
				if (data == null)
				{
					continue;
				}


				if (!string.IsNullOrEmpty(data.Result) && data.Result.Equals("failure"))
				{
					// If the action was CARD_PLACED, restore the card to the player's hand
					if (!string.IsNullOrEmpty(data.ActionType) &&
						data.ActionType.Equals("CARD_PLACED", StringComparison.OrdinalIgnoreCase))
					{
						// Try to extract card_id from parameters if available
						int cardId = -1;
						if (data.Parameters.ValueKind == System.Text.Json.JsonValueKind.Object &&
							data.Parameters.TryGetProperty("card_id", out var cardIdProp) &&
							cardIdProp.TryGetInt32(out int extractedId))
						{
							cardId = extractedId;
						}
						if (cardId != -1 && CardManager != null)
						{
							bool restored = false;
							// Try to restore from any slot
							foreach (var slot in Board)
							{
								if (slot is BattleSlot battleSlot && battleSlot.Card != null && battleSlot.Card.CardID == cardId)
								{
									CardManager.BounceBattleSlot(battleSlot);
									restored = true;
									break;
								}
							}
							// If not found in any slot, recreate and return to hand
							if (!restored)
							{
								var cardTemp = CardBuilder.GenerateCard(cardId);
								CardManager._playerHand.AddCard(cardTemp);
							}
						}
					}
					return;
				}

				if (!string.IsNullOrEmpty(data.MessageType) &&
					data.MessageType.Equals("error", StringComparison.OrdinalIgnoreCase))
				{
					GD.PrintErr("Server error: ", data.ErrorMessage);
					continue;
				}

				if (data.StateView.ValueKind != JsonValueKind.Undefined &&
					data.StateView.ValueKind != JsonValueKind.Null)
				{
					try
					{
						PlayerState = JsonSerializer.Deserialize<PlayerState>(data.StateView);
						if (PlayerState != null)
						{
							ApplyPhaseIfChanged(PlayerState.Phase);
						}
					}
					catch (Exception e)
					{
						GD.PrintErr("Unable to parse state view ", e);
					}
				}

				if (!string.IsNullOrEmpty(data.ActionType) &&
					data.ActionType.Equals("TICK_UPDATE", StringComparison.OrdinalIgnoreCase))
				{
					if (data.Parameters.ValueKind == JsonValueKind.Undefined ||
						data.Parameters.ValueKind == JsonValueKind.Null)
					{
						continue;
					}
					// GD.Print("Received tick update");
					var tickUpdate = JsonSerializer.Deserialize<TickUpdater>(data.Parameters);

					if (tickUpdate == null)
					{
						continue;
					}

					ApplyTickUpdate(tickUpdate);
					SyncBoardState(tickUpdate);
					SyncDeckSpaceIfNeeded(tickUpdate);
					SyncHandIfNeeded(tickUpdate);
				}

			}
			catch (Exception e)
			{
				GD.PrintErr("Unable to serialize ", e);
			}

			// Here, we somehow parse said information about card and then mess around with it. But to continue, I need to settle card Dictionary
		}
	}

	private void SyncHandIfNeeded(TickUpdater tickUpdate)
	{
		if (tickUpdate.Hand == null)
		{
			return;
		}

		if (HandMatchesServer(tickUpdate.Hand))
		{
			return;
		}

		CardManager._playerHand.SuppressSignals = true;
		try
		{
			ClearHandControl(CardManager._playerHand);
			foreach (var card in tickUpdate.Hand)
			{
				var cardTemp = CardBuilder.GenerateCard(card.CardID);
				ApplyHandCardStats(cardTemp, card.Attack, card.Hp, card.ManaCost);
				cardTemp.CurrentSlotStatus = Card.SlotStatus.Hand;
				cardTemp.ZIndex = 4;
				CardManager.AddChild(cardTemp);
				CardManager._playerHand.AddCard(cardTemp);
			}
		}
		finally
		{
			CardManager._playerHand.SuppressSignals = false;
		}
	}

	private bool HandMatchesServer(HandCardView[] hand)
	{
		return MatchesCardIds(CardManager._playerHand._cardList, hand);
	}

	private bool MatchesCardIds(Card[] localCards, HandCardView[] serverCards)
	{
		if (localCards == null || serverCards == null)
		{
			return false;
		}

		var counts = new Dictionary<int, int>();
		int localCount = 0;
		foreach (var card in localCards)
		{
			if (card == null)
			{
				continue;
			}
			localCount++;
			if (!counts.TryGetValue(card.CardID, out var count))
			{
				counts[card.CardID] = 1;
			}
			else
			{
				counts[card.CardID] = count + 1;
			}
		}

		if (localCount != serverCards.Length)
		{
			return false;
		}

		foreach (var card in serverCards)
		{
			if (!counts.TryGetValue(card.CardID, out var count))
			{
				return false;
			}
			count--;
			if (count == 0)
			{
				counts.Remove(card.CardID);
			}
			else
			{
				counts[card.CardID] = count;
			}
		}

		return counts.Count == 0;
	}

	private void ApplyTickUpdate(TickUpdater tickUpdate)
	{
		if (!string.IsNullOrEmpty(tickUpdate.Phase))
		{
			ApplyPhaseIfChanged(tickUpdate.Phase);
		}

		Elixir = tickUpdate.Elixir;
		var ElixirBar = (Elixir)FindChild("Elixir");
		ElixirBar.UpdateElixir(Elixir);

		if (tickUpdate.RoundNumber > 0 && tickUpdate.RoundNumber != TurnRound)
		{
			TurnRound = tickUpdate.RoundNumber;
			ElixirBar.UpdateRound(TurnRound);
		}
	}

	private void ApplyPhaseIfChanged(string phase)
	{
		if (string.IsNullOrEmpty(phase))
		{
			return;
		}

		if (phase.Equals("GAME_OVER", StringComparison.OrdinalIgnoreCase))
		{
			if (GameEnd)
			{
				return;
			}
			GameEnd = true;
			ReturnToHomeFromGameOver();
			return;
		}

		if (string.Equals(_currentPhase, phase, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		_currentPhase = phase;
		_phaseDrivenByServer = true;

		if (phase.Equals("PRE_TURN", StringComparison.OrdinalIgnoreCase))
		{
			TurnPause = true;
			PauseTimer = 0;
			HandArea.RaiseDeck();
			CardManager.UnstuckCard();
			CardManager._playerHand.PauseCardsInHand();
			return;
		}

		if (phase.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
		{
			TurnPause = false;
			PauseTimer = 0;
			HandArea.LowerDeck();
			CardManager._playerHand.ActivateCardsInHand();
			return;
		}
	}

	private void ReturnToHomeFromGameOver()
	{
		var gameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		gameStateManager.ChangeGameState(GameState.HOMESCREEN);
	}

	private void SyncBoardState(TickUpdater tickUpdate)
	{
		if (tickUpdate.YourBoard == null && tickUpdate.EnemyBoard == null)
		{
			return;
		}

		// Update health UI for player and enemy
		if (_playerIcon != null)
		{
			var healthLabel = (RichTextLabel)_playerIcon.FindChild("DefenceValue", true);
			healthLabel.Text = tickUpdate.YourHp.ToString();
		}
		if (_enemyIcon != null)
		{
			var healthLabel = (RichTextLabel)_enemyIcon.FindChild("DefenceValue", true);
			healthLabel.Text = tickUpdate.EnemyHp.ToString();
		}

		if (tickUpdate.YourBoard != null)
		{
			var skipYourBoard = IsPreTurnPhase(tickUpdate)
				&& tickUpdate.YourBoard.Length == 0
				&& HasAnyCard(Board);
			if (!skipYourBoard && !TryUpdateBoardInPlace(tickUpdate.YourBoard, Board))
			{
				ClearBoardSlots(Board);
				foreach (var cardView in tickUpdate.YourBoard)
				{
					PlaceBoardCard(cardView, Board);
				}
			}
		}

		if (tickUpdate.EnemyBoard != null)
		{
			var skipEnemyBoard = IsPreTurnPhase(tickUpdate)
				&& tickUpdate.EnemyBoard.Length == 0
				&& HasAnyCard(OpponentBoard);
			if (!skipEnemyBoard && !TryUpdateBoardInPlace(tickUpdate.EnemyBoard, OpponentBoard))
			{
				ClearBoardSlots(OpponentBoard);
				foreach (var cardView in tickUpdate.EnemyBoard)
				{
					PlaceBoardCard(cardView, OpponentBoard);
				}
			}
		}
	}

	private bool HasAnyCard(Slot[] slots)
	{
		if (slots == null)
		{
			return false;
		}
		foreach (var slot in slots)
		{
			if (slot != null && slot.CardInSlot)
			{
				return true;
			}
		}
		return false;
	}

	private void SyncDeckSpaceIfNeeded(TickUpdater tickUpdate)
	{
		if (tickUpdate.DrawPile == null)
		{
			return;
		}

		if (!IsPreTurnPhase(tickUpdate))
		{
			return;
		}

		if (DeckSpaceMatchesDrawPile(tickUpdate.DrawPile))
		{
			return;
		}

		CardManager._deckSpace.SuppressSignals = true;
		try
		{
			ClearHandControl(CardManager._deckSpace);
			foreach (var card in tickUpdate.DrawPile)
			{
				var cardTemp = CardBuilder.GenerateCard(card.CardID);
				ApplyHandCardStats(cardTemp, card.Attack, card.Hp, card.ManaCost);
				cardTemp.CurrentSlotStatus = Card.SlotStatus.Deck;
				cardTemp.ZIndex = 4;
				CardManager.AddChild(cardTemp);
				CardManager._deckSpace.AddCard(cardTemp);
			}
		}
		finally
		{
			CardManager._deckSpace.SuppressSignals = false;
		}
	}

	private bool IsPreTurnPhase(TickUpdater tickUpdate)
	{
		var phase = tickUpdate?.Phase;
		if (string.IsNullOrEmpty(phase))
		{
			phase = _currentPhase;
		}
		if (string.IsNullOrEmpty(phase) && PlayerState != null)
		{
			phase = PlayerState.Phase;
		}
		return string.Equals(phase, "PRE_TURN", StringComparison.OrdinalIgnoreCase);
	}

	private void ClearBoardSlots(Slot[] slots)
	{
		foreach (var slot in slots)
		{
			if (slot == null || !slot.CardInSlot)
			{
				continue;
			}

			var card = slot.Card;
			slot.RemoveCard();
			card?.QueueFree();
		}
	}

	private void ClearHandControl(HandControl control)
	{
		if (control == null)
		{
			return;
		}

		for (int i = 0; i < control._cardList.Length; i++)
		{
			var card = control._cardList[i];
			if (card == null)
			{
				continue;
			}
			control.RemoveCard(card);
			card.QueueFree();
		}
	}

	private bool DeckSpaceMatchesDrawPile(HandCardView[] drawPile)
	{
		return MatchesCardIds(CardManager._deckSpace._cardList, drawPile);
	}

	private bool TryUpdateBoardInPlace(BoardCardView[] cardViews, Slot[] slots)
	{
		var hasCardInTick = new bool[slots.Length];
		foreach (var cardView in cardViews)
		{
			var index = (cardView.Row * 3) + cardView.Col;
			if (index < 0 || index >= slots.Length)
			{
				return false;
			}
			hasCardInTick[index] = true;
			if (slots[index] is not BattleSlot battleSlot || !battleSlot.CardInSlot)
			{
				return false;
			}
			if (battleSlot.Card.CardID != cardView.CardID)
			{
				return false;
			}

			ApplyBoardCardStats(battleSlot.Card, cardView.CardAttack, cardView.CurrentHealth);
			battleSlot.Card.ActiveX = cardView.Col;
			battleSlot.Card.ActiveY = cardView.Row;
		}

		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i] != null && slots[i].CardInSlot && !hasCardInTick[i])
			{
				return false;
			}
		}

		return true;
	}

	private void PlaceBoardCard(BoardCardView cardView, Slot[] slots)
	{
		var index = (cardView.Row * 3) + cardView.Col;
		if (index < 0 || index >= slots.Length)
		{
			return;
		}

		if (slots[index] is not BattleSlot battleSlot)
		{
			return;
		}

		var cardTemp = CardBuilder.GenerateCard(cardView.CardID);
		ApplyBoardCardStats(cardTemp, cardView.CardAttack, cardView.CurrentHealth);
		cardTemp.CurrentSlotStatus = Card.SlotStatus.Battle;
		cardTemp.ActiveX = cardView.Col;
		cardTemp.ActiveY = cardView.Row;
		cardTemp.Position = battleSlot.Position;
		cardTemp.ZIndex = 2;
		cardTemp.EnterBattleSlot();
		CardManager.AddChild(cardTemp);
		battleSlot.AddCard(cardTemp);

		var collisionShape = cardTemp.GetNodeOrNull<CollisionShape2D>("Area2D/CollisionShape2D");
		if (collisionShape != null)
		{
			collisionShape.Disabled = true;
		}
	}

	private void ApplyBoardCardStats(Card card, int attack, int health)
	{
		var attackLabel = (RichTextLabel)card.FindChild("Attack", true);
		attackLabel.Text = attack.ToString();
		card.Attack = attack;

		var healthLabel = (RichTextLabel)card.FindChild("Health", true);
		healthLabel.Text = health.ToString();
		card.Health = health;
	}

	private void ApplyHandCardStats(Card card, int attack, int health, int manaCost)
	{
		ApplyBoardCardStats(card, attack, health);
		var costLabel = (RichTextLabel)card.FindChild("ElixirCost", true);
		costLabel.Text = manaCost.ToString();
	}

	// This is to handle the
	private void HandleGameTimer(double delta)
	{
		var bar = GetNode<ProgressBar>("TurnBarContainer/TurnBar");

		if (_phaseDrivenByServer)
		{
			if (_currentPhase.Equals("PRE_TURN", StringComparison.OrdinalIgnoreCase))
			{
				PauseTimer += delta;
				bar.Value = PauseTimer / PAUSE_TIMER;
			}
			else if (_currentPhase.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
			{
				RegenInterval += delta;
				GameTimer += delta;
				bar.Value = (GameTimer - (ROUND_TIMER * (TurnRound - 1))) / ROUND_TIMER;
			}
			else
			{
				return;
			}
		}
		else if (TurnPause)
		{
			PauseTimer += delta;
			bar.Value = PauseTimer / PAUSE_TIMER;
		}
		else
		{
			RegenInterval += delta;
			GameTimer += delta;
			bar.Value = (GameTimer - (ROUND_TIMER * (TurnRound - 1))) / ROUND_TIMER;
		}
		if (!_phaseDrivenByServer && PauseTimer >= PAUSE_TIMER)
		{
			GD.Print("Pause Ended");
			HandArea.LowerDeck();
			CardManager._playerHand.ActivateCardsInHand();
			TurnPause = false;
			PauseTimer = 0;
			TurnRound += 1;
			var ElixirBar = (Elixir)FindChild("Elixir");
			ElixirBar.UpdateRound(TurnRound);
			return;
		}

		if (!_phaseDrivenByServer && GameTimer >= ROUND_TIMER * TurnRound && !TurnPause)
		{
			GD.Print("Round updated");
			// TODO: Trigger secondary draw card event
			TurnPause = true;
			HandArea.RaiseDeck();
			CardManager.UnstuckCard();
			CardManager._playerHand.PauseCardsInHand();
		}

		if (!_phaseDrivenByServer && RegenInterval >= SECONDS_PER_ELIXIR)
		{
			if (Elixir >= TurnRound + BASE_ELIXIR || Elixir >= MAX_ELIXER)
			{
				return;
			}
			Elixir++;
			RegenInterval = 0.0;
			var ElixirBar = (Elixir)FindChild("Elixir");
			ElixirBar.UpdateElixir(Elixir);
		}
	}

	public bool PlaceCardCheck(int Cost)
	{
		if (Elixir < Cost)
		{
			return false;
		}

		Elixir -= Cost;

		return true;
	}

	/*
	* I am going on a whim here but this should be called within the main game loop
	*/
	public void WriteToServer(RequestAction req, string parameters = "")
	{
		RequestConstructor reqAck = new();
		var message = reqAck.GenerateRequest(req, parameters, PlayerState);
		Socket.SendText(message);
		// writeQueue.Enqueue(message);
	}

}

/*
// I'll leave this here temporarily because it has all our mocking information and what not
	public void StartGameLoop()
	{
		// TODO: Should be obvious
		while (true)
		{

			break;
		}
		// This is just to test whether it would load

		// Making a second card doesn't work too well
		// TestCard = new CardData(10, "farmer", Colour.BLUE, 100, 10, 5);
		// CardTexture = Builder.BuildCard(TestCard);
		// CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
		// var CardTemp = CardScene.Instantiate<Card>();
		// CardTemp.LoadDataTexture(CardTexture);
		// CardManager.AddChild(CardTemp);



		// Testing attack phase -> Call this function when you somehow detect a card is played on the field
		// CardTemp.EnterBattlefield();

		// This is just to test whether it would load
		// SINCE THIS IS NOT TO BE CONSTANTLY DESTROYED AND RECREATED, THIS IS A POC TO TEST IF THE
		// THING WILL LOAD (sorry Jared)
		// GD.Print("Attempting to create the initial player view");
		// try {
		// 	var PlayerTextureView = Builder.BuildPlayer();
		// 	// var PlayerIcon = (PlayerView)FindChild("PlayerIcon");
		// 	var PlayerTrial = GD.Load<PackedScene>("res://scenes/PlayerIcon.tscn");
		// 	var PlayerIcon = PlayerTrial.Instantiate<PlayerView>();
		// 	PlayerIcon.LoadDataTexture(PlayerTextureView);
		// 	PlayerIcon.Scale = new Vector2(0.35f, 0.35f);
		// 	AddChild(PlayerIcon);

		// } catch (Exception e) {
		// 	GD.PrintErr("Exception: ", e);
		// }

		// GD.Print("Successfully created the initial player view");

		// See how to initialise Elixir

		ReturnToHomeScreen();
	}
*/
