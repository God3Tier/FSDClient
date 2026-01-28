namespace Client.data;

using Godot;

public class CardData
{
    private int Cost { get; }
    private string Name { get; }
    private int Health { get; set; }
    private int Attack { get; }
    private Sprite2D Sprite2D { get; } = new();

    public CardData(int cost, string name)
    {
        Cost = cost;
        Name = name;

        // Set url of this later
        string url = "";
        var texture2D = ResourceLoader.Load<Texture2D>(url);
        Sprite2D.Texture = texture2D;
    }
}
