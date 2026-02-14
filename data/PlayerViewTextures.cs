namespace FSDClient.data;

using Godot;

public record PlayerViewData(string IconName, string Color, int Health, int Attack);

public class PlayerViewTextures
{

    private static readonly string BACKGROUND_BASE = "res://assets/portraits/portrait_PLACEHOLDER.png";
    private static readonly string PORTRAIT_BASE = "res://assets/heros/PLACEHOLDER_hero.png";
    private static readonly string COUNTER_ATTACK = "res://assets/symbols/counterattack_symbol.png";
    private static readonly string DEFENCE_SYMBOL = "res://assets/symbols/defence_symbol.png";

    public Texture2D BorderTexture { get; set; }
    public Texture2D IconTexture { get; set; }
    public Texture2D CounterAttackTexture { get; set; }
    public Texture2D DefenceSymbolTexture { get; set; }
    public Label AttackValue { get; set; }
    public Label HealthValue { get; set; }

    public PlayerViewTextures(PlayerViewData playerViewData)
    {
        BorderTexture = GD.Load<Texture2D>(BACKGROUND_BASE.Replace("PLACEHOLDER", playerViewData.Color));
        IconTexture = GD.Load<Texture2D>(PORTRAIT_BASE.Replace("PLACEHOLDER", playerViewData.IconName));

        if (IconTexture == null)
        {
            GD.Print("Error unable to load texture\n");
        }

        CounterAttackTexture = GD.Load<Texture2D>(COUNTER_ATTACK);
        if (CounterAttackTexture == null)
        {
            GD.Print("Error unable to load texture\n");
        }
        DefenceSymbolTexture = GD.Load<Texture2D>(DEFENCE_SYMBOL);
        if (DefenceSymbolTexture == null)
        {
            GD.Print("Error unable to load texture\n");
        }

        AttackValue = new Label();
        AttackValue.Text = playerViewData.Attack.ToString();
        if (AttackValue == null)
        {
            GD.Print("Error unable to load texture\n");
        }

        HealthValue = new Label();
        HealthValue.Text = playerViewData.Health.ToString();
        if (IconTexture == null)
        {
            GD.Print("Error unable to load texture\n");
        }
    }

}
