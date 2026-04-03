namespace FSDClient.card.mechanics.blue;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;

public partial class Pufferfish : Card
{
    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        if (OpponentBoard[0][battleslot.y] != null) {
            // Bounce card back into hand 
        } else if (OpponentBoard[1][battleslot.y] != null) {
            // Logic to bounce back into hand 
        }
        
        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);

    }
}
