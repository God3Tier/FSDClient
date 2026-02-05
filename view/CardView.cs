namespace FSDClient.view;

using Godot;
using FSDClient.builder;

public partial class CardView : Control
{
    private static readonly string BORDER_BASE_URL = "res://assets/cards/PLACEHOLDER_card_outline.png";
    private static readonly string ICON_BASE_URL = "res://assets/cards/characters/PLACEHOLDER.png";
    private static readonly string NUMBER_BASE_URL = "res://assets/cost/PLACEHOLDER_card_cost.png";

    private TextureRect _border { get; set; }
    private TextureRect _icon { get; set; }
    private int CurrentHealth { get; set; }
    private int Attack { get; }

    public void Setup(CardViewData CardViewData)
    {
        _border = new TextureRect();
        string BorderUrl = BORDER_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        Texture2D border = GD.Load<Texture2D>(BorderUrl);
        _border.Texture = border;

        _icon = new TextureRect();
        string IconURL = ICON_BASE_URL.Replace("PLACEHOLDER", CardViewData.IconName);
        Texture2D Icon = GD.Load<Texture2D>(IconURL);
        _icon.Texture = Icon;

        

    }
    
}
