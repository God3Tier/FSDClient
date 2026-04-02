namespace FSDClient.card.grey.mechanics;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class Pig : Card
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
        Board[battleslot.x][battleslot.y] = battleslot.Card;
        if (Transformed) {
            ((Sprite2D)battleslot.Card.FindChild("Icon", true)).Texture = GD.Load<Texture2D>("res://assets/characters/technoblade.png");
            ((RichTextLabel)battleslot.Card.FindChild("Attack", true)).Text = "995";
            battleslot.Card.Attack = 995;
        }
    }

}
