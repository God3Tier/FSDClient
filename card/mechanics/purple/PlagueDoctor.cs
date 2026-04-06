
namespace FSDClient.card.mechanics.purple;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class PlagueDoctor  : Card
{
	public int SummonHealth = -10;
	public int SummonAttack = -15;
	
	public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot, ref int player1Health, ref int player2Health)
	{
		GD.Print("Updating Board");
		Board[battleslot.x][battleslot.y] = this;
		battleslot.Card.EnterBattlefield();
		
		for (int i = 0; i < 2; i++) {
	   		if (!OpponentBoard[i][1].IsEmpty)
		 	{
		  		OpponentBoard[i][1].UpdateHealth(SummonHealth);
				OpponentBoard[i][1].Attack += SummonAttack;
		  	}
		}
	}
}
