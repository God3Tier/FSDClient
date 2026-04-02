namespace FSDClient.card.red.mechanics;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class Barbarian : Card
{
    public bool Transformed = false;

    public void LoadDataTexture(CardViewTextures cardViewTextures)
    {
        if (Random.Shared.Next(1, 2049) == 2048)
        {
            Transformed = true;
        }
        base.LoadDataTexture(cardViewTextures);
    }


    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
    {
        if (Transformed)
        {
            battleslot.Card.TimeToAttack /= 2;
        }
        base.SpawnCard(OpponentBoard, Board, battleslot, ref player1Health, ref player2Health);

    }

}
