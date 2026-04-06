namespace FSDClient.battlefield;

using FSDClient.builder;
using Godot;
using FSDClient.player;
using FSDClient.card.display;
using FSDClient.card;
using FSDClient.battlefield.handManagement;
using FSDClient.autoLoad;
using FSDClient.battlefield.responseType;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System;
using System.Text.Json;
using FSDClient.battlefield.response;


public partial class Gameloop : Node2D
{
    public static readonly string BASE_WEBSOCKET_URL = "ws://localhost:8002/ws?session_id=SESSIONID";

    public static readonly double MAX_ELIXER = 8;
    public static readonly double ROUND_TIMER = 10.0;
    public static readonly double PAUSE_TIMER = 5.0;
    public static readonly double SECONDS_PER_ELIXIR = 3f;
    public static readonly int BASE_ELIXIR = 4;

    public NetworkManager NetworkManager { get; set; }
    // Unsure to keep this as the state manager or make it it's own individual player data. TBD on a later date
    public PlayerStateManager MainPlayer { get; set; }
    public PlayerData IncomingPlayer { get; set; }

    // Deal with card placement and card movement
    private Card[][] Board { get; set; } = new Card[2][];
    private Card[][] OpponentBoard { get; set; } = new Card[2][];
    private CardManager CardManager;
    private HandArea HandArea;
    private DeckSpace DeckSpace;

    // Deal with active gameplay
    public int Player1Health;
    public int Player2Health;
    public PlayerState PlayerState { get; set; }

    // Parameters for game state to be managed
    private int Elixir { get; set; }
    private int RoundNumber { get; set; }
    private double GameTimer { get; set; } = 0;
    private double RegenInterval { get; set; } = 1;
    private bool TurnPause { get; set; } = true;
    private double PauseTimer { get; set; } = 0;
    private int TurnRound = 1;
    private bool GameEnd { get; set; } = false;

    // Websocket Connection and managing concurrency
    private WebSocketPeer Socket = new WebSocketPeer();
    private readonly ConcurrentQueue<string> readQueue = new();
    private readonly ConcurrentQueue<string> writeQueue = new();
    private double _reconnectTimer { get; set; }
    private readonly double _reconnectDelay = 3.0;
    private readonly ConcurrentQueue<AttackEvent> EventQueue = new();

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


        for (int i = 0; i < 2; i++)
        {
            Board[i] = new Card[3];
            OpponentBoard[i] = new Card[3];
        }

        foreach (Node child in GetChildren())
        {
            string childName = child.Name.ToString();
            if (childName.Contains("BattleSlot"))
            {
                var BattleSlot = (BattleSlot)child;
                int lastInt = (int)(childName[^1] - '1');

                BattleSlot.x = lastInt / 3;
                BattleSlot.y = lastInt % 3;
                GD.Print(child.Name);

            }

        }

        var opponentsCards = (Control)FindChild("OpponentsCards");

        foreach (Node child in opponentsCards.GetChildren())
        {
            if (child is Card c)
            {
                GD.Print("Found opponent card removing texture it");
                c.EmptyTexture();
                // c.Scale =  -> Set scale
            }
        }

        //TODO: Add the logic to load the nonsence so I can start using the stuff for damage numbers
        // and whatnot
        // IncomingPlayer = new PlayerData("Placeholder", "Placeholder", [], false);
        CardManager = (CardManager)FindChild("CardManager", true);
        CardManager._playerHand = GetNode<PlayerHand>("HandArea/PlayerHand");
        CardManager._deckSpace = GetNode<DeckSpace>("HandArea/DeckSpace");
        CardManager.CardDropped += OnCardDropped;
        // Testing the HandArea
        HandArea = GetNode<HandArea>("HandArea");
        HandArea._playerHand = CardManager._playerHand;
        HandArea._deckSpace = CardManager._deckSpace;

        HandArea._playerHand.AddCardMessage += OnCardAdd;
        HandArea._playerHand.RemoveCardMessage += OnCardReturn; 
        
        GD.Print(HandArea);
        GD.Print(CardManager._playerHand.Name);

