namespace FSDClient.battlefield.response;

using System.Text.Json.Serialization;

public class StateView
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }

    [JsonPropertyName("phase")]
    public string Phase { get; set; }

    [JsonPropertyName("sequence_number")]
    public long SequenceNumber { get; set; }

    [JsonPropertyName("your_user_id")]
    public long YourUserId { get; set; }

    [JsonPropertyName("your_username")]
    public string YourUsername { get; set; }

    [JsonPropertyName("opponent_user_id")]
    public long OpponentUserId { get; set; }

    [JsonPropertyName("opponent_username")]
    public string OpponentUsername { get; set; }

    [JsonPropertyName("opponent_connected")]
    public bool OpponentConnected { get; set; }

    [JsonPropertyName("tick_number")]
    public ulong TickNumber { get; set; }

    [JsonPropertyName("state_hash")]
    public ulong StateHash { get; set; }
}