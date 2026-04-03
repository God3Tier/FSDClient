namespace FSDClient.card.mechanics.green;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;

public partial class AlphaWolf : Card
{
    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        int amountWolves = 0;

        for (int i = 0; i < Board.Length; i++)
        {
            for (int j = 0; j < Board[i].Length; j++)
            {
                if (amountWolves >= 2)
                {
                    break;
                }
                if (Board[i][j] == null)
                {
                    // Spawn wolves. I will do this when I figure out the card logic thing
                    amountWolves += 1;
                }
            }
        }


        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);

    }


}
