namespace FSDClient.autoLoad;

using Godot;
using FSDClient.resource;

public partial class ResourceManager : Node
{
	public CardStatsTable CardStatsTable { get; set; }

	public override void _Ready()
	{
		CardStatsTable = new();
	}
    
	
}