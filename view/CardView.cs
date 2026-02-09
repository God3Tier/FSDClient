namespace FSDClient.view;

using Godot;
using FSDClient.builder;

/*
    This class is to construct the card's Texture nodes based on whatever information
    provided by the deckhand upon creation
*/
public partial class CardView : Control
{
    private static readonly string BORDER_BASE_URL = "res://assets/cards/PLACEHOLDER_card_outline.png";
    private static readonly string ICON_BASE_URL = "res://assets/characters/PLACEHOLDER.png";
    private static readonly string LABEL_BASE_URL = "res://assets/cost/PLACEHOLDER_card_cost.png";
    private static readonly string ATTACK_LABEL = "res://assets/symbols/attack_symbol.png";
    private static readonly string DEFENCE_LABEL = "res://assets/symbols/defence_symbol.png";
    
    private TextureRect Border { get; set; }
    private TextureRect NumberBase { get; set; }
    private TextureRect Icon { get; set; }
    private Label ElixirCost { get; set; } 
    private Label CurrentHealth { get; set; }

    // There should be a better way to do this. Ts looking like YandereDev's code 
    public CardView(CardViewData CardViewData)
    {
        // Spawn the card diagrams (may have plans to abstract this to the record directly loading in)
        Border = new TextureRect();
        string BorderUrl = BORDER_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        Texture2D border = GD.Load<Texture2D>(BorderUrl);
        Border.Texture = border;
        AddChild(Border);

        Icon = new TextureRect();
        string IconURL = ICON_BASE_URL.Replace("PLACEHOLDER", CardViewData.IconName);
        Texture2D icon = GD.Load<Texture2D>(IconURL);
        Icon.Texture = icon;
        Icon.Position = new Vector2(40, 0);
        AddChild(Icon);

        NumberBase = new TextureRect();
        string LabelIcon = LABEL_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        Texture2D label = GD.Load<Texture2D>(LabelIcon);
        NumberBase.Texture = label;
        AddChild(NumberBase);

        var AttackLabel = new TextureRect();
        Texture2D attackLabel = GD.Load<Texture2D>(ATTACK_LABEL);
        AttackLabel.Texture = attackLabel;
        AttackLabel.Scale = new Vector2(0.75f, 0.75f);
        AttackLabel.Position = new Vector2(25, 345);
        AddChild(AttackLabel);

        var DefenceLabel = new TextureRect();
        Texture2D defenceLabel = GD.Load<Texture2D>(DEFENCE_LABEL);
        DefenceLabel.Texture = defenceLabel;
        DefenceLabel.Scale = new Vector2(0.75f, 0.75f);
        DefenceLabel.Position = new Vector2(225, 345);
        AddChild(DefenceLabel);

        var AttackValue = new Label();
        AttackValue.Text = CardViewData.Attack.ToString();
        AttackValue.Size = new Vector2(50, 30);
        AttackValue.AddThemeFontSizeOverride("font_size", 32);
        AttackValue.AddThemeColorOverride("font_color", Colors.Black);
        AttackValue.Position = new Vector2(70, 345);
        AddChild(AttackValue);

        ElixirCost = new Label();
        ElixirCost.Text = CardViewData.Cost.ToString();
        ElixirCost.Size = new Vector2(50, 30);
        ElixirCost.AddThemeFontSizeOverride("font_size", 32);
        ElixirCost.AddThemeColorOverride("font_color", Colors.Black);
        ElixirCost.Position = new Vector2(80, 15);
        AddChild(ElixirCost);

        CurrentHealth = new Label();
        CurrentHealth.Text = CardViewData.Health.ToString();
        CurrentHealth.Size = new Vector2(50, 30);
        CurrentHealth.AddThemeFontSizeOverride("font_size", 32);
        CurrentHealth.AddThemeColorOverride("font_color", Colors.Black);
        CurrentHealth.Position = new Vector2(260, 345);
        AddChild(CurrentHealth);



    }
    
    // Fix the overlapping problem later 
    public void OnFieldMode() {
        RemoveChild(Border);
        RemoveChild(NumberBase);
        RemoveChild(ElixirCost);
        Icon.Scale = new Vector2(1.1f, 1.1f);
        
    }
    
    public void UpdateHealth(int damageTaken) {
        if (int.TryParse(CurrentHealth.Text, out int health)) {
            health -= damageTaken;
            CurrentHealth.Text = health.ToString();
        } else {
            GD.Print("Whoops");
        }
    }

}
