namespace FSDClient.card.mechanics.blue;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;

public partial class MagicSwordsmen : Card {
    
    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        if (OpponentBoard[0][battleslot.y] != null) {
            // Somehow force the card to become pig (this can be done by passing in params. I will settle this when i settle loading)
            // OpponentBoard[0][battleslot.y]; 
        } else if (OpponentBoard[1][battleslot.y] != null) {
            // Somehow force the card to become pig (this can be done by passing in params. I will settle this when i settle loading)
            // OpponentBoard[1][battleslot.y]; 
        }

        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);

    }
}
