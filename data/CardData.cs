namespace Client.data;

using Godot;

public class CardData
{
    // Ts is not exaustive yet. I need to see whether i need a timer here. But 
    // How to sync with server? 
    private int Cost { get; }
    private string Name { get; }
    private int Health { get; set; }
    private int Attack { get; }
    private Sprite2D CardIcon { get; } = new();


    public CardData(int cost, string name)
    {
        Cost = cost;
        Name = name;

        // Set url of this later
        string url = "";
        // var texture2D = ResourceLoader.Load<Texture2D>(url);
        // CardIcon.Texture = texture2D;
    }
    
    
}
