using FSDClient.builder;
using Godot;
using FSDClient.player;
using FSDClient.card.display;
using FSDClient.card;
using FSDClient.battlefield.handManagement;
using FSDClient.autoLoad;
using System.Threading;
using System.Collections.Concurrent;

namespace FSDClient.battlefield;


public partial class Gameloop : Node2D
{
	public static readonly string WEBSOCKET_URL = "";
	public static readonly double MAX_ELIXER = 8;
	public static readonly double ROUND_TIMER = 5.0;
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

    // Parameters for game state to be managed
    private int Elixir { get; set; }
    private int RoundNumber { get; set; }
    private double GameTimer { get; set; } = 0;
    private double RegenInterval { get; set; } = 1;
    private bool TurnPause { get; set; } = false;
    private double PauseTimer { get; set; } = 0;
    private int TurnRound = 1;

    // Websocket Connection and managing concurrency
    private WebSocketPeer Socket = new WebSocketPeer();
    private readonly ConcurrentQueue<string> readQueue = new();
    private readonly ConcurrentQueue<string> writeQueue = new();


    // I am of the assumption that this is what is being called by the
    // We put this in gameloop later
    public override void _Ready()
    {
        Socket.ConnectToUrl(WEBSOCKET_URL);
        HandArea = GetNode<HandArea>("HandArea");
        HandArea.RaiseDeck();

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
        MainPlayer = PlayerStateManager.Instance;
        // IncomingPlayer = new PlayerData("Placeholder", "Placeholder", [], false);
        CardManager = (CardManager)FindChild("CardManager", true);
        CardManager._playerHand = GetNode<PlayerHand>("HandArea/PlayerHand");
        CardManager._deck = GetNode<Control>("HandArea/Deck");
        CardManager.CardDropped += OnCardDropped;
        GD.Print(CardManager._playerHand.Name);

        TestCard();
        GD.Print("Completed everything without a problem");
    }

    private void TestCard()
    {
        var TestCard = new CardData(10, "farmer", Colour.RED, 100, 10, 5);
        var CardTexture = Builder.BuildCard(TestCard);
        var CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
        var CardTemp = CardScene.Instantiate<Card>();
        CardTemp.LoadDataTexture(CardTexture);
        CardManager.AddChild(CardTemp);
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
    }

    // This function is a proof of concept
    private void OnCardDropped(BattleSlot battleslot)
    {
        // TODO: Write to Server should be implemented once backend decides how to transfer information
        WriteToServer("");
        // Simulate delay of card
        Thread.Sleep(1);
        GD.Print("Updating Board");
        Board[battleslot.x][battleslot.y] = battleslot.Card;
        battleslot.Card.ActiveY = battleslot.y;
        battleslot.Card.Attacked += OnAttacked;
        battleslot.Card.EnterBattlefield();

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

		if (Socket.GetReadyState() != WebSocketPeer.State.Open)
		{
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
		if (readQueue.TryDequeue(out string msg))
		{
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
	public void WriteToServer(string message)
	{
		writeQueue.Enqueue(message);
	}

	private void ReturnToHomeScreen()
	{
		GameStateManager.Instance.ChangeGameState(GameState.HOMESCREEN);
	}

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

		// Testing the HandArea
		HandArea = GetNode<HandArea>("HandArea");
		GD.Print(HandArea);

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


}
