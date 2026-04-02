namespace FSDClient.card.red.mechanics;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class Bombadier : Card
{

    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        player2Health -= 15; 
        base.SpawnCard(OpponentBoard, Board, battleslot,ref  player1Health, ref player2Health);

    }

}
