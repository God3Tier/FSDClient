namespace FSDClient.view;

using Godot;
using FSDClient.builder;

public partial class CardView : Control
{
    private static readonly string BORDER_BASE_URL = "res://assets/cards/PLACEHOLDER_card_outline.png";
    private static readonly string ICON_BASE_URL = "res://assets/characters/PLACEHOLDER.png";
    private static readonly string LABEL_BASE_URL = "res://assets/cost/PLACEHOLDER_card_cost.png";

    private TextureRect Border { get; set; }
    private TextureRect Icon { get; set; }
    private TextureRect NumberBase { get; set; }
    private Label Value { get; set; }
    private int CurrentHealth { get; set; }
    private int Attack { get; }

    public void Setup(CardViewData CardViewData)
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
        AddChild(Icon);

        NumberBase = new TextureRect();
        string LabelIcon = LABEL_BASE_URL.Replace("PLACEHOLDER", CardViewData.Color);
        Texture2D label = GD.Load<Texture2D>(LabelIcon);
        NumberBase.Texture = label;
        AddChild(NumberBase);


        Value = new Label();
        Value.Text = CardViewData.Cost.ToString();
        AddChild(Value);
        
    }
    
}
