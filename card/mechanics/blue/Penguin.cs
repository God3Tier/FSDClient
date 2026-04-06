namespace FSDClient.card.mechanics.blue;
using FSDClient.card.display;
using FSDClient.battlefield.handManagement;

public partial class Penguin : Card
{
    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        for (int i = 0; i < OpponentBoard.Length; i++) {
            if (OpponentBoard[i][battleslot.y].IsEmpty) {
                OpponentBoard[i][battleslot.y].Attack -= 10;   
            }
        }
        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);
    }
}
