namespace FSDClient.player.display;

using Godot;
using FSDClient.builder;

public partial class PlayerView : Node2D
{
    private int Health { get; set; }

    public override void _Ready()
    {

    }
    
    public override void _Process(double delta)
    {
        
    }

    public void LoadDataTexture(PlayerViewTextures PlayerViewTextures)
    {
        var Background = (Sprite2D)FindChild("Border");
        Background.Texture = PlayerViewTextures.BorderTexture;

        var Portrait = (Sprite2D)FindChild("Portrait");
        Portrait.Texture = PlayerViewTextures.IconTexture;
        Portrait.Scale = new Vector2(0.75f, 0.75f);

        var AttackValue = (RichTextLabel)FindChild("AttackValue");
        AttackValue.Text = PlayerViewTextures.AttackValue;
        // AddChild(AttackValue);
        
        var DefenceValue = (RichTextLabel)FindChild("DefenceValue");
        DefenceValue.Text = PlayerViewTextures.HealthValue;
        // AddChild(HealthValue);

        // SetPosition();
    }
    
    
    // Settle later 
    // public void SetPosition()
    // {
    //     AttackValue.Size = new Vector2(50, 30);
    //     AttackValue.AddThemeFontSizeOverride("font_size", 32);
    //     AttackValue.AddThemeColorOverride("font_color", Colors.Black);
    //     AttackValue.Position = new Vector2(75, 260);
        
    //     HealthValue.Size = new Vector2(50, 30);
    //     HealthValue.AddThemeFontSizeOverride("font_size", 32);
    //     HealthValue.AddThemeColorOverride("font_color", Colors.Black);
    //     HealthValue.Position = new Vector2(370, 260);

    //     CounterAttack.Position = new Vector2(0, 250);
    //     Defence.Position = new Vector2(320, 250);

    //     Portrait.Scale = new Vector2(0.7f, 0.7f);
    //     Portrait.Position = new Vector2(130, 30);
    // }
}