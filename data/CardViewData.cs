namespace FSDClient.data;

using Godot;

public record CardViewData(string Color, string IconName, int Cost, int Health, int Attack)
{

}

public class CardViewTextures
{
    private static readonly string BORDER_BASE_URL = "res://assets/cards/PLACEHOLDER_card_outline.png";
    private static readonly string ICON_BASE_URL = "res://assets/characters/PLACEHOLDER.png";
    private static readonly string LABEL_BASE_URL = "res://assets/cost/PLACEHOLDER_card_cost.png";
    private static readonly string ATTACK_LABEL = "res://assets/symbols/attack_symbol.png";
    private static readonly string DEFENCE_LABEL = "res://assets/symbols/defence_symbol.png";


    public Texture2D BorderTexture { get; set; }
    public Texture2D IconTexture { get; set; }
    public Texture2D LabelTexture { get; set; }
    public Texture2D AttackTexture { get; set; }
    public Texture2D DefenceTexture { get; set; }
    public Label AttackValue { get; set; }
    public Label ElixirCost { get; set; }
    public Label CurrentHealth { get; set; }

    public CardViewTextures(CardViewData CardViewData)
    {
        string BorderUrl = BORDER_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        BorderTexture = GD.Load<Texture2D>(BorderUrl);

        string IconURL = ICON_BASE_URL.Replace("PLACEHOLDER", CardViewData.IconName);
        IconTexture = GD.Load<Texture2D>(IconURL);

        string LabelIcon = LABEL_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        LabelTexture = GD.Load<Texture2D>(LabelIcon);

        AttackTexture = GD.Load<Texture2D>(ATTACK_LABEL);

        DefenceTexture = GD.Load<Texture2D>(DEFENCE_LABEL);
        
        AttackValue = new Label();
        AttackValue.Text = CardViewData.Attack.ToString();
        
        ElixirCost = new Label();
        ElixirCost.Text = CardViewData.Cost.ToString();
        
        CurrentHealth = new Label();
        CurrentHealth.Text = CardViewData.Health.ToString();

    }
}
