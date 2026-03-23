namespace FSDClient.player.display;

using Godot;
using FSDClient.builder;

public partial class PlayerView : Control
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
        
        var DefenceValue = (RichTextLabel)FindChild("DefenceValue");
        DefenceValue.Text = PlayerViewTextures.HealthValue;
    }
}