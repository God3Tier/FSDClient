namespace FSDClient.card.red.mechanics;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class Ninja : Card
{

    public void OnDamaged(Card[][] OpponentBoard, Card[][] Board, int damageTaken)
    {
        Attack += 10;
        ((RichTextLabel)FindChild("Attack", true)).Text = Attack.ToString();
        base.OnDamaged(OpponentBoard, Board, damageTaken);
    }

}
