namespace FSDClient.resource;

using Godot;

[GlobalClass]
public partial class CardStatsTable : Resource
{
	private static readonly string BASE_DIRECTORY = "res://godotResources";

	// Explicit mapping of card ID to resource file.
	// In exported builds, DirAccess cannot enumerate packed files and
	// C# script types on .tres resources are not always resolved.
	private static readonly (int id, string file)[] CardEntries = {
		(1,  "pig.tres"),               (2,  "farmer.tres"),
		(3,  "mercenary.tres"),         (4,  "town_guard.tres"),
		(5,  "town_hero.tres"),         (6,  "travelling_merchant.tres"),
		(7,  "barbarian.tres"),         (8,  "dwarf.tres"),
		(9,  "bombadier.tres"),         (10, "ninja.tres"),
		(11, "apache.tres"),            (12, "dinosaur.tres"),
		(13, "penguin.tres"),           (14, "apprentice_magician.tres"),
		(15, "krazy_kraken.tres"),      (16, "pufferfish.tres"),
		(17, "magic_swordman.tres"),    (18, "big_whale.tres"),
		(19, "swamp_ogre.tres"),        (20, "nymph.tres"),
		(21, "living_tree.tres"),       (22, "dryad.tres"),
		(23, "alpha_wolf.tres"),        (24, "quetzalcoatl.tres"),
		(25, "glass_bones.tres"),       (26, "traitor.tres"),
		(27, "plague_doctor.tres"),     (28, "witch.tres"),
		(29, "dullahan.tres"),          (30, "shikigami.tres"),
		(31, "lazy_chick.tres"),        (32, "angel.tres"),
		(33, "holy_spear_knight.tres"), (34, "cat_sith.tres"),
		(35, "paladin.tres"),           (36, "archangel.tres"),
		(37, "technoblade.tres"),       (38, "wolf.tres"),
	};

	public Godot.Collections.Dictionary<int, CardStats> cardInfo = new();

	public CardStatsTable()
	{
		foreach (var (id, fileName) in CardEntries)
		{
			string path = BASE_DIRECTORY + "/" + fileName;
			try
			{
				var resource = ResourceLoader.Load(path);
				if (resource == null)
				{
					GD.PrintErr($"Failed to load card resource: {path}");
					continue;
				}

				CardStats cardTemp;
				if (resource is CardStats cs)
				{
					// Editor / debug mode: direct cast works
					cardTemp = cs;
				}
				else
				{
					// Exported build: C# script type not resolved.
					// Read properties via Godot's Get() which reads the
					// serialized .tres values even without the script type.
					cardTemp = new CardStats();
					cardTemp.id = id;

					var v = resource.Get("Name");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Name = (string)v;
					v = resource.Get("Colour");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Colour = (string)v;
					v = resource.Get("Rarity");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Rarity = (string)v;
					v = resource.Get("Cost");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Cost = (int)v;
					v = resource.Get("Attack");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Attack = (int)v;
					v = resource.Get("Health");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Health = (int)v;
					v = resource.Get("Effect");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Effect = (string)v;
					v = resource.Get("Summon");
					if (v.VariantType != Variant.Type.Nil) cardTemp.Summon = (string)v;
					v = resource.Get("OnAttack");
					if (v.VariantType != Variant.Type.Nil) cardTemp.OnAttack = (string)v;
					v = resource.Get("OnDamaged");
					if (v.VariantType != Variant.Type.Nil) cardTemp.OnDamaged = (string)v;
					v = resource.Get("OnDeath");
					if (v.VariantType != Variant.Type.Nil) cardTemp.OnDeath = (string)v;
					v = resource.Get("Image");
					if (v.VariantType != Variant.Type.Nil && v.Obj is Texture2D tex)
						cardTemp.Image = tex;
				}

				// Always override id from our known mapping
				cardTemp.id = id;
				cardInfo.Add(id, cardTemp);
				GD.Print($"Loaded card: {fileName} id={id}");
			}
			catch (System.Exception e)
			{
				GD.PrintErr($"Error loading {fileName}: {e.Message}");
			}
		}
	}
}
