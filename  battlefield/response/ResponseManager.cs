namespace FSDClient.battlefield.response;

using System.Text.Json.Serialization;


public enum MessageType
{
    StateUpdate,
    ActionResult
}

public enum ActionType
{
    JoinGame,
    SelectCards,
    CardPlaced,
    TickUpdate
}

public class ResponseManager
{
    [JsonPropertyName("message_type")]
    public string MessageType { get; set; }
    [JsonPropertyName("action_type")]
    public string ActionType { get; set; }
    [JsonPropertyName("params")]
    public string Parameters { get; set; }
    [JsonPropertyName("sequence_number")]
    public string SequenceNumber { get; set; }

}
