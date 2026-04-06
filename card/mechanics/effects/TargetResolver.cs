using FSDClient.card.display;
using System.Collections.Generic;

// If front end ever needs to mock the targets
namespace FSDClient.card.effects
{
	public interface ITargetResolver
	{
		List<Card> ResolveTargets(EffectContext context);
	}
}