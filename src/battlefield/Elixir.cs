namespace FSDClient.battlefield;

using Godot;
using System;

public partial class Elixir : Node2D
{
	// private static readonly string NULL_ElIXIR_URL = "res://assets/symbols/null_energy.png";
	private static readonly string NO_ELIXIR_URL = "res://assets/symbols/no_energy.png";
	private static readonly string ELIXIR_URL = "res://assets/symbols/energy.png";
	private int RoundNumber { get; set; } = 1;
	private int CurrElixir { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set to 0 Energy
		for (int i = 1; i <= 5; i++)

		{
			var Elixir = (Sprite2D)FindChild("NullEnergy" + i);
			Elixir.Texture = GD.Load<Texture2D>(NO_ELIXIR_URL);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void UpdateRound(int RoundValue)
	{
		if (RoundValue > 4)
		{
			return;
		}
		RoundNumber = RoundValue;
		var Elixir = (Sprite2D)FindChild("NullEnergy" + (4 + RoundValue));
		Elixir.Texture = GD.Load<Texture2D>(NO_ELIXIR_URL);
	}

	public void UpdateElixir(int Elixir)
	{
		try {
			CurrElixir = Elixir;
	
			for (int i = 0; i < CurrElixir; i++)
			{
				var ElixirTexture = (Sprite2D)FindChild("NullEnergy" + (i + 1));
				ElixirTexture.Texture = GD.Load<Texture2D>(ELIXIR_URL);
	
			}
	
			for (int i = CurrElixir + 1; i < (4 + RoundNumber); i++)
			{
				var ElixirTexture = (Sprite2D)FindChild("NullEnergy" + (i));
				ElixirTexture.Texture = GD.Load<Texture2D>(NO_ELIXIR_URL);
	
			}
	
			var ElixirValue = (RichTextLabel)FindChild("ElixirValue", true);
			ElixirValue.Text = "" + Elixir + "/" + (4 + RoundNumber);
			
		} catch (Exception e) {
			GD.Print("Unable to update Elixir because of ", e);
		}
	}
}
