namespace FSDClient.card.mechanics.green;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using Godot;

public partial class Nymph : Card
{
    private static readonly (int x, int y)[] COORDS = { (0, 1), (1, 0), (-1, 0), (0, -1) };

    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        foreach ((int x, int y) in COORDS)
        {
            int newX = x + battleslot.x;
            int newY = y + battleslot.y;

            if ((newX < 0 || newX >= 2 || newY < 0 || newY >= 2) && Board[newX][newY] != null)
            {
                continue;
            }

            Board[newX][newY].Attack += 10;
            ((RichTextLabel)Board[newX][newY].FindChild("Attack", true)).Text = Board[newX][newY].Attack.ToString();

            Board[newX][newY].Health += 10;
            ((RichTextLabel)Board[newX][newY].FindChild("Health", true)).Text = Board[newX][newY].Health.ToString();

        }
        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);
    }
}
