namespace FSDClient.card.mechanics.red;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class Dwarf : Card
{

    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        if (Board[battleslot.x - 1][battleslot.y] != null)
        {
            Board[battleslot.x - 1][battleslot.y].Attack += 10;
            ((RichTextLabel)battleslot.Card.FindChild("Attack", true)).Text = battleslot.Card.Attack.ToString();

        }
        base.SpawnCard(OpponentBoard, Board, battleslot,ref  player1Health, ref player2Health);

    }

}