        TestCard("Card1");
        TestCard("Card2");
        TestCard("Card3");
        TestCard("Card4");
        TestCard("Card5");
        TestCard("Card6");
        TestCard("Card7");
        TestCard("Card8");
        HandArea.RaiseDeck();
        GD.Print("Completed everything without a problem");
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
        var obj = new {
            card_id = cardID
        };
        WriteToServer(RequestAction.DESELECT_CARD, JsonSerializer.Serialize(obj));
    }

    private void TestCard(string Name)
    {
        var TestCard = new CardData(10, "farmer", Colour.RED, 100, 10, 5);
        var CardTexture = Builder.BuildCard(TestCard);
        var CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
        var CardTemp = CardScene.Instantiate<Card>();
        CardTemp.Name = Name;
        CardTemp.CurrentSlotStatus = Card.SlotStatus.Deck;
        CardTemp.ZIndex = 4;
        CardTemp.LoadDataTexture(CardTexture);
        CardManager.AddChild(CardTemp);
        CardManager._deckSpace.AddCard(CardTemp);
    }

    private string ConstructWebsocketUrl()
    {
        var session = PlayerStateManager.Instance.SessionId;
        string BaseUrl = BASE_WEBSOCKET_URL.Replace("SESSIONID", session);
        GD.Print(BaseUrl);
        return BaseUrl;
    }

    private void OnAttacked(Card card)
    {
        int ActiveY = card.ActiveY;
        if (OpponentBoard[0][ActiveY] == null && OpponentBoard[0][ActiveY] == null)
        {
            // Handle logic for player getting attacked and opponent getting counterAttack
            GD.Print("Counter attack succesful");
        }
        else if (OpponentBoard[0][ActiveY] == null)
        {
            OpponentBoard[1][ActiveY].UpdateHealth(card.Attack);
        }
        else
        {
            OpponentBoard[0][ActiveY].UpdateHealth(card.Attack);
        }
        int player1Copy = Player1Health;
        int player2Copy = Player2Health;
        card.AttackOpponent(OpponentBoard, Board, ref player1Copy, ref player2Copy);
        Player1Health = player1Copy;
        Player2Health = player2Copy;
    }

    // This function is a proof of concept
    private void OnCardDropped(BattleSlot battleslot)
    {
        // TODO: Write to Server should be implemented once backend decides how to transfer information
        var playerStateManager = PlayerStateManager.Instance;
        var obj = new
        {
            card_id = battleslot.Card.CardID,
            pos_x = battleslot.x,
            pos_y = battleslot.y,
        };

        WriteToServer(RequestAction.CARD_PLACED, JsonSerializer.Serialize(obj));
        // Simulate delay of card
        // Thread.Sleep(1);
        battleslot.Card.ActiveY = battleslot.y;
        battleslot.Card.ActiveX = battleslot.x;
        battleslot.Card.Attacked += OnAttacked;

        int player1Copy = Player1Health;
        int player2Copy = Player2Health;
        battleslot.Card.SpawnCard(OpponentBoard, Board, battleslot, ref player1Copy, ref player2Copy);
        Player1Health = player1Copy;
        Player2Health = player2Copy;
    }

    // This is how to generate Elixir. Since client only ever knows about 1 player's resource
    // i can do it within the gameplay loop itself
    public override void _Process(double delta)
    {
        HandleWebSocket();
        HandleGameTimer(delta);
        HandleInputFromServer();
        ProcessEvent();
    }

    public void ProcessEvent()
    {
        if (EventQueue.TryDequeue(out AttackEvent attackEvent))
        {
            // Manage it here
            // I have no idea if this is correct or not
            if (MainPlayer.UserId != attackEvent.AttackerId)
            {

                if (attackEvent.TargetIsLeader)
                {
                    Player2Health -= attackEvent.Damage;
                    OpponentBoard[attackEvent.AttackerCol][attackEvent.AttackerRow].Health -= attackEvent.CounterDamage;
                }
                else
                {
                    Board[attackEvent.TargetCol][attackEvent.TargetRow].OnDamaged(Board, OpponentBoard, attackEvent.Damage, attackEvent.TargetCol, attackEvent.TargetRow);
                }

            }
            else
            {
                if (attackEvent.TargetIsLeader)
                {
                    Player1Health -= attackEvent.Damage;
                    Board[attackEvent.AttackerCol][attackEvent.AttackerRow].Health -= attackEvent.CounterDamage;
                }
                else
                {
                    OpponentBoard[attackEvent.TargetCol][attackEvent.TargetRow].OnDamaged(OpponentBoard, Board, attackEvent.Damage, attackEvent.TargetCol, attackEvent.TargetRow);
                }
            }
        }
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

        if (writeQueue.TryDequeue(out string msg))
        {
            Socket.SendText(msg);
        }

    }

    private void HandleInputFromServer()
    {
        while (readQueue.TryDequeue(out string msg))
        {
            GD.Print(msg);
            try
            {
                var Data = JsonSerializer.Deserialize<ResponseManager>(msg);
                if (Data.Result.Equals("failure"))
                {
                    // Some error handling
                    return;
                }


                if (Enum.TryParse<ActionType>(Data.ActionType, out var actionType))
                {
                    switch (actionType)
                    {
                        case ActionType.CardPlaced:
                            {
                                PlayerState = JsonSerializer.Deserialize<PlayerState>(Data.Parameters);
                                break;
                            }
                        case ActionType.TickUpdate:
                            {
                                var tickUpdate = JsonSerializer.Deserialize<TickUpdater>(Data.Parameters);


                                if (TurnPause && CardManager._deckSpace._cardCount != tickUpdate.DrawPile.Length)
                                {
                                    foreach (var card in tickUpdate.DrawPile)
                                    {
                                        var cardTemp = CardBuilder.GenerateCard(card.CardID);
                                        CardManager.AddChild(cardTemp);
                                        CardManager._deckSpace.AddCard(cardTemp);
                                    }

                                    return;

                                }

                                foreach (var action in tickUpdate.AttackEvent)
                                {
                                    EventQueue.Enqueue(action);
                                }

                                foreach (var board in tickUpdate.EnemyBoard)
                                {
                                    if (OpponentBoard[board.Col][board.Row].IsEmpty)
                                    {
                                        CardBuilder.LoadTextureFromId(board.CardID, OpponentBoard[board.Col][board.Row]);
                                        OpponentBoard[board.Col][board.Row].IsEmpty = false;
                                    }
                                }

                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    PlayerState = JsonSerializer.Deserialize<PlayerState>(Data.Parameters);
                }

            }
            catch (Exception e)
            {
                GD.PrintErr("Unable to serialize ", e);
            }

            // Here, we somehow parse said information about card and then mess around with it. But to continue, I need to settle card Dictionary
        }
    }

    // This is to handle the
    private void HandleGameTimer(double delta)
    {
        // GD.Print("Called");
        if (TurnPause)
        {
            PauseTimer += delta;
        }
        else
        {
            RegenInterval += delta;
            GameTimer += delta;
        }
        if (PauseTimer >= PAUSE_TIMER)
        {
            GD.Print("Pause Ended");
            HandArea.LowerDeck();
            CardManager.UnstuckCard();
            CardManager._playerHand.ActivateCardsInHand();
            TurnPause = false;
            PauseTimer = 0;
            TurnRound += 1;
            var ElixirBar = (Elixir)FindChild("Elixir");
            ElixirBar.UpdateRound(TurnRound);
            return;
        }

        if (GameTimer >= ROUND_TIMER * TurnRound && !TurnPause)
        {
            GD.Print("Round updated");
            // TODO: Trigger secondary draw card event
            TurnPause = true;
            HandArea.RaiseDeck();
            CardManager.UnstuckCard();
            CardManager._playerHand.PauseCardsInHand();
        }

        if (RegenInterval >= SECONDS_PER_ELIXIR)
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
        writeQueue.Enqueue(message);
    }

    private void ReturnToHomeScreen()
    {
        GameStateManager.Instance.ChangeGameState(GameState.HOMESCREEN);
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
