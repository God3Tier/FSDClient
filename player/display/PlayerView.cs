namespace FSDClient.player.display;

using Godot;
using FSDClient.builder;

public partial class PlayerView : Control
{  
    private TextureRect Background { get; set;}
    private TextureRect CounterAttack { get; set;}
    private TextureRect Defence { get; set;}
    private TextureRect Portrait { get; set;}
    private Label AttackValue { get; set; }
    private Label HealthValue { get; set; }
    
    // Here, I will only need to store the health 
    public PlayerView(PlayerViewTextures PlayerViewTextures)
    {
        Background = new TextureRect();
        Background.Texture = PlayerViewTextures.BorderTexture;
        AddChild(Background);

        Portrait = new TextureRect();
        Portrait.Texture = PlayerViewTextures.IconTexture;
        AddChild(Portrait);
        
        CounterAttack = new TextureRect();
        CounterAttack.Texture = PlayerViewTextures.CounterAttackTexture;
        AddChild(CounterAttack);

        Defence = new TextureRect();
        Defence.Texture = PlayerViewTextures.DefenceSymbolTexture;
        AddChild(Defence);

        AttackValue = PlayerViewTextures.AttackValue;
        AddChild(AttackValue);

        HealthValue = PlayerViewTextures.HealthValue;
        AddChild(HealthValue);

        SetPosition();
    }
    
    // Settle later 
    public void SetPosition()
    {
        AttackValue.Size = new Vector2(50, 30);
        AttackValue.AddThemeFontSizeOverride("font_size", 32);
        AttackValue.AddThemeColorOverride("font_color", Colors.Black);
        AttackValue.Position = new Vector2(75, 260);
        
        HealthValue.Size = new Vector2(50, 30);
        HealthValue.AddThemeFontSizeOverride("font_size", 32);
        HealthValue.AddThemeColorOverride("font_color", Colors.Black);
        HealthValue.Position = new Vector2(370, 260);

        CounterAttack.Position = new Vector2(0, 250);
        Defence.Position = new Vector2(320, 250);

        Portrait.Scale = new Vector2(0.7f, 0.7f);
        Portrait.Position = new Vector2(130, 30);
    }
}