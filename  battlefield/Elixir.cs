namespace FSDClient.battlefield;

using Godot;
using System;

public partial class Elixir : Node2D
{
    private static readonly string NULL_ElIXIR_URL = "res://assets/symbols/null_energy.png";
    private static readonly string NO_ELIXIR_URL = "res://assets/symbols/no_energy.png";
    private static readonly string ELIXIR_URL = "res://assets/symbols/energy.png";
    private int RoundNumber { get; set; }
    private int CurrElixir { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Set to 0 Energy
        for (int i = 0; i < 5; i++)
        {
            var Elixir = (Sprite2D)FindChild("NullEnergy" + i);
            Elixir.Texture = GD.Load<Texture2D>(NO_ELIXIR_URL);
        }
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    // Havent decided whether to set this at 0
    public void UpdateRound(int RoundValue)
    {
        if (RoundValue > 3)
        {
            return;
        }

        var Elixir = (Sprite2D)FindChild("NullEnergy" + 5 + RoundValue);
        Elixir.Texture = GD.Load<Texture2D>(NO_ELIXIR_URL);
    }

    public void UpdateElixir(int Elixir)
    {
        CurrElixir = Elixir;
        var ElixirTexture = (Sprite2D)FindChild("NullEnergy" + CurrElixir);
        ElixirTexture.Texture = GD.Load<Texture2D>(ELIXIR_URL);
    }
}
