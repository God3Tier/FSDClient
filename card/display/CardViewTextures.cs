namespace FSDClient.card.display;

using Godot;

public record CardViewData(int CardID, string Color, string IconName, int Cost, int Health, int Attack, double TimeToAttack);

public class CardViewTextures
{
    private static readonly string BORDER_BASE_URL = "res://assets/cards/PLACEHOLDER_card_outline.png";
    private static readonly string ICON_BASE_URL = "res://assets/characters/PLACEHOLDER.png";
    private static readonly string LABEL_BASE_URL = "res://assets/cost/PLACEHOLDER_card_cost.png";
    private static readonly string ATTACK_LABEL = "res://assets/symbols/attack_symbol.png";
    private static readonly string DEFENCE_LABEL = "res://assets/symbols/defence_symbol.png";

    public int CardID { get; set; }
    public Texture2D BorderTexture { get; set; }
    public Texture2D IconTexture { get; set; }
    public Texture2D LabelTexture { get; set; }
    public Texture2D AttackTexture { get; set; }
    public Texture2D DefenceTexture { get; set; }
    public string AttackValue { get; set; }
    public int  ElixirCost { get; set; }
    public string CurrentHealth { get; set; }
    public double TimeToAttack { get; set; }

    public CardViewTextures(CardViewData CardViewData)
    {
        CardID = CardViewData.CardID;
        string BorderUrl = BORDER_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        BorderTexture = GD.Load<Texture2D>(BorderUrl);

        string IconURL = ICON_BASE_URL.Replace("PLACEHOLDER", CardViewData.IconName.Replace(" ", "_"));
        IconTexture = GD.Load<Texture2D>(IconURL);

        string LabelIcon = LABEL_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        LabelTexture = GD.Load<Texture2D>(LabelIcon);

        AttackTexture = GD.Load<Texture2D>(ATTACK_LABEL);

        DefenceTexture = GD.Load<Texture2D>(DEFENCE_LABEL);

        AttackValue = CardViewData.Attack.ToString();

        ElixirCost = CardViewData.Cost;

        CurrentHealth = CardViewData.Health.ToString();

        TimeToAttack = CardViewData.TimeToAttack;
    }
}
