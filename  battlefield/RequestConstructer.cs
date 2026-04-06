namespace FSDClient.battlefield;

using System.Text.Json;
using System.Text.Json.Serialization;
using FSDClient.battlefield.responseType;

public enum RequestAction
{
	JOIN_GAME,
	CARD_PLACED,
	RECONNECT,
	SELECT_CARD,
	DESELECT_CARD
}

public class RequestConstructor
{
	[JsonPropertyName("action")]
	private string Action { get; set; }
	[JsonPropertyName("params")]
	private JsonElement Params { get; set; }
	[JsonPropertyName("state_hash_after")]
	private ulong StateHashAfter { get; set; }
	[JsonPropertyName("sequence_number")]
	private long SequenceNumber { get; set; }


	public string GenerateRequest(RequestAction req, string parameters, PlayerState playerstate)
	{
		if (string.IsNullOrEmpty(parameters))
			Params = JsonSerializer.Deserialize<JsonElement>("{}");
		else
			Params = JsonSerializer.Deserialize<JsonElement>(parameters);
		Action = req.ToString();
		StateHashAfter = playerstate.HashPlayerView();
		SequenceNumber = playerstate.SequenceNumber;
		return JsonSerializer.Serialize(this);
	}

}
