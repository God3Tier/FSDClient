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

    private TextureRect Border { get; set; }
    private TextureRect NumberBase { get; set; }
    private TextureRect Icon { get; set; }
    private TextureRect AttackLabel { get; set; }
    private TextureRect DefenceLabel { get; set; }
    private Label AttackValue { get; set; }
    private Label ElixirCost { get; set; }
    private Label CurrentHealth { get; set; }

    /*
        Instantiate the CardView as a Control that has all the different textures it renders as a 
        
    */
    public CardView(CardViewTextures CardViewTextures)
    {
        // Spawn the card diagrams (may have plans to abstract this to the record directly loading in)
        Border = new TextureRect();
        Border.Texture = CardViewTextures.BorderTexture;
        AddChild(Border);

        Icon = new TextureRect();
        Icon.Texture = CardViewTextures.IconTexture;
        AddChild(Icon);

        NumberBase = new TextureRect();
        NumberBase.Texture = CardViewTextures.LabelTexture;
        AddChild(NumberBase);

        AttackLabel = new TextureRect();
        AttackLabel.Texture = CardViewTextures.AttackTexture;
        AddChild(AttackLabel);

        DefenceLabel = new TextureRect();
        DefenceLabel.Texture = CardViewTextures.LabelTexture;
        AddChild(DefenceLabel);

        // THIS DOES NOT LOOK LIKE A GOOD IDEA
        AttackValue = CardViewTextures.AttackValue;
        AddChild(AttackValue);

        ElixirCost = CardViewTextures.ElixirCost;
        AddChild(ElixirCost);

        CurrentHealth = CardViewTextures.CurrentHealth;
        AddChild(CurrentHealth);

        SetPosition();
    }
    
    // Again we have now just moved the problem somewhere else 
    private void SetPosition()
    {
        Icon.Position = new Vector2(40, 0);
        
        AttackLabel.Scale = new Vector2(0.75f, 0.75f);
        AttackLabel.Position = new Vector2(25, 345);
        
        DefenceLabel.Scale = new Vector2(0.75f, 0.75f);
        DefenceLabel.Position = new Vector2(225, 345);
        
        AttackValue.Size = new Vector2(50, 30);
        AttackValue.AddThemeFontSizeOverride("font_size", 32);
        AttackValue.AddThemeColorOverride("font_color", Colors.Black);
        AttackValue.Position = new Vector2(70, 345);
        
        ElixirCost.Size = new Vector2(50, 30);
        ElixirCost.AddThemeFontSizeOverride("font_size", 32);
        ElixirCost.AddThemeColorOverride("font_color", Colors.Black);
        ElixirCost.Position = new Vector2(80, 15);
        
        CurrentHealth.Size = new Vector2(50, 30);
        CurrentHealth.AddThemeFontSizeOverride("font_size", 32);
        CurrentHealth.AddThemeColorOverride("font_color", Colors.Black);
        CurrentHealth.Position = new Vector2(260, 345);
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
