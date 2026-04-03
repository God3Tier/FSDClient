namespace FSDClient.card.mechanics.green;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using Godot;

public partial class Drayad : Card
{
    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        if (OpponentBoard[0][battleslot.y].IsEmpty || OpponentBoard[1][battleslot.y].IsEmpty)
        {
            battleslot.Card.Attack += 10;
            ((RichTextLabel)battleslot.Card.FindChild("Attack", true)).Text = battleslot.Card.Attack.ToString();

            battleslot.Card.Health += 10; 
            ((RichTextLabel)battleslot.Card.FindChild("Health", true)).Text = battleslot.Card.Health.ToString();

        }
        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);

    }
}
