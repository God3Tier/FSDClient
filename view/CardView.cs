namespace FSDClient.view;

using Godot;
using FSDClient.data;
using System.Collections.Generic;

/*
    This class is to construct the card's Texture nodes based on whatever information
    provided by the deckhand upon creation
*/
public partial class CardView : Control
{
    // The below states are instances because they need to be controlled by other methods later down the line,
    // hense unable to declare as a local scope in the constructor
    private TextureRect Border { get; set; }
    private TextureRect NumberBase { get; set; }
    private TextureRect Icon { get; set; }
    private Label ElixirCost { get; set; }
    private Label CurrentHealth { get; set; }

    // There should be a better way to do this. Ts looking like YandereDev's code
    // I think I should abstract the code to another preperation function for the scale. Otherwise
    // this looks abit too messy 
    public CardView(CardViewTextures CardViewTextures)
    {
        // Spawn the card diagrams (may have plans to abstract this to the record directly loading in)
        Border = new TextureRect();
        Border.Texture = CardViewTextures.BorderTexture;
        AddChild(Border);

        Icon = new TextureRect();
        Icon.Texture = CardViewTextures.IconTexture;
        Icon.Position = new Vector2(40, 0);
        AddChild(Icon);

        NumberBase = new TextureRect();
        NumberBase.Texture = CardViewTextures.LabelTexture;
        AddChild(NumberBase);

        var AttackLabel = new TextureRect();
        AttackLabel.Texture = CardViewTextures.AttackTexture;
        AttackLabel.Scale = new Vector2(0.75f, 0.75f);
        AttackLabel.Position = new Vector2(25, 345);
        AddChild(AttackLabel);

        var DefenceLabel = new TextureRect();
        DefenceLabel.Texture = CardViewTextures.LabelTexture;
        DefenceLabel.Scale = new Vector2(0.75f, 0.75f);
        DefenceLabel.Position = new Vector2(225, 345);
        AddChild(DefenceLabel);

        // THIS DOES NOT LOOK LIKE A GOOD IDEA
        CardViewTextures.AttackValue.Size = new Vector2(50, 30);
        CardViewTextures.AttackValue.AddThemeFontSizeOverride("font_size", 32);
        CardViewTextures.AttackValue.AddThemeColorOverride("font_color", Colors.Black);
        CardViewTextures.AttackValue.Position = new Vector2(70, 345);
        AddChild(CardViewTextures.AttackValue);

        CardViewTextures.ElixirCost.Size = new Vector2(50, 30);
        CardViewTextures.ElixirCost.AddThemeFontSizeOverride("font_size", 32);
        CardViewTextures.ElixirCost.AddThemeColorOverride("font_color", Colors.Black);
        CardViewTextures.ElixirCost.Position = new Vector2(80, 15);
        AddChild(CardViewTextures.ElixirCost);

        CardViewTextures.CurrentHealth.Size = new Vector2(50, 30);
        CardViewTextures.CurrentHealth.AddThemeFontSizeOverride("font_size", 32);
        CardViewTextures.CurrentHealth.AddThemeColorOverride("font_color", Colors.Black);
        CardViewTextures.CurrentHealth.Position = new Vector2(260, 345);
        AddChild(CardViewTextures.CurrentHealth);
    }

    // Fix the overlapping problem later
    public void OnFieldMode()
    {
        RemoveChild(Border);
        RemoveChild(NumberBase);
        RemoveChild(ElixirCost);
        Icon.Scale = new Vector2(1.1f, 1.1f);

    }

    public void UpdateHealth(int damageTaken)
    {
        if (int.TryParse(CurrentHealth.Text, out int health))
        {
            health -= damageTaken;
            CurrentHealth.Text = health.ToString();
        }
        else
        {
            GD.Print("Whoops");
        }
    }

}
