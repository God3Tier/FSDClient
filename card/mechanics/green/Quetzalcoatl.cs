namespace FSDClient.card.mechanics.grey;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using Godot;

public partial class Quetzalcoatl : Card
{
    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        for (int i = 0; i < Board.Length; i++)
        {
            for (int j = 0; j < Board[i].Length; j++)
            {
                if (Board[i][j] == null)
                {
                    Board[i][j].Attack += 10;
                    ((RichTextLabel)Board[i][j].FindChild("Attack", true)).Text = Board[i][j].Attack.ToString();
        
                    Board[i][j].Health += 10;
                    ((RichTextLabel)Board[i][j].FindChild("Health", true)).Text = Board[i][j].Health.ToString();

                }
            }
        }


        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);

    }
}
