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
	public string Action { get; set; }
	[JsonPropertyName("params")]
	public JsonElement Params { get; set; }
	[JsonPropertyName("state_hash_after")]
	public ulong StateHashAfter { get; set; }
	[JsonPropertyName("sequence_number")]
	public long SequenceNumber { get; set; }


	public string GenerateRequest(RequestAction req, string parameters, PlayerState playerstate)
	{
		if (string.IsNullOrEmpty(parameters))
			Params = JsonSerializer.Deserialize<JsonElement>("{}");
		else
			Params = JsonSerializer.Deserialize<JsonElement>(parameters);
		Action = req.ToString();
		if (playerstate == null)
		{
			StateHashAfter = 0;
			SequenceNumber = 0;
		}
		else
		{
			StateHashAfter = playerstate.HashPlayerView();
			SequenceNumber = (long)playerstate.TickNumber;
		}
		return JsonSerializer.Serialize(this);
	}

}
