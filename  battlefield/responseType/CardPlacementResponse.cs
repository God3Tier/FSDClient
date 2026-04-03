namespace FSDClient.battlefield.responseType;

using System.Text.Json.Serialization;

public class CardPlacementResponse
{
	[JsonPropertyName("valid_input")]
	public bool ValidInput;
	// Minimum information required
	[JsonPropertyName("card_id")]
	public int CardID;
	[JsonPropertyName("x_pos")]
	public int XPos;
	[JsonPropertyName("YPos")]
	public int YPos;
	
	
}