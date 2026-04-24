namespace FSDClient.resource;

using Godot;

[GlobalClass]
public partial class CardStatsTable : Resource
{
	public Godot.Collections.Dictionary<int, CardStats> cardInfo = new();

	private static readonly string[] CARD_RESOURCE_PATHS = new string[]
	{
		"res://godotResources/alpha_wolf.tres",
		"res://godotResources/angel.tres",
		"res://godotResources/apache.tres",
		"res://godotResources/apprentice_magician.tres",
		"res://godotResources/archangel.tres",
		"res://godotResources/barbarian.tres",
		"res://godotResources/big_whale.tres",
		"res://godotResources/bombadier.tres",
		"res://godotResources/cat_sith.tres",
		"res://godotResources/dinosaur.tres",
		"res://godotResources/dryad.tres",
		"res://godotResources/dullahan.tres",
		"res://godotResources/dwarf.tres",
		"res://godotResources/farmer.tres",
		"res://godotResources/glass_bones.tres",
		"res://godotResources/holy_spear_knight.tres",
		"res://godotResources/krazy_kraken.tres",
		"res://godotResources/lazy_chick.tres",
		"res://godotResources/living_tree.tres",
		"res://godotResources/magic_swordman.tres",
		"res://godotResources/mercenary.tres",
		"res://godotResources/ninja.tres",
		"res://godotResources/nymph.tres",
		"res://godotResources/paladin.tres",
		"res://godotResources/penguin.tres",
		"res://godotResources/pig.tres",
		"res://godotResources/plague_doctor.tres",
		"res://godotResources/pufferfish.tres",
		"res://godotResources/quetzalcoatl.tres",
		"res://godotResources/shikigami.tres",
		"res://godotResources/swamp_ogre.tres",
		"res://godotResources/technoblade.tres",
		"res://godotResources/town_guard.tres",
		"res://godotResources/town_hero.tres",
		"res://godotResources/traitor.tres",
		"res://godotResources/travelling_merchant.tres",
		"res://godotResources/witch.tres",
		"res://godotResources/wolf.tres"
		// Add more card resource paths here as needed
	};

	public CardStatsTable()
	{
		foreach (var path in CARD_RESOURCE_PATHS)
		{
			var cardTemp = GD.Load<CardStats>(path);
			cardInfo.Add(cardTemp.id, cardTemp);
			GD.Print($"Loaded card: {path}");
		}
	}
}
