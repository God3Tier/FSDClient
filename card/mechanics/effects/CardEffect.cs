using FSDClient.card.display;
using System.Collections.Generic;

namespace FSDClient.card.effects
{
	// For grouping all the parameters that the effects may possibly need
	public class EffectContext
	{
		// Related to board state
		public Card Source { get; init; }
		public Card[][] Board { get; init; }
		public Card[][] OpponentBoard { get; init; }
		public BattleSlot BattleSlot { get; set; }
		// public PlayerHand PlayerHand { get; set; }
		// public PlayerHand OpponentHand { get; set; }
		
		public List<Card> Targets { get; set; } = new();
		
		// Player Health points
		public int Player1Health { get; set; };
		public int Player2Health { get; set; };
	}

	public class CardAbilitySet
	{
		public ICardEffect? OnSummon { get; set; }
		public ICardEffect? OnAttack { get; set; }
		public ICardEffect? OnDamaged { get; set; }
		public ICardEffect? OnDeath { get; set; }
	}
	
	// Spawn in effects
	public interface IOnSpawnEffect
	{
		void OnSpawn(EffectContext context);
	}
	
	// Damaged effects
	public interface IOnDamagedEffect
	{
		void OnSpawn(EffectContext context);
	}
	
	// Dying effects
	public interface IOnDeathEffect
	{
		void OnDeath(EffectContext context);
	}
	
	// after attacking effects
	public interface IAfterAttackEffect
	{
		void AfterAttack(EffectContext context);
	}
}