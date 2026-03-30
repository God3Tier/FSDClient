namespace FSDClient.resource;

using Godot;

[GlobalClass]
public partial class CardStatsTable : Resource
{
	private static readonly string BASE_DIRECTORY = "res://resource/godotResources";
	public Godot.Collections.Dictionary<int, CardStats> cardInfo = new();

	public CardStatsTable()
	{
		using var dir = DirAccess.Open(BASE_DIRECTORY);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				var cardTemp = GD.Load<CardStats>(BASE_DIRECTORY + "/" + fileName);
				cardInfo.Add(cardTemp.CardId, cardTemp);
				GD.Print($"Found file: {fileName}");
				fileName = dir.GetNext();
			}
		}
		else
		{
			GD.Print("An error occurred when trying to access the path.");
		}
	}
}
