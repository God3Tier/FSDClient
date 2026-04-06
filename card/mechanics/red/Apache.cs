namespace FSDClient.card.mechanics.red;

using FSDClient.card.display;
using Godot;

public partial class Apache : Card
{
    public void OnDamaged(Card[][] OpponentBoard, Card[][] Board, int damageTaken,  int attackX, int attackY)
    {
        Attack += 10;
        ((RichTextLabel)FindChild("Attack", true)).Text = Attack.ToString();
        base.OnDamaged(OpponentBoard, Board, damageTaken, attackX, attackY);
    }
}
