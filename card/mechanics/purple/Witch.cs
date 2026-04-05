namespace FSDClient.card.purple.mechanics;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using System;
using Godot;

public partial class Witch : Card
{
	public int SummonHealth = -10;
	public int SummonAttack = -15;
	
	public void OnDeath(Card[][] OpponentBoard, Card[][] Board)
	{
		Board[ActiveX][ActiveY] = null;
	}
}
