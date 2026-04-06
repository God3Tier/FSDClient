
namespace FSDClient.card.mechanics.red;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;

public partial class Ninja : Card
{

    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        battleslot.Card.Timer /= 2;
        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);
    }

}
