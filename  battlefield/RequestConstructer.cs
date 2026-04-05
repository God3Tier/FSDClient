namespace FSDClient.battlefield;

using System.Text.Json;
using System.Text.Json.Serialization;
using FSDClient.battlefield.responseType;

public enum RequestAction
{
    JOIN_GAME,
    CARD_PLACED
}

public class RequestConstructor
{
    [JsonPropertyName("action")]
    private string Action { get; set; }
    [JsonPropertyName("params")]
    private string Params { get; set; }
    [JsonPropertyName("state_hash_after")]
    private ulong StateHashAfter { get; set; }
    [JsonPropertyName("sequence_number")]
    private long SequenceNumber { get; set; }


    public string GenerateRequest(RequestAction req, string parameters, PlayerState playerstate)
    {   
        Params = parameters;
        Action = req.ToString();
        StateHashAfter = playerstate.HashPlayerView();
        SequenceNumber = playerstate.SequenceNumber; 
        return JsonSerializer.Serialize(this);
    }

}
