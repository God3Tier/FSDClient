namespace FSDClient.card.mechanics.purple;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class GlassBones : Card
{
	public int SelfDamage = 10;
	
	public void AttackOpponent(Card[][] OpponentBoard, Card[][] Board, ref int player1Health, ref int player2Health)
    {
        if (OpponentBoard[0][ActiveY] == null && OpponentBoard[0][ActiveY] == null)
        {
            // Handle logic for player getting attacked and opponent getting counterAttack
            GD.Print("Counter attack successful");
        }
        else if (OpponentBoard[0][ActiveY] == null)
        {
            OpponentBoard[1][ActiveY].UpdateHealth(Attack);
            if (OpponentBoard[1][ActiveY].Health == 0)
            {
                OpponentBoard[1][ActiveY].EmptyTexture();
            }
        }
        else
        {
            OpponentBoard[0][ActiveY].UpdateHealth(Attack);
            if (OpponentBoard[0][ActiveY].Health == 0)
            {
                OpponentBoard[0][ActiveY].EmptyTexture();
            }
        }
        
        this.UpdateHealth(SelfDamage);
        if (this.Health == 0)
        {
        	Board[ActiveX][ActiveY].EmptyTexture();
        }
    }

}
