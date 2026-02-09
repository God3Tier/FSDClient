namespace FSDClient.view;

using Godot;
using FSDClient.builder;

public partial class PlayerView : Control
{
    private static readonly string PORTRAIT_BASE = "res://assets/portraits/portrait_PLACEHOLDER.png";
    private static readonly string BACKGROUND_BASE = "res://assets/heros/PLACEHOLDER_hero.png";
    private static readonly string COUNTER_ATTACK = "res://assets/symbols/counterattack_symbol.png";
    private static readonly string DEFENCE_SYMBOL = "res://assets/symbols/counterattack_symbol.png";
    
    public PlayerView(PlayerViewData playerViewData)
    {
        var Background = new TextureRect();
        var backgroundTexture = GD.Load<Texture2D>(BACKGROUND_BASE.Replace("PLACEHOLDER", playerViewData.Color));
        Background.Texture = backgroundTexture;
        AddChild(Background);

        var Portrait = new TextureRect();
        var portraitTexture = GD.Load<Texture2D>(PORTRAIT_BASE.Replace("PLACEHOLDER", playerViewData.IconName));
        Portrait.Texture = portraitTexture;
        AddChild(Portrait);

        var CounterAttack = new TextureRect();
        var counterAttackTexture = GD.Load<Texture2D>(COUNTER_ATTACK);
        CounterAttack.Texture = counterAttackTexture;
        AddChild(CounterAttack);
        
        
    }
}